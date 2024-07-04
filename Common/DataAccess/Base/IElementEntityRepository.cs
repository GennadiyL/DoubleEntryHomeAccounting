using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;

public interface IElementEntityRepository<TGroup, TElement> : IEntityRepository<TElement>, IElementRepository<TGroup, TElement>
    where TGroup : class, IReferenceDataGroupEntity<TGroup, TElement>
    where TElement : class, IReferenceDataElementEntity<TGroup, TElement>
{
}