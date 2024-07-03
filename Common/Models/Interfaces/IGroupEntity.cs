namespace GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

public interface IGroupEntity<TGroup, TElement> 
    where TGroup : IGroupEntity<TGroup, TElement>
{
    TGroup Parent { get; set; }
    IList<TGroup> Children { get; }
    IList<TElement> Elements { get; }
}