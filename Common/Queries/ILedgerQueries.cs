using GLSoft.DoubleEntryHomeAccounting.Common.Infos;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Queries;

public interface ILedgerQueries
{
    LedgerInfo Get();
}