﻿using GLSoft.DoubleEntryHomeAccounting.Business.Services.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Repositories;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Params;
using GLSoft.DoubleEntryHomeAccounting.Common.Services;

namespace GLSoft.DoubleEntryHomeAccounting.Business.Services;

public class CategoryService : ElementService<CategoryGroup, Category, ElementParam>, ICategoryService
{
    public CategoryService(IUnitOfWorkFactory unitOfWork) : base(unitOfWork)
    {
    }

    protected override Func<IAccountRepository, Category, Task<ICollection<Account>>> GetAccountsByEntity =>
        async (accountRepository, category) => await accountRepository.GetByCategoryId(category.Id);

    protected override Action<Category, Account> SetAccountEntity =>
        (category, account) =>
        {
            account.Category = category;
            account.CategoryId = category?.Id;
        };
}