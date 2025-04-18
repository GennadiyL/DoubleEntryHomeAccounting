﻿using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Repositories;
using GLSoft.DoubleEntryHomeAccounting.Common.Exceptions;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Params;
using GLSoft.DoubleEntryHomeAccounting.Common.Services;
using GLSoft.DoubleEntryHomeAccounting.Common.Utils.Check;
using GLSoft.DoubleEntryHomeAccounting.Common.Utils.Currency;
using GLSoft.DoubleEntryHomeAccounting.Common.Utils.Ordering;

namespace GLSoft.DoubleEntryHomeAccounting.Business.Services;

public class CurrencyService : ICurrencyService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    public CurrencyService(IUnitOfWorkFactory unitOfWorkFactory) => _unitOfWorkFactory = unitOfWorkFactory;

    public async Task<Guid> Add(CurrencyParam param, decimal initialRate)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ICurrencyRepository currencyRepository = unitOfWork.GetRepository<ICurrencyRepository>();

        Guard.CheckParamForNull(param);
        CheckIsoCodeString(param.Code);
        CheckInitialRate(initialRate);

        CurrencyData currencyData = CurrencyDataUtils.GetCurrencyData(param.Code);
        string symbol = string.IsNullOrEmpty(param.Symbol) ? currencyData.Symbol : param.Symbol;
        string name = string.IsNullOrEmpty(param.Name) ? currencyData.Name : param.Name;

        Currency addedCurrency = new Currency
        {
            Id = Guid.NewGuid(),
            IsoCode = currencyData.Code,
            Symbol = symbol,
            Name = name,
            Order = await currencyRepository.GetMaxOrder() + 1,
        };

        addedCurrency.Rates.Add(new CurrencyRate { Rate = initialRate });
        await currencyRepository.Add(addedCurrency);

        await unitOfWork.SaveChanges();

        return addedCurrency.Id;
    }

    public async Task Update(CurrencyParam param)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ICurrencyRepository currencyRepository = unitOfWork.GetRepository<ICurrencyRepository>();

        Guard.CheckParamForNull(param);
        CheckIsoCodeString(param.Code);

        CurrencyData currencyData = CurrencyDataUtils.GetCurrencyData(param.Code);
        string symbol = string.IsNullOrEmpty(param.Symbol) ? currencyData.Symbol : param.Symbol;
        string name = string.IsNullOrEmpty(param.Name) ? currencyData.Name : param.Name;

        Currency updatedCurrency = await currencyRepository.GetByIsoCode(currencyData.Code);
        updatedCurrency.Symbol = symbol;
        updatedCurrency.Name = name;

        await unitOfWork.SaveChanges();
    }

    public async Task Delete(string isoCode)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ICurrencyRepository currencyRepository = unitOfWork.GetRepository<ICurrencyRepository>();
        IAccountRepository accountRepository = unitOfWork.GetRepository<IAccountRepository>();

        CheckIsoCodeString(isoCode);

        Currency deletedCurrency = await currencyRepository.GetByIsoCode(isoCode);
        if (deletedCurrency == null)
        {
            throw new MissingCurrencyException(isoCode);
        }

        if ((await accountRepository.GetByCurrencyId(deletedCurrency.Id)).Any())
        {
            throw new ReferenceEntityException(typeof(Currency), typeof(Account), deletedCurrency.Id);
        }

        await currencyRepository.Delete(deletedCurrency.Id);
        ICollection<Currency> currencies = await currencyRepository.GetAll();
        currencies.Reorder();
        await currencyRepository.Update(currencies);

        await unitOfWork.SaveChanges();
    }

    public async Task SetOrder(Guid entityId, int order)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ICurrencyRepository currencyRepository = unitOfWork.GetRepository<ICurrencyRepository>();

        Currency currency = await Guard.CheckAndGetEntityById(currencyRepository.GetById, entityId);
        if (currency.Order == order)
        {
            return;
        }

        ICollection<Currency> currencies = await currencyRepository.GetAll();
        currencies.SetOrder(currency, order);

        await currencyRepository.Update(currencies);

        await unitOfWork.SaveChanges();
    }

    public async Task SetFavoriteStatus(Guid entityId, bool isFavorite)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ICurrencyRepository currencyRepository = unitOfWork.GetRepository<ICurrencyRepository>();

        Currency currency = await Guard.CheckAndGetEntityById(currencyRepository.GetById, entityId);
        if (currency.IsFavorite == isFavorite)
        {
            return;
        }

        currency.IsFavorite = isFavorite;
        await currencyRepository.Update(currency);

        await unitOfWork.SaveChanges();
    }

    private static void CheckIsoCodeString(string isoCode)
    {
        if (string.IsNullOrEmpty(isoCode))
        {
            throw new InvalidCurrencyIsoCodeException(null);
        }
    }

    private static void CheckInitialRate(decimal initialRate)
    {
        if (initialRate <= 0)
        {
            throw new InvalidCurrencyRateException(initialRate);
        }
    }
}