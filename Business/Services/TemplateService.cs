using Common.DataAccess;
using Common.Infrastructure.Peaa;
using Common.Models;
using Common.Params;
using Common.Services;
using Common.Utils.Check;
using Common.Utils.Ordering;

namespace Business.Services;

public class TemplateService : ITemplateService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public TemplateService(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task<Guid> Add(TemplateParam param)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Guard.CheckParamForNull(param);
        Guard.CheckParamNameForNull(param);
        if (param.Entries == null || param.Entries.Count < 2)
        {
            throw new ArgumentException("Invalid amount of Transaction Entries: amount must be more than 1");
        }

        ITemplateRepository templateRepository = await unitOfWork.GetRepository<ITemplateRepository>();
        ITemplateGroupRepository templateGroupRepository = await unitOfWork.GetRepository<ITemplateGroupRepository>();
        IAccountRepository accountRepository = await unitOfWork.GetRepository<IAccountRepository>();

        List<TemplateEntry> entries = await CreateEntries(param, accountRepository);

        TemplateGroup parent = await Getter.GetEntityById(templateGroupRepository.Get, param.ParentId);
        await Guard.CheckEntityWithSameName(templateRepository, parent.Id, Guid.Empty, param.Name);

        var addedEntity = new Template
        {
            Name = param.Name,
            Description = param.Description,
            IsFavorite = param.IsFavorite,
            Order = await templateRepository.GetMaxOrder(parent.Id) + 1
        };

        addedEntity.Entries.AddRange(entries);

        parent.Children.Add(addedEntity);
        addedEntity.Parent = parent;
        await templateRepository.Add(addedEntity);

        await unitOfWork.SaveChanges();

        return addedEntity.Id;
    }

    public async Task Update(Guid entityId, TemplateParam param)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Guard.CheckParamForNull(param);
        Guard.CheckParamNameForNull(param);

        if (param.Entries == null || param.Entries.Count < 2)
        {
            throw new ArgumentException("Template entity is not correct: number of Entries must be more or equal 2");
        }

        ITemplateRepository templateRepository = await unitOfWork.GetRepository<ITemplateRepository>();
        IAccountRepository accountRepository = await unitOfWork.GetRepository<IAccountRepository>();

        List<TemplateEntry> entries = await CreateEntries(param, accountRepository);

        var updatedEntity = await Getter.GetEntityById(templateRepository.Get, entityId);
        await Guard.CheckEntityWithSameName(templateRepository, updatedEntity.Parent.Id, entityId, param.Name);

        updatedEntity.Name = param.Name;
        updatedEntity.Description = param.Description;
        updatedEntity.IsFavorite = param.IsFavorite;
        updatedEntity.Entries.Clear();
        updatedEntity.Entries.AddRange(entries);

        await templateRepository.Update(updatedEntity);

        await unitOfWork.SaveChanges();
    }

    public async Task Delete(Guid entityId)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ITemplateRepository templateRepository = await unitOfWork.GetRepository<ITemplateRepository>();
        ITemplateGroupRepository templateGroupRepository = await unitOfWork.GetRepository<ITemplateGroupRepository>();

        Template deletedEntity = await Getter.GetEntityById(templateRepository.Get, entityId);
        await templateRepository.LoadParent(deletedEntity);
        TemplateGroup parent = deletedEntity.Parent;
        await templateGroupRepository.LoadChildren(parent);

        parent.Children.Remove(deletedEntity);
        await templateRepository.Delete(deletedEntity);

        OrderingUtils.Reorder(parent.Children);
        await templateRepository.UpdateList(parent.Children);

        await unitOfWork.SaveChanges();
    }

    public async Task SetFavoriteStatus(Guid entityId, bool isFavorite)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ITemplateRepository templateRepository = await unitOfWork.GetRepository<ITemplateRepository>();

        Template template = await Getter.GetEntityById(templateRepository.Get, entityId);
        if (template.IsFavorite != isFavorite)
        {
            template.IsFavorite = isFavorite;
            await templateRepository.Update(template);
        }

        await unitOfWork.SaveChanges();
    }

    public async Task SetOrder(Guid entityId, int order)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ITemplateRepository templateRepository =  await unitOfWork.GetRepository<ITemplateRepository>();
        ITemplateGroupRepository templateGroupRepository = await unitOfWork.GetRepository<ITemplateGroupRepository>();

        Template template = await Getter.GetEntityById(templateRepository.Get, entityId);
        if (template.Order != order)
        {
            await templateRepository.LoadParent(template);
            TemplateGroup group = template.Parent;
            await templateGroupRepository.LoadChildren(group);
            OrderingUtils.SetOrder(group.Children, template, order);
            await templateRepository.UpdateList(group.Children);
        }

        await unitOfWork.SaveChanges();
    }

    public async Task MoveToAnotherParent(Guid entityId, Guid parentId)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ITemplateRepository templateRepository =  await unitOfWork.GetRepository<ITemplateRepository>();
        ITemplateGroupRepository templateGroupRepository = await unitOfWork.GetRepository<ITemplateGroupRepository>();

        Template entity = await Getter.GetEntityById(templateRepository.Get, entityId);
        await templateRepository.LoadParent(entity);
        TemplateGroup fromParent = entity.Parent;
        TemplateGroup toParent = await Getter.GetEntityById(templateGroupRepository.Get, parentId);

        if (fromParent.Id != toParent.Id)
        {
            await Guard.CheckEntityWithSameName(templateRepository, toParent.Id, entity.Id, entity.Name);
            await templateGroupRepository.LoadChildren(fromParent);

            toParent.Children.Add(entity);
            entity.Parent = toParent;
            entity.Order = await templateRepository.GetMaxOrder(fromParent.Id) + 1;

            fromParent.Children.Remove(entity);
            await templateRepository.Update(entity);
            OrderingUtils.Reorder(fromParent.Children);
            await templateRepository.UpdateList(fromParent.Children);
        }

        await unitOfWork.SaveChanges();
    }

    private async Task<List<TemplateEntry>> CreateEntries(TemplateParam param, IAccountRepository accountRepository)
    {
        List<TemplateEntry> entries = new List<TemplateEntry>();
        foreach (var entry in param.Entries)
        {
            TemplateEntry templateEntry = new TemplateEntry
            {
                Id = Guid.NewGuid(),
                Account = await Getter.GetEntityById(accountRepository.Get, entry.AccountId),
                Amount = entry.Amount
            };
            entries.Add(templateEntry);
        }

        return entries;
    }
}