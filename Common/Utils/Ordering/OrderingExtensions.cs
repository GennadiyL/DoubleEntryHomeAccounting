﻿using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Utils.Ordering;

public static class OrderingExtensions
{
    public static void Reorder<T>(this ICollection<T> entities) where T : class, IOrderedEntity
    {
        List<T> orderedItems = entities.OrderBy(i => i.Order).ToList();

        for (int i = 0; i < orderedItems.Count; i++)
        {
            orderedItems[i].Order = i + 1;
        }
    }

    public static void SetOrder<T>(this ICollection<T> entities, T orderedEntity, int order) where T : class, IOrderedEntity
    {
        if (orderedEntity.Order > order || orderedEntity.Order == 0)
        {
            foreach (T item in entities)
            {
                if (item.Order >= order)
                {
                    item.Order++;
                }
            }

            orderedEntity.Order = order;
            entities.Reorder();
        }
        else if (orderedEntity.Order < order)
        {
            foreach (T item in entities)
            {
                if (item.Order <= order && item.Order > orderedEntity.Order)
                {
                    item.Order--;
                }
            }

            orderedEntity.Order = order;
            entities.Reorder();
        }
    }

    public static int GetMaxOrder<T>(this ICollection<T> entities) where T : IOrderedEntity
    {
        return entities.Max(e => e.Order);
    }
}