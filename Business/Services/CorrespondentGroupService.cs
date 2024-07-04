﻿using Business.Services.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.Infrastructure.Peaa;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Params;
using GLSoft.DoubleEntryHomeAccounting.Common.Services;

namespace Business.Services;

public class CorrespondentGroupService 
    : ReferenceDataGroupService<CorrespondentGroup, Correspondent, GroupParam>, ICorrespondentGroupService
{
    public CorrespondentGroupService(
        IUnitOfWorkFactory unitOfWorkFactory,
        ICorrespondentGroupRepository groupRepository,
        ICorrespondentRepository elementRepository)
        : base(unitOfWorkFactory,groupRepository, elementRepository )
    {
    }
}