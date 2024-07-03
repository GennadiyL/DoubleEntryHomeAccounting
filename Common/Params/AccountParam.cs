﻿using GLSoft.DoubleEntryHomeAccounting.Common.Params.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Params;

public class AccountParam : INamedParam, IFavoriteParam, IElementParam
{
    public Guid CurrencyId { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? CorrespondentId { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid GroupId { get; set; }
    public bool IsFavorite { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}