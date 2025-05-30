﻿using GLSoft.DoubleEntryHomeAccounting.Common.Params;
using GLSoft.DoubleEntryHomeAccounting.Common.Services.Base;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Services;

public interface ITransactionService : IEntityService<TransactionParam>
{
    Task DeleteTransactionList(List<Guid> transactionIds);
}