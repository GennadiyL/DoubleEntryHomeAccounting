using GLSoft.DoubleEntryHomeAccounting.Business.Services.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.Infrastructure.Peaa;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Params;
using GLSoft.DoubleEntryHomeAccounting.Common.Services;

namespace GLSoft.DoubleEntryHomeAccounting.Business.Services;

public class ProjectService : ElementService<ProjectGroup, Project, ElementParam>, IProjectService
{
    public ProjectService(IUnitOfWorkFactory unitOfWork) : base(unitOfWork)
    {
    }

    protected override Func<IAccountRepository, Project, Task<ICollection<Account>>> GetAccountsByEntity =>
        async (accountRepository, project) => await accountRepository.GetByProjectId(project.Id);

    protected override Action<Project, Account> AccountEntitySetter => 
        (project, account) => account.Project = project;
}