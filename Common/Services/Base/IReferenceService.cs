using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;
using GLSoft.DoubleEntryHomeAccounting.Common.Params.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Services.Base;

public interface IReferenceService<T, in TParam> : IEntityService<T, TParam>, IOrderedService<T>, IFavoriteService<T>
    where T : class, IReferenceEntity
    where TParam : class, IParam
{
}