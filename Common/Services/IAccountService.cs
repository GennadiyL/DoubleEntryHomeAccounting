﻿using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Params;
using GLSoft.DoubleEntryHomeAccounting.Common.Services.Base;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Services;

public interface IAccountService : IReferenceDataElementService<AccountGroup, Account, AccountParam>
{
}