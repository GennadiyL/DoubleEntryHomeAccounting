using GLSoft.DoubleEntryHomeAccounting.Common.Params.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Params;

public class TemplateParam : INamedParam, IFavoriteParam, IElementParam
{
    public Guid GroupId { get; set; }
    public bool IsFavorite { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<TemplateEntryParam> Entries { get; } = new();
}