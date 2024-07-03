using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Utils.Check
{
    public static class Getter
    {
        public static async Task<T> GetEntityById<T>(Func<Guid, Task<T>> getter, Guid entityId) where T : IEntity
        {
            T item = await getter(entityId);
            if (item == null)
            {
                throw new ArgumentNullException($"Entity {typeof(T).Name} does not exist");
            }
            return item;
        }
    }
}
