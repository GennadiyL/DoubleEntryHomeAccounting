using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Models.Base;

public abstract class Entity : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public override string ToString()
    {
        return $"{GetType().Name} {Id:N}";
    }
}