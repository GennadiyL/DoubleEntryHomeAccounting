﻿using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;

public interface ICategoryGroupRepository : IGroupEntityRepository<CategoryGroup, Category>
{
}