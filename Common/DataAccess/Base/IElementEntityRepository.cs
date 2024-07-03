using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;

public interface IElementEntityRepository<TGroup, TElement> : IEntityRepository<TElement>, IElementRepository<TGroup, TElement>
    where TGroup : IReferenceDataGroupEntity<TGroup, TElement>
    where TElement : IReferenceDataElementEntity<TGroup>
{
}