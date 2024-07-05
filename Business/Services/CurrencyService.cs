using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.Infrastructure.Peaa;
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
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IAccountRepository _accountRepository;
    public CurrencyService(
        IUnitOfWorkFactory unitOfWorkFactory,
        ICurrencyRepository currencyRepository,
        IAccountRepository accountRepository)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _currencyRepository = currencyRepository;
        _accountRepository = accountRepository;
    }

    public async Task<Guid> Add(CurrencyParam param, decimal initialRate)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Guard.CheckParamForNull(param);
        CheckIsoCodeString(param.Code);
        ChechInitialRate(initialRate);

        CurrencyData currencyData = CurrencyDataUtils.GetCurrencyData(param.Code);
        string symbol = string.IsNullOrEmpty(param.Symbol) ? currencyData.Symbol : param.Symbol;
        string name = string.IsNullOrEmpty(param.Name) ? currencyData.Name : param.Name;

        Currency addedCurrency = new Currency
        {
            IsoCode = currencyData.Code,
            Symbol = symbol,
            Name = name,
            Order = await _currencyRepository.GetMaxOrder() + 1,
        };

        addedCurrency.Rates.Add(new CurrencyRate { Rate = initialRate });
        await _currencyRepository.Add(addedCurrency);

        await unitOfWork.SaveChanges();

        return addedCurrency.Id;
    }

    public async Task Update(CurrencyParam param)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Guard.CheckParamForNull(param);
        CheckIsoCodeString(param.Code);

        CurrencyData currencyData = CurrencyDataUtils.GetCurrencyData(param.Code);
        string symbol = string.IsNullOrEmpty(param.Symbol) ? currencyData.Symbol : param.Symbol;
        string name = string.IsNullOrEmpty(param.Name) ? currencyData.Name : param.Name;

        Currency updatedCurrency = await _currencyRepository.GetByIsoCode(currencyData.Code);
        updatedCurrency.Symbol = symbol;
        updatedCurrency.Name = name;

        await unitOfWork.SaveChanges();
    }

    public async Task Delete(string isoCode)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        CheckIsoCodeString(isoCode);

        Currency deletedCurrency = await _currencyRepository.GetByIsoCode(isoCode);
        if (deletedCurrency == null)
        {
            throw new ArgumentNullException($"$Currency with iso code '{isoCode}' does not exist in DB");
        }

        if ((await _accountRepository.GetByCurrencyId(deletedCurrency.Id)).Any())
        {
            throw new ArgumentException("This Currency can not be deleted because it is used in one or more Accounts");
        }

        await _currencyRepository.Delete(deletedCurrency.Id);
        ICollection<Currency> currencies = await _currencyRepository.GetAll();
        OrderingUtils.Reorder(currencies);
        await _currencyRepository.Update(currencies);

        await unitOfWork.SaveChanges();
    }

    public async Task SetOrder(Guid entityId, int order)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Currency currency = await Getter.GetEntityById(_currencyRepository, entityId);
        if (currency.Order == order)
        {
            return;
        }

        ICollection<Currency> currencies = await _currencyRepository.GetAll();
        OrderingUtils.SetOrder(currencies, currency, order);

        await _currencyRepository.Update(currencies);

        await unitOfWork.SaveChanges();
    }

    public async Task SetFavoriteStatus(Guid entityId, bool isFavorite)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Currency currency = await Getter.GetEntityById(_currencyRepository, entityId);
        if (currency.IsFavorite == isFavorite)
        {
            return;
        }

        currency.IsFavorite = isFavorite;
        await _currencyRepository.Update(currency);

        await unitOfWork.SaveChanges();
    }

    private static void CheckIsoCodeString(string isoCode)
    {
        if (string.IsNullOrEmpty(isoCode))
        {
            throw new ArgumentNullException($"IsoCode as entity cannot be a null or empty");
        }
    }

    private static void ChechInitialRate(decimal initialRate)
    {
        if (initialRate <= 0)
        {
            throw new ArgumentException($"Rate must be more than 0.");
        }
    }
}