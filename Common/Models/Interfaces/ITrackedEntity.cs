namespace GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

public interface ITrackedEntity : IEntity
{
    DateTime Original { get; set; }
    DateTime Current { get; set; }
    bool IsDeleted { get; set; }
}