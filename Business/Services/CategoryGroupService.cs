using Business.Services.Base;
using Common.Infrastructure.Peaa;
using Common.Models;
using Common.Params;
using Common.Services;

namespace Business.Services;

public class CategoryGroupService 
    : ReferenceParentEntityService<CategoryGroup, Category, GroupEntityParam>, ICategoryGroupService
{
    public CategoryGroupService(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }
}