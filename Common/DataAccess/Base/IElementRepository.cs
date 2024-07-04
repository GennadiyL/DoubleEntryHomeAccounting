using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;

public interface IElementRepository<TGroup, TElement>
    where TGroup : IReferenceDataGroupEntity<TGroup, TElement>
    where TElement : IReferenceDataElementEntity<TGroup>
{
    Task<ICollection<TElement>> GetByName(string name);
    Task<TGroup> GetByGroupId(Guid groupId);
    Task<ICollection<TElement>> GetByGroupIdAndName(Guid groupId, string name);
    Task<int> GetMaxOrder(Guid groupId);
    Task<int> GetCount(Guid groupId);
}