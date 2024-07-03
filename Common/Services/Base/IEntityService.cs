using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;
using GLSoft.DoubleEntryHomeAccounting.Common.Params.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Services.Base;

public interface IEntityService<T, in TParam> 
    where T : IEntity
    where TParam : IParam
{
    Task Add(TParam param);
    Task Update(Guid entityId, TParam param);
    Task Delete(Guid entityId);
}