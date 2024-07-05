using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;

public interface IElementRepository<TGroup, TElement>
    where TGroup : class, IReferenceDataGroupEntity<TGroup, TElement>
    where TElement : class, IReferenceDataElementEntity<TGroup, TElement>
{
    Task<ICollection<TElement>> GetByName(string name);
    Task<TGroup> GetByGroupId(Guid groupId);
    [Obsolete]
    Task<TGroup> GetByGroupIdAndName(Guid groupId, string name);
    Task<int> GetMaxOrder(Guid groupId);
    Task<int> GetCount(Guid groupId);
}