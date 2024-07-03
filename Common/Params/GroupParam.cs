using GLSoft.DoubleEntryHomeAccounting.Common.Params.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Params;

public class GroupParam : INamedParam, IFavoriteParam, IGroupParam
{
    public Guid? ParentId { get; set; }
    public bool IsFavorite { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}