using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;
using GLSoft.DoubleEntryHomeAccounting.Common.Params.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Services.Base;

public interface IReferenceDataService<T, in TParam> : IEntityService<T, TParam>, IOrderedEntityService<T>, IFavoriteService<T>
    where T : class, IEntity, IFavoriteEntity, IOrderedEntity
    where TParam : class, IParam
{
}