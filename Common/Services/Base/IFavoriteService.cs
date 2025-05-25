using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Services.Base;

// ReSharper disable once UnusedTypeParameter
public interface IFavoriteService<T>
    where T : IFavoriteEntity
{
    Task SetFavoriteStatus(Guid entityId, bool isFavorite);
}