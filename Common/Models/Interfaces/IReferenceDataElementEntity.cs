using System.Xml.Linq;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

public interface IReferenceDataElementEntity<TGroup, TElement> : IReferenceDataEntity, IElementEntity<TGroup, TElement>
    where TGroup : class, IReferenceDataGroupEntity<TGroup, TElement>
    where TElement : class, IReferenceDataElementEntity<TGroup, TElement>
{
}