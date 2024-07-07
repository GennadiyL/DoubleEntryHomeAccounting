using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.Exceptions;
using GLSoft.DoubleEntryHomeAccounting.Common.Infrastructure.Peaa;
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
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ITemplateRepository templateRepository = unitOfWork.GetRepository<ITemplateRepository>();
        ITemplateGroupRepository templateGroupRepository = unitOfWork.GetRepository<ITemplateGroupRepository>();
        IAccountRepository accountRepository = unitOfWork.GetRepository<IAccountRepository>();

        CheckInputTemplateParam(param);

        Template addedEntity = new Template();
        List<TemplateEntry> entries = await CreateEntries(accountRepository, param, addedEntity);

        TemplateGroup group = await Getter.GetEntityById(g => templateGroupRepository.GetById(g), param.GroupId);
        await Guard.CheckElementWithSameName(templateRepository, group.Id, Guid.Empty, param.Name);
        
        addedEntity.Name = param.Name;
        addedEntity.Description = param.Description;
        addedEntity.IsFavorite = param.IsFavorite;
        addedEntity.Order = await templateRepository.GetMaxOrder(group.Id) + 1;
        addedEntity.Entries.AddRange(entries);
        addedEntity.Group = group;
        group.Elements.Add(addedEntity);

        await templateRepository.Add(addedEntity);

        await unitOfWork.SaveChanges();

        return addedEntity.Id;
    }

    public async Task Update(Guid entityId, TemplateParam param)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ITemplateRepository templateRepository = unitOfWork.GetRepository<ITemplateRepository>();
        IAccountRepository accountRepository = unitOfWork.GetRepository<IAccountRepository>();

        CheckInputTemplateParam(param);

        Template updatedEntity = await Getter.GetEntityById(templateRepository.GetTemplateById, entityId);
        await Guard.CheckElementWithSameName(templateRepository, updatedEntity.GroupId, entityId, param.Name);

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
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ITemplateRepository templateRepository = unitOfWork.GetRepository<ITemplateRepository>();

        Template deletedEntity = await Getter.GetEntityById(templateRepository.GetTemplateById, entityId); 

        TemplateGroup group = await templateRepository.GetByGroupId(deletedEntity.GroupId);

        group.Elements.Remove(deletedEntity);
        OrderingUtils.Reorder(group.Elements);

        await templateRepository.Delete(deletedEntity.Id);
        await templateRepository.Update(group.Elements);

        await unitOfWork.SaveChanges();
    }

    public async Task SetOrder(Guid entityId, int order)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ITemplateRepository templateRepository = unitOfWork.GetRepository<ITemplateRepository>();

        Template template = await Getter.GetEntityById(g => templateRepository.GetById(g), entityId);
        if (template.Order == order)
        {
            return;
        }

        TemplateGroup group = await templateRepository.GetByGroupId(template.GroupId);
        OrderingUtils.SetOrder(group.Elements, template, order);

        await templateRepository.Update(group.Elements);

        await unitOfWork.SaveChanges();
    }

    public async Task SetFavoriteStatus(Guid entityId, bool isFavorite)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ITemplateRepository templateRepository = unitOfWork.GetRepository<ITemplateRepository>();

        Template template = await Getter.GetEntityById(g => templateRepository.GetById(g), entityId);
        if (template.IsFavorite == isFavorite)
        {
            return;
        }

        template.IsFavorite = isFavorite;

        await templateRepository.Update(template);

        await unitOfWork.SaveChanges();
    }

    public async Task MoveToAnotherGroup(Guid entityId, Guid groupId)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ITemplateRepository templateRepository = unitOfWork.GetRepository<ITemplateRepository>();

        Template entity = await Getter.GetEntityById(g => templateRepository.GetById(g), entityId);
        TemplateGroup fromGroup = await templateRepository.GetByGroupId(entity.Group.Id);
        TemplateGroup toGroup = await templateRepository.GetByGroupId(groupId);

        if (fromGroup.Id == toGroup.Id)
        {
            return;
        }

        await Guard.CheckElementWithSameName(templateRepository, toGroup.Id, entity.Id, entity.Name);
        int newOrder = await templateRepository.GetMaxOrder(toGroup.Id) + 1;

        fromGroup.Elements.Remove(entity);
        entity.Group = toGroup;
        toGroup.Elements.Add(entity);

        OrderingUtils.Reorder(fromGroup.Elements);
        OrderingUtils.SetOrder(toGroup.Elements, entity, newOrder);

        await templateRepository.Update(toGroup.Elements);
        await templateRepository.Update(fromGroup.Elements);

        await unitOfWork.SaveChanges();
    }

    public Task CombineElements(Guid primaryId, Guid secondaryId)
    {
        throw new NotSupportedException($"{nameof(CombineElements)} doesn't support for Templates");
    }

    private async Task<List<TemplateEntry>> CreateEntries(IAccountRepository accountRepository, TemplateParam param, Template template)
    {
        List<TemplateEntry> entries = new List<TemplateEntry>();
        foreach (TemplateEntryParam entry in param.Entries)
        {
            TemplateEntry templateEntry = new TemplateEntry
            {
                Id = Guid.NewGuid(),
                Account = await Getter.GetEntityById(g => accountRepository.GetById(g), entry.AccountId),
                Amount = entry.Amount,
                Template = template,
            };
            entries.Add(templateEntry);
        }

        return entries;
    }

    private static void CheckInputTemplateParam(TemplateParam param)
    {
        Guard.CheckParamForNull(param);

        Guard.CheckParamNameForNull(param);

        if (param.Entries == null || param.Entries.Count < 2)
        {
            throw new EntriesAmountException();
        }
    }
}