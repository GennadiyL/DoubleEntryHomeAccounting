using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Model;
using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Utils.Check
{
    public static class Getter
    {
        public static async Task<T> GetEntityById<T>(IEntityRepository<T> repo, Guid entityId) where T : IEntity
        {
            T item = await repo.GetById(entityId, null);
            if (item == null)
            {
                throw new ArgumentNullException($"Entity {typeof(T).Name} does not exist");
            }
            return item;
        }

        public static async Task<TElement> GetElementById<TGroup, TElement>(IElementEntityRepository<TGroup, TElement> repo, Guid entityId)
            where TGroup : class, IReferenceDataGroupEntity<TGroup, TElement>
            where TElement : class, IReferenceDataElementEntity<TGroup, TElement>
        {
            TElement item = await repo.GetById(entityId, Include<TElement>.Create(e => e.Group));
            if (item == null)
            {
                throw new ArgumentNullException($"Entity {typeof(TElement).Name} does not exist");
            }
            return item;
        }

        public static async Task<TGroup> GetGroupById<TGroup, TElement>(IGroupEntityRepository<TGroup, TElement> repo, Guid entityId)
            where TGroup : class, IReferenceDataGroupEntity<TGroup, TElement>
            where TElement : class, IReferenceDataElementEntity<TGroup, TElement>
        {
            TGroup item = await repo.GetById(entityId, Include<TGroup>.Create(e => e.Parent));
            if (item == null)
            {
                throw new ArgumentNullException($"Entity {typeof(TGroup).Name} does not exist");
            }
            return item;
        }

    }
}
