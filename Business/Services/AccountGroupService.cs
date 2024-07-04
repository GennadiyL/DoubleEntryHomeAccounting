﻿using Business.Services.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.Infrastructure.Peaa;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Params;
using GLSoft.DoubleEntryHomeAccounting.Common.Services;

namespace Business.Services;

public class AccountGroupService 
    : ReferenceDataGroupService<AccountGroup, Account, GroupParam>, IAccountGroupService
{
    public AccountGroupService(
        IUnitOfWorkFactory unitOfWorkFactory,
        IAccountGroupRepository groupRepository,
        IAccountRepository elementRepository)
        : base(unitOfWorkFactory, groupRepository, elementRepository)
    {
    }
}