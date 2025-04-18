using GLSoft.DoubleEntryHomeAccounting.Business.Services.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Params;
using GLSoft.DoubleEntryHomeAccounting.Common.Services;

namespace GLSoft.DoubleEntryHomeAccounting.Business.Services;

public class ProjectGroupService 
    : GroupService<ProjectGroup, Project, GroupParam>, IProjectGroupService
{
    public ProjectGroupService(IUnitOfWorkFactory unitOfWorkFactory) : base(unitOfWorkFactory)
    {
    }
}