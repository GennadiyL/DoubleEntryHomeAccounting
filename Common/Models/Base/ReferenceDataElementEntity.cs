using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Models.Base;

public abstract class ReferenceDataElementEntity<TGroup, TElement> : ReferenceDataEntity, IReferenceDataElementEntity<TGroup, TElement>
    where TGroup : class, IGroupEntity<TGroup, TElement>
    where TElement : class, IElementEntity<TGroup, TElement>
{
    public Guid GroupId { get; set; }
    public TGroup Group { get; set; }
}