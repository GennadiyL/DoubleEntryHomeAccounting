namespace GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

public interface ITrackedEntity : IEntity
{
    string TimeStamp { get; set; }
}