namespace GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

public interface IGroupEntity<TGroup, TElement> 
    where TGroup : class, IReferenceDataGroupEntity<TGroup, TElement>
    where TElement : class, IReferenceDataElementEntity<TGroup, TElement>
{
    TGroup Parent { get; set; }
    ICollection<TGroup> Children { get; }
    ICollection<TElement> Elements { get; }
}