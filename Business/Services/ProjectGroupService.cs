using Business.Services.Base;
using Common.Infrastructure.Peaa;
using Common.Models;
using Common.Params;
using Common.Services;

namespace Business.Services;

public class ProjectGroupService 
    : ReferenceParentEntityService<ProjectGroup, Project, GroupEntityParam>, IProjectGroupService
{
    public ProjectGroupService(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }
}