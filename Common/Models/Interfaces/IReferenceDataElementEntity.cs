using System.Xml.Linq;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

public interface IReferenceDataElementEntity<TGroup, TElement> : IReferenceDataEntity, IElementEntity<TGroup, TElement>
    where TGroup : class, IGroupEntity<TGroup, TElement>
    where TElement : class, IElementEntity<TGroup, TElement>
{
}