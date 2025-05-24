using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
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
        Guard.CheckParamForNull(param);
        CheckIsoCodeString(param.Code);
        Guard.CheckCurrencyRate(initialRate);

        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ICurrencyRepository currencyRepository = unitOfWork.GetRepository<ICurrencyRepository>();

        CurrencyProfile currencyProfile = CurrencyProfileUtils.GetCurrencyData(param.Code);
        string symbol = string.IsNullOrEmpty(param.Symbol) ? currencyProfile.Symbol : param.Symbol;
        string name = string.IsNullOrEmpty(param.Name) ? currencyProfile.Name : param.Name;

        Currency addedCurrency = new Currency
        {
            Id = Guid.NewGuid(),
            IsoCode = currencyProfile.Code,
            Symbol = symbol,
            Name = name,
            Order = await currencyRepository.GetMaxOrder() + 1,
        };

        CurrencyRate currencyRate = new CurrencyRate
        {
            Id = Guid.NewGuid(),
            Rate = initialRate,
            Currency = addedCurrency,
            CurrencyId = addedCurrency.Id,
        };
        addedCurrency.Rates.Add(currencyRate);
        
        await currencyRepository.Add(addedCurrency);

        await unitOfWork.SaveChanges();

        return addedCurrency.Id;
    }

    public async Task Update(Guid currencyId, CurrencyParam param)
    {
        Guard.CheckParamForNull(param);

        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ICurrencyRepository currencyRepository = unitOfWork.GetRepository<ICurrencyRepository>();

        Currency updatedCurrency =  await Guard.CheckAndGetEntityById(currencyRepository.GetById, currencyId);
        
        updatedCurrency.Symbol = param.Symbol;
        updatedCurrency.Name = param.Name;

        await currencyRepository.Update(updatedCurrency);
        
        await unitOfWork.SaveChanges();
    }

    public async Task Delete(Guid currencyId)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ICurrencyRepository currencyRepository = unitOfWork.GetRepository<ICurrencyRepository>();
        IAccountRepository accountRepository = unitOfWork.GetRepository<IAccountRepository>();

        Currency deletedCurrency =  await Guard.CheckAndGetEntityById(currencyRepository.GetById, currencyId);

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
}