namespace GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

public interface IElementEntity<TGroup>
{
    TGroup Group { get; set; }
}