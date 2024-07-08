using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Models.Base;

public abstract class ReferenceDataEntity : Entity, IReferenceDataEntity
{
    public DateTime Original { get; set; }
    public DateTime Current { get; set; }
    public bool IsDeleted { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Order { get; set; }
    public bool IsFavorite { get; set; }
}