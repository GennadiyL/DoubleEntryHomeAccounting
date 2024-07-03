namespace GLSoft.DoubleEntryHomeAccounting.Common.Infos.Interfaces;

public interface IGroupInfo
{
    Guid? ParentId { get; set; }
    List<Guid> ChildrenIds { get; }
    List<Guid> ElementsIds { get; }
}