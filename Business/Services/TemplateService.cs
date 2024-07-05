using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
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

        CheckInputTemplateParam(param);

        Template addedEntity = new Template();
        List<TemplateEntry> entries = await CreateEntries(param, addedEntity);

        TemplateGroup group = await Getter.GetEntityById(_templateGroupRepository, param.GroupId);
        await Guard.CheckElementWithSameName(_templateRepository, group.Id, Guid.Empty, param.Name);
        
        addedEntity.Name = param.Name;
        addedEntity.Description = param.Description;
        addedEntity.IsFavorite = param.IsFavorite;
        addedEntity.Order = await _templateRepository.GetMaxOrder(group.Id) + 1;
        addedEntity.Entries.AddRange(entries);
        addedEntity.Group = group;
        group.Elements.Add(addedEntity);

        await _templateRepository.Add(addedEntity);

        await unitOfWork.SaveChanges();

        return addedEntity.Id;
    }

    public async Task Update(Guid entityId, TemplateParam param)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        CheckInputTemplateParam(param);

        Template updatedEntity = await _templateRepository.GetTemplateById(entityId)
                                 ?? throw new ArgumentNullException($"Template #{entityId} does not exist"); ;
        await Guard.CheckElementWithSameName(_templateRepository, updatedEntity.GroupId, entityId, param.Name);

        List<TemplateEntry> oldEntries = updatedEntity.Entries;
        List<TemplateEntry> newEntries = await CreateEntries(param, updatedEntity);

        updatedEntity.Name = param.Name;
        updatedEntity.Description = param.Description;
        updatedEntity.IsFavorite = param.IsFavorite;
        updatedEntity.Entries.Clear();
        updatedEntity.Entries.AddRange(newEntries);
        oldEntries.ForEach(e => e.Template = null);

        await _templateRepository.Update(updatedEntity);

        await unitOfWork.SaveChanges();
    }

    public async Task Delete(Guid entityId)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Template deletedEntity = await _templateRepository.GetTemplateById(entityId)
                                 ?? throw new ArgumentNullException($"Template #{entityId} does not exist");

        TemplateGroup group = await _templateRepository.GetByGroupId(deletedEntity.GroupId);

        group.Elements.Remove(deletedEntity);
        OrderingUtils.Reorder(group.Elements);

        await _templateRepository.Delete(deletedEntity.Id);
        await _templateRepository.Update(group.Elements);

        await unitOfWork.SaveChanges();
    }

    public async Task SetOrder(Guid entityId, int order)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Template template = await Getter.GetEntityById(_templateRepository, entityId);
        if (template.Order == order)
        {
            return;
        }

        TemplateGroup group = await _templateRepository.GetByGroupId(template.GroupId);
        OrderingUtils.SetOrder(group.Elements, template, order);

        await _templateRepository.Update(group.Elements);

        await unitOfWork.SaveChanges();
    }

    public async Task SetFavoriteStatus(Guid entityId, bool isFavorite)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Template template = await Getter.GetEntityById(_templateRepository, entityId);
        if (template.IsFavorite == isFavorite)
        {
            return;
        }

        template.IsFavorite = isFavorite;

        await _templateRepository.Update(template);

        await unitOfWork.SaveChanges();
    }

    public async Task MoveToAnotherGroup(Guid entityId, Guid groupId)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Template entity = await Getter.GetEntityById(_templateRepository, entityId);
        TemplateGroup fromGroup = await _templateRepository.GetByGroupId(entity.Group.Id);
        TemplateGroup toGroup = await _templateRepository.GetByGroupId(groupId);

        if (fromGroup.Id == toGroup.Id)
        {
            return;
        }

        await Guard.CheckElementWithSameName(_templateRepository, toGroup.Id, entity.Id, entity.Name);
        int newOrder = await _templateRepository.GetMaxOrder(toGroup.Id) + 1;

        fromGroup.Elements.Remove(entity);
        entity.Group = toGroup;
        toGroup.Elements.Add(entity);

        OrderingUtils.Reorder(fromGroup.Elements);
        OrderingUtils.SetOrder(toGroup.Elements, entity, newOrder);

        await _templateRepository.Update(toGroup.Elements);
        await _templateRepository.Update(fromGroup.Elements);

        await unitOfWork.SaveChanges();
    }

    public Task CombineElements(Guid primaryId, Guid secondaryId)
    {
        throw new NotSupportedException($"{nameof(CombineElements)} doesn't support for Templates");
    }

    private async Task<List<TemplateEntry>> CreateEntries(TemplateParam param, Template template)
    {
        List<TemplateEntry> entries = new List<TemplateEntry>();
        foreach (var entry in param.Entries)
        {
            TemplateEntry templateEntry = new TemplateEntry
            {
                Id = Guid.NewGuid(),
                Account = await Getter.GetEntityById(_accountRepository, entry.AccountId),
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
            throw new ArgumentException("Invalid amount of Transaction Entries: amount must be more than 1");
        }
    }
}