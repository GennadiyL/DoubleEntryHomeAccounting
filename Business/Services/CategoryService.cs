using GLSoft.DoubleEntryHomeAccounting.Business.Services.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.Infrastructure.Peaa;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Params;
using GLSoft.DoubleEntryHomeAccounting.Common.Services;

namespace GLSoft.DoubleEntryHomeAccounting.Business.Services;

public class CategoryService 
    : ReferenceDataElementService<CategoryGroup, Category, ElementParam>, ICategoryService
{
    public CategoryService(
        IUnitOfWorkFactory unitOfWorkFactory,
        ICategoryGroupRepository groupRepository,
        ICategoryRepository elementRepository,
        IAccountRepository accountRepository)
        : base(unitOfWorkFactory, groupRepository, elementRepository, accountRepository)
    {
    }

    protected override Func<IAccountRepository, Category, Task<ICollection<Account>>> GetAccountsByEntity =>
        async (accountRepository, category) => await accountRepository.GetByCategoryId(category.Id);

    protected override Action<Category, Account> AccountEntitySetter =>
        (category, account) => account.Category = category;
}