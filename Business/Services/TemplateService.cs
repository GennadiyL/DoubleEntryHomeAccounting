using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.Infrastructure.Peaa;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Params;
using GLSoft.DoubleEntryHomeAccounting.Common.Services;
using GLSoft.DoubleEntryHomeAccounting.Common.Utils.Check;
using GLSoft.DoubleEntryHomeAccounting.Common.Utils.Ordering;

namespace Business.Services;

public class TemplateService : ITemplateService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ITemplateRepository _templateRepository;
    private readonly ITemplateGroupRepository _templateGroupRepository;
    private readonly IAccountRepository _accountRepository;

    public TemplateService(
        IUnitOfWorkFactory unitOfWorkFactory, 
        ITemplateRepository templateRepository, 
        ITemplateGroupRepository templateGroupRepository, 
        IAccountRepository accountRepository)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _templateRepository = templateRepository;
        _templateGroupRepository = templateGroupRepository;
        _accountRepository = accountRepository;
    }

    public async Task<Guid> Add(TemplateParam param)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Guard.CheckParamForNull(param);
        Guard.CheckParamNameForNull(param);
        CheckEntries(param);

        List<TemplateEntry> entries = await CreateEntries(param);

        TemplateGroup group = await Getter.GetEntityById(_templateGroupRepository, param.GroupId);
        await Guard.CheckElementWithSameName(_templateRepository, group.Id, Guid.Empty, param.Name);

        Template addedEntity = new Template
        {
            Name = param.Name,
            Description = param.Description,
            IsFavorite = param.IsFavorite,
            Order = await _templateRepository.GetMaxOrder(group.Id) + 1
        };

        addedEntity.Entries.AddRange(entries);

        group.Elements.Add(addedEntity);
        addedEntity.Group = group;
        await _templateRepository.Add(addedEntity);

        await unitOfWork.SaveChanges();

        return addedEntity.Id;
    }

    public async Task Update(Guid entityId, TemplateParam param)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Guard.CheckParamForNull(param);
        Guard.CheckParamNameForNull(param);
        CheckEntries(param);

        List<TemplateEntry> entries = await CreateEntries(param);

        var updatedEntity = await Getter.GetEntityById(_templateRepository, entityId);
        await Guard.CheckElementWithSameName(_templateRepository, updatedEntity.Group.Id, entityId, param.Name);

        updatedEntity.Name = param.Name;
        updatedEntity.Description = param.Description;
        updatedEntity.IsFavorite = param.IsFavorite;
        updatedEntity.Entries.Clear();
        updatedEntity.Entries.AddRange(entries);

        await _templateRepository.Update(updatedEntity);

        await unitOfWork.SaveChanges();
    }

    public async Task Delete(Guid entityId)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Template deletedEntity = await Getter.GetElementById(_templateRepository, entityId);
        TemplateGroup group = await _templateRepository.GetByGroupId(deletedEntity.Group.Id);

        group.Elements.Remove(deletedEntity);
        await _templateRepository.Delete(deletedEntity.Id);

        OrderingUtils.Reorder(group.Elements);
        await _templateRepository.Update(group.Elements);

        await unitOfWork.SaveChanges();
    }

    public async Task SetOrder(Guid entityId, int order)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Template template = await Getter.GetElementById(_templateRepository, entityId);
        if (template.Order != order)
        {
            TemplateGroup group = await _templateRepository.GetByGroupId(template.Group.Id);
            OrderingUtils.SetOrder(group.Elements, template, order);
            await _templateRepository.Update(group.Elements);
        }

        await unitOfWork.SaveChanges();
    }

    public async Task SetFavoriteStatus(Guid entityId, bool isFavorite)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Template template = await Getter.GetEntityById(_templateRepository, entityId);
        if (template.IsFavorite != isFavorite)
        {
            template.IsFavorite = isFavorite;
            await _templateRepository.Update(template);
        }

        await unitOfWork.SaveChanges();
    }

    public async Task MoveToAnotherGroup(Guid entityId, Guid groupId)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Template entity = await Getter.GetElementById(_templateRepository, entityId);
        TemplateGroup fromGroup = await _templateRepository.GetByGroupId(entity.Group.Id);
        TemplateGroup toGroup = await _templateRepository.GetByGroupId(groupId);

        if (fromGroup.Id != toGroup.Id)
        {
            await Guard.CheckElementWithSameName(_templateRepository, toGroup.Id, entity.Id, entity.Name);
            int newOrder = await _templateRepository.GetMaxOrder(toGroup.Id) + 1;

            entity.Group = toGroup;
            toGroup.Elements.Add(entity);
            fromGroup.Elements.Remove(entity);

            await _templateRepository.Update(toGroup.Elements);
            await _templateRepository.Update(fromGroup.Elements);
            await _templateGroupRepository.Update(toGroup);
            await _templateGroupRepository.Update(fromGroup);
        }

        await unitOfWork.SaveChanges();
    }

    public Task CombineElements(Guid primaryId, Guid secondaryId)
    {
        throw new NotSupportedException($"{nameof(CombineElements)} doesn't support for Templates");
    }

    private async Task<List<TemplateEntry>> CreateEntries(TemplateParam param)
    {
        List<TemplateEntry> entries = new List<TemplateEntry>();
        foreach (var entry in param.Entries)
        {
            TemplateEntry templateEntry = new TemplateEntry
            {
                Id = Guid.NewGuid(),
                Account = await Getter.GetEntityById(_accountRepository, entry.AccountId),
                Amount = entry.Amount
            };
            entries.Add(templateEntry);
        }

        return entries;
    }

    private static void CheckEntries(TemplateParam param)
    {
        if (param.Entries == null || param.Entries.Count < 2)
        {
            throw new ArgumentException("Invalid amount of Transaction Entries: amount must be more than 1");
        }
    }

}