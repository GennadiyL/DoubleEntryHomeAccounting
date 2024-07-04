namespace GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

public interface IReferenceDataGroupEntity<TGroup, TElement> : IReferenceDataEntity, IGroupEntity<TGroup, TElement>
    where TGroup : class, IReferenceDataGroupEntity<TGroup, TElement>
    where TElement : class, IReferenceDataElementEntity<TGroup, TElement>
{
}