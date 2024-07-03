using Business.Services.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.Infrastructure.Peaa;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Params;
using GLSoft.DoubleEntryHomeAccounting.Common.Services;

namespace Business.Services;

public class CorrespondentService 
    : ReferenceDataElementService<CorrespondentGroup, Correspondent, ElementParam>, ICorrespondentService
{
    public CorrespondentService(
        IUnitOfWorkFactory unitOfWorkFactory,
        ICorrespondentGroupRepository groupRepository,
        ICorrespondentRepository elementRepository,
        IAccountRepository accountRepository)
        : base(unitOfWorkFactory, groupRepository, elementRepository, accountRepository)
    {
    }

    protected override Func<IAccountRepository, Correspondent, Task<ICollection<Account>>> GetAccountsByEntity =>
        async (accountRepository, correspondent) => await accountRepository.GetByCorrespondentId(correspondent.Id);


    protected override Action<Correspondent, Account> AccountEntitySetter =>
        (correspondent, account) => account.Correspondent = correspondent;
}