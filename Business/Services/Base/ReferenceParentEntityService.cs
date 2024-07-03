using Common.DataAccess.Base;
using Common.Infrastructure.Peaa;
using Common.Models.Interfaces;
using Common.Params.Interfaces;
using Common.Services.Base;
using Common.Utils.Check;
using Common.Utils.Ordering;

namespace Business.Services.Base;

public abstract class ReferenceParentEntityService<TParent, TChild, TParam> : IReferenceEntityService<TParent, TParam>
    where TParent : IReferenceParentEntity<TChild>, new()
    where TChild : IReferenceChildEntity<TParent>
    where TParam: INamedParam, IFavoriteParam
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    protected IUnitOfWorkFactory UnitOfWorkFactory => _unitOfWorkFactory;

    protected ReferenceParentEntityService(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task<Guid> Add(TParam param)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Guard.CheckParamForNull(param);
        Guard.CheckParamNameForNull(param);

        IParentEntityRepository<TParent> entityRepository = await unitOfWork.GetRepository<IParentEntityRepository<TParent>>();
        await Guard.CheckEntityWithSameName(entityRepository, Guid.Empty, param.Name);

        TParent addedEntity = new TParent
        {
            Name = param.Name,
            Description = param.Description,
            IsFavorite = param.IsFavorite,
            Order = await entityRepository.GetMaxOrder() + 1
        };

        await entityRepository.Add(addedEntity);

        await unitOfWork.SaveChanges();

        return addedEntity.Id;
    }

    public async Task Update(Guid entityId, TParam param)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Guard.CheckParamForNull(param);
        Guard.CheckParamNameForNull(param);

        IParentEntityRepository<TParent> entityRepository = await unitOfWork.GetRepository<IParentEntityRepository<TParent>>();
        await Guard.CheckEntityWithSameName(entityRepository, entityId, param.Name);

        TParent updatedEntity = await Getter.GetEntityById(entityRepository.Get, entityId);
        updatedEntity.Name = param.Name;
        updatedEntity.Description = param.Description;
        updatedEntity.IsFavorite = param.IsFavorite;

        await entityRepository.Update(updatedEntity);

        await unitOfWork.SaveChanges();
    }

    public async Task Delete(Guid entityId)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IParentEntityRepository<TParent> entityRepository =  await unitOfWork.GetRepository<IParentEntityRepository<TParent>>();
        TParent deletedEntity = await Getter.GetEntityById(entityRepository.Get, entityId);

        IChildEntityRepository<TChild> childEntityRepository = await unitOfWork.GetRepository< IChildEntityRepository<TChild>>();
        await Guard.CheckExistedChildrenInTheGroup(childEntityRepository, entityId);

        await entityRepository.Delete(deletedEntity);

        List<TParent> list = await entityRepository.GetList();
        OrderingUtils.Reorder(list);
        await entityRepository.UpdateList(list);

        await unitOfWork.SaveChanges();
    }

    public async Task SetFavoriteStatus(Guid entityId, bool isFavorite)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IParentEntityRepository<TParent> entityRepository = await unitOfWork.GetRepository<IParentEntityRepository<TParent>>();

        TParent entity = await Getter.GetEntityById(entityRepository.Get, entityId);
        if (entity.IsFavorite != isFavorite)
        {
            entity.IsFavorite = isFavorite;
            await entityRepository.Update(entity);
        }

        await unitOfWork.SaveChanges();
    }

    public async Task SetOrder(Guid entityId, int order)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IParentEntityRepository<TParent> entityRepository = await unitOfWork.GetRepository<IParentEntityRepository<TParent>>();

        TParent entity = await Getter.GetEntityById(entityRepository.Get, entityId);
        if (entity.Order != order)
        {
            List<TParent> list = await entityRepository.GetList();
            OrderingUtils.SetOrder(list, entity, order);
            await entityRepository.UpdateList(list);
        }

        await unitOfWork.SaveChanges();
    }
}