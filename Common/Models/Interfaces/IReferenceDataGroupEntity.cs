namespace GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

public interface IReferenceDataGroupEntity<TGroup, TElement> : IReferenceDataEntity, IGroupEntity<TGroup, TElement>
    where TGroup : IGroupEntity<TGroup, TElement>
{
}