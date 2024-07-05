using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Model;
using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Utils.Check
{
    public static class Getter
    {
        public static async Task<T> GetEntityById<T>(IEntityRepository<T> repo, Guid entityId) where T : IEntity
        {
            T item = await repo.GetById(entityId, null)
                ?? throw new ArgumentNullException($"Entity {typeof(T).Name} does not exist");
            return item;
        }

        [Obsolete]
        public static async Task<TElement> GetElementWithGroupById<TGroup, TElement>(IElementEntityRepository<TGroup, TElement> repo, Guid entityId)
            where TGroup : class, IReferenceDataGroupEntity<TGroup, TElement>
            where TElement : class, IReferenceDataElementEntity<TGroup, TElement>
        {
            TElement item = await repo.GetById(entityId, Include<TElement>.Create(e => e.Group))
                ?? throw new ArgumentNullException($"Entity {typeof(TElement).Name} does not exist");
            return item;
        }
    }
}
