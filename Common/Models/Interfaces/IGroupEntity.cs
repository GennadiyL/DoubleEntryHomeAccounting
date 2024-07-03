namespace GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

public interface IGroupEntity<TGroup, TElement> 
    where TGroup : IGroupEntity<TGroup, TElement>
{
    TGroup Parent { get; set; }
    ICollection<TGroup> Children { get; }
    ICollection<TElement> Elements { get; }
}