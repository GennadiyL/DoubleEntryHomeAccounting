namespace GLSoft.DoubleEntryHomeAccounting.Common.Infos.Interfaces;

public interface IGroupInfo : IReferenceInfo
{
    Guid ParentId { get; set; }
    List<Guid> ChildrenIds { get; }
    List<Guid> ElementsIds { get; }
}