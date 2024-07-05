namespace GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

public interface IElementEntity<TGroup, TElement>
    where TGroup : class, IReferenceDataGroupEntity<TGroup, TElement>
    where TElement: class, IReferenceDataElementEntity<TGroup, TElement>
{
    Guid GroupId { get; set; }
    TGroup Group { get; set; }
}