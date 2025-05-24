using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Repositories;
using GLSoft.DoubleEntryHomeAccounting.Common.Exceptions;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Params;
using GLSoft.DoubleEntryHomeAccounting.Common.Services;
using GLSoft.DoubleEntryHomeAccounting.Common.Utils.Check;
using GLSoft.DoubleEntryHomeAccounting.Common.Utils.Ordering;

namespace GLSoft.DoubleEntryHomeAccounting.Business.Services;

public class TemplateService : ITemplateService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public TemplateService(IUnitOfWorkFactory unitOfWorkFactory) => _unitOfWorkFactory = unitOfWorkFactory;

    public async Task<Guid> Add(TemplateParam param)
    {
        CheckInputTemplateParam(param);

        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ITemplateRepository templateRepository = unitOfWork.GetRepository<ITemplateRepository>();
        ITemplateGroupRepository templateGroupRepository = unitOfWork.GetRepository<ITemplateGroupRepository>();
        IAccountRepository accountRepository = unitOfWork.GetRepository<IAccountRepository>();

        TemplateGroup group = await Guard.CheckAndGetEntityById(templateGroupRepository.GetById, param.GroupId);
        await Guard.CheckElementWithSameName(templateRepository, group.Id, Guid.Empty, param.Name);
        
        Template addedEntity = new Template
        {
            Id = Guid.NewGuid(),
            Name = param.Name,
            Description = param.Description,
            IsFavorite = param.IsFavorite,
            Order = await templateRepository.GetMaxOrderInGroup(group.Id) + 1,
            Group = group,
            GroupId = param.GroupId
        };
        
        List<TemplateEntry> entries = await CreateEntries(accountRepository, param, addedEntity);
        addedEntity.Entries.AddRange(entries);
        
        group.Elements.Add(addedEntity);

        await templateRepository.Add(addedEntity);

        await unitOfWork.SaveChanges();

        return addedEntity.Id;
    }

    public async Task Update(Guid entityId, TemplateParam param)
    {
        CheckInputTemplateParam(param);
        
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ITemplateRepository templateRepository = unitOfWork.GetRepository<ITemplateRepository>();
        ITemplateGroupRepository templateGroupRepository = unitOfWork.GetRepository<ITemplateGroupRepository>();
        IAccountRepository accountRepository = unitOfWork.GetRepository<IAccountRepository>();

        Template updatedEntity = await Guard.CheckAndGetEntityById(templateRepository.GetTemplateById, entityId);

        TemplateGroup group = await Guard.CheckAndGetEntityById(templateGroupRepository.GetById, param.GroupId);
        
        Guard.CheckEntityWithSameName(group.Elements, updatedEntity.Id, param.Name);

        List<TemplateEntry> oldEntries = updatedEntity.Entries;
        List<TemplateEntry> newEntries = await CreateEntries(accountRepository, param, updatedEntity);

        updatedEntity.Name = param.Name;
        updatedEntity.Description = param.Description;
        updatedEntity.IsFavorite = param.IsFavorite;
        
        updatedEntity.Entries.Clear();
        updatedEntity.Entries.AddRange(newEntries);
        
        oldEntries.ForEach(e => e.Template = null);

        await templateRepository.Update(updatedEntity);

        await unitOfWork.SaveChanges();
    }

    public async Task Delete(Guid entityId)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ITemplateRepository templateRepository = unitOfWork.GetRepository<ITemplateRepository>();

        Template deletedEntity = await Guard.CheckAndGetEntityById(templateRepository.GetTemplateById, entityId);

        TemplateGroup group = await templateRepository.GetGroupWithElementsByGroupId(deletedEntity.GroupId);

        deletedEntity.Group = default;
        deletedEntity.GroupId = default;
        group.Elements.Remove(deletedEntity);
        group.Elements.Reorder();

        await templateRepository.Delete(deletedEntity.Id);
        await templateRepository.Update(group.Elements);

        await unitOfWork.SaveChanges();
    }

    public async Task SetOrder(Guid entityId, int order)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ITemplateRepository templateRepository = unitOfWork.GetRepository<ITemplateRepository>();

        Template template = await Guard.CheckAndGetEntityById(templateRepository.GetById, entityId);
        if (template.Order == order)
        {
            return;
        }

        TemplateGroup group = await templateRepository.GetGroupWithElementsByGroupId(template.GroupId);
        
        group.Elements.SetOrder(template, order);

        await templateRepository.Update(group.Elements);

        await unitOfWork.SaveChanges();
    }

    public async Task SetFavoriteStatus(Guid entityId, bool isFavorite)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ITemplateRepository templateRepository = unitOfWork.GetRepository<ITemplateRepository>();

        Template template = await Guard.CheckAndGetEntityById(templateRepository.GetById, entityId);
        if (template.IsFavorite == isFavorite)
        {
            return;
        }

        template.IsFavorite = isFavorite;

        await templateRepository.Update(template);

        await unitOfWork.SaveChanges();
    }

    public async Task MoveToAnotherGroup(Guid entityId, Guid toGroupId)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ITemplateRepository templateRepository = unitOfWork.GetRepository<ITemplateRepository>();

        Template entity = await Guard.CheckAndGetEntityById(templateRepository.GetById, entityId);
        TemplateGroup fromGroup = await templateRepository.GetGroupWithElementsByGroupId(entity.Group.Id);
        TemplateGroup toGroup = await templateRepository.GetGroupWithElementsByGroupId(toGroupId);

        if (fromGroup.Id == toGroup.Id)
        {
            return;
        }

        await Guard.CheckElementWithSameName(templateRepository, toGroup.Id, entity.Id, entity.Name);

        entity.Group = toGroup;
        entity.GroupId = toGroup.Id;
        entity.Order = await templateRepository.GetMaxOrderInGroup(toGroup.Id) + 1;
        toGroup.Elements.Add(entity);

        fromGroup.Elements.Remove(entity);
        fromGroup.Elements.Reorder();

        await templateRepository.Update(toGroup.Elements);
        await templateRepository.Update(fromGroup.Elements);

        await unitOfWork.SaveChanges();
    }

    public Task CombineElements(Guid toElementId, Guid fromElementId)
    {
        throw new NotSupportedException($"{nameof(CombineElements)} doesn't support for Templates");
    }

    private async Task<List<TemplateEntry>> CreateEntries(IAccountRepository accountRepository, TemplateParam param, Template template)
    {
        List<TemplateEntry> entries = new List<TemplateEntry>();
        foreach (TemplateEntryParam entry in param.Entries)
        {
            Account account = await Guard.CheckAndGetEntityById(accountRepository.GetById, entry.AccountId);
            TemplateEntry templateEntry = new TemplateEntry
            {
                Id = Guid.NewGuid(),
                Account = account,
                AccountId = account.Id,
                Amount = entry.Amount,
                Template = template,
                TemplateId = template.Id,
            };
            entries.Add(templateEntry);
        }

        return entries;
    }

    private static void CheckInputTemplateParam(TemplateParam param)
    {
        Guard.CheckParamForNull(param);

        Guard.CheckParamNameForNullOrEmpty(param);

        if (param.Entries == null || param.Entries.Count < 2)
        {
            throw new EntriesAmountException();
        }
    }
}