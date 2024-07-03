using Business.Services.Base;
using Common.Infrastructure.Peaa;
using Common.Models;
using Common.Params;
using Common.Services;

namespace Business.Services;

public class AccountGroupService 
    : ReferenceParentEntityService<AccountGroup, AccountSubGroup, AccountGroupParam>, IAccountGroupService
{
    public AccountGroupService(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {

    }
}