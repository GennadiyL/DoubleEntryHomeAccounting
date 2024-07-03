using Business.Services.Base;
using Common.Infrastructure.Peaa;
using Common.Models;
using Common.Params;
using Common.Services;

namespace Business.Services;

public class TemplateGroupService : ReferenceParentEntityService<TemplateGroup, Template, GroupEntityParam>, ITemplateGroupService
{
    public TemplateGroupService(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }
}