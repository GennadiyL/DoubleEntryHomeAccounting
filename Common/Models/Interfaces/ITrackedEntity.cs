namespace GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

public interface ITrackedEntity
{
    DateTime Original { get; set; }
    DateTime Current { get; set; }
    bool IsDeleted { get; set; }
}