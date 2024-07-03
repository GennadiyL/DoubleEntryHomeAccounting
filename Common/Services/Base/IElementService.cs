using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Services.Base;

public interface IElementService<TGroup, TElement>
    where TElement : IElementEntity<TGroup>
{
    Task MoveToAnotherGroup(Guid entityId, Guid parentId);
}