﻿using GLSoft.DoubleEntryHomeAccounting.Business.Services.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Params;
using GLSoft.DoubleEntryHomeAccounting.Common.Services;

namespace GLSoft.DoubleEntryHomeAccounting.Business.Services;

public class TemplateGroupService
    : GroupService<TemplateGroup, Template, GroupParam>, ITemplateGroupService
{
    public TemplateGroupService(IUnitOfWorkFactory unitOfWorkFactory) : base(unitOfWorkFactory)
    {
    }
}