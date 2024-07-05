namespace GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

public interface IReferenceDataGroupEntity<TGroup, TElement> : IReferenceDataEntity, IGroupEntity<TGroup, TElement>
    where TGroup : class, IGroupEntity<TGroup, TElement>
    where TElement : class, IElementEntity<TGroup, TElement>
{
}