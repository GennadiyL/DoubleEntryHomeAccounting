﻿using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;
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
    }
}