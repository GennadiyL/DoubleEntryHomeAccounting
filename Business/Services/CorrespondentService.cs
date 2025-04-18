using GLSoft.DoubleEntryHomeAccounting.Business.Services.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Repositories;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Params;
using GLSoft.DoubleEntryHomeAccounting.Common.Services;

namespace GLSoft.DoubleEntryHomeAccounting.Business.Services;

public class CorrespondentService : ElementService<CorrespondentGroup, Correspondent, ElementParam>, ICorrespondentService
{
    public CorrespondentService(IUnitOfWorkFactory unitOfWork) : base(unitOfWork)
    {
    }

    protected override Func<IAccountRepository, Correspondent, Task<ICollection<Account>>> GetAccountsByEntity =>
        async (accountRepository, correspondent) => await accountRepository.GetByCorrespondentId(correspondent.Id);

    protected override Action<Correspondent, Account> AccountEntitySetter =>
        (correspondent, account) => account.Correspondent = correspondent;
}