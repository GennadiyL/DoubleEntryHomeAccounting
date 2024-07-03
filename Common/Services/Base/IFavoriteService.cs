using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Services.Base;

public interface IFavoriteService<T>
    where T : IFavoriteEntity
{
    Task SetFavoriteStatus(Guid entityId, bool isFavorite);
}