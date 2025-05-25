using GLSoft.DoubleEntryHomeAccounting.Common.Params.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Services.Base;

public interface IEntityService<in TParam>
    where TParam : IParam
{
    Task<Guid> Add(TParam param);
    Task Update(Guid entityId, TParam param);
    Task Delete(Guid entityId);
}