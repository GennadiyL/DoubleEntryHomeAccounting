using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;
using GLSoft.DoubleEntryHomeAccounting.Common.Params.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Services.Base;

public interface IReferenceDataService<T, in TParam> : IEntityService<T, TParam>, IOrderedService<T>, IFavoriteService<T>
    where T : class, IReferenceDataEntity
    where TParam : class, IParam
{
}