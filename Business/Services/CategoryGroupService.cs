using Business.Services.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.Infrastructure.Peaa;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Params;
using GLSoft.DoubleEntryHomeAccounting.Common.Services;

namespace Business.Services;

public class CategoryGroupService 
    : ReferenceDataGroupService<CategoryGroup, Category, GroupParam>, ICategoryGroupService
{
    public CategoryGroupService(
        IUnitOfWorkFactory unitOfWorkFactory,
        ICategoryGroupRepository  groupRepository,
        ICategoryRepository elementRepository) 
        : base(unitOfWorkFactory, groupRepository, elementRepository)
    {
    }
}