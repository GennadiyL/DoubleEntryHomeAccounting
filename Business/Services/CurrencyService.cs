using Common.DataAccess;
using Common.Infrastructure.Peaa;
using Common.Models;
using Common.Params;
using Common.Services;
using Common.Utils.Check;
using Common.Utils.Currency;
using Common.Utils.Ordering;

namespace Business.Services;

public class CurrencyService : ICurrencyService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public CurrencyService(
        IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task<Guid> Add(CurrencyParam param, decimal initialRate)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Guard.CheckParamForNull(param);
        CheckIsoCodeString(param.Code);
        CurrencyData currencyData = CurrencyDataUtils.GetCurrencyData(param.Code);
        string symbol = string.IsNullOrEmpty(param.Symbol) ? currencyData.Symbol : param.Symbol;
        string name = string.IsNullOrEmpty(param.Name) ? currencyData.Name : param.Name;

        ICurrencyRepository currencyRepository = await unitOfWork.GetRepository<ICurrencyRepository>();

        Currency addedCurrency = new Currency
        {
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
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Guard.CheckParamForNull(param);
        CheckIsoCodeString(param.Code);
        CurrencyData currencyData = CurrencyDataUtils.GetCurrencyData(param.Code);
        string symbol = string.IsNullOrEmpty(param.Symbol) ? currencyData.Symbol : param.Symbol;
        string name = string.IsNullOrEmpty(param.Name) ? currencyData.Name : param.Name;

        ICurrencyRepository currencyRepository = await unitOfWork.GetRepository<ICurrencyRepository>();

        Currency updatedCurrency = await currencyRepository.GetByIsoCode(currencyData.Code);
        updatedCurrency.Symbol = symbol;
        updatedCurrency.Name = name;

        await unitOfWork.SaveChanges();
    }

    public async Task Delete(string isoCode)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        CheckIsoCodeString(isoCode);

        ICurrencyRepository currencyRepository = await unitOfWork.GetRepository<ICurrencyRepository>();
        IAccountRepository accountRepository = await unitOfWork.GetRepository<IAccountRepository>();

        Currency deletedCurrency = await currencyRepository.GetByIsoCode(isoCode);
        if (deletedCurrency == null)
        {
            throw new ArgumentNullException($"Currency with entity isocode is not existed");
        }
        if ((await accountRepository.GetAccountsByCurrency(deletedCurrency)).Any())
        {
            throw new ArgumentException("This Currency can not be deleted because it's used in Account");
        }

        await currencyRepository.Delete(deletedCurrency);
        OrderingUtils.Reorder(await currencyRepository.GetList());

        await unitOfWork.SaveChanges();
    }

    public async Task SetOrder(Guid entityId, int order)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ICurrencyRepository currencyRepository = await unitOfWork.GetRepository<ICurrencyRepository>();

        Currency currency = await Getter.GetEntityById(currencyRepository.Get, entityId);
        if (currency.Order != order)
        {
            currency.Order = order;
            OrderingUtils.SetOrder( await currencyRepository.GetList(), currency, order);
            await currencyRepository.UpdateList(await currencyRepository.GetList());
        }

        await unitOfWork.SaveChanges();
    }

    public async Task SetFavoriteStatus(Guid entityId, bool isFavorite)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ICurrencyRepository currencyRepository = await unitOfWork.GetRepository<ICurrencyRepository>();

        Currency currency = await Getter.GetEntityById(currencyRepository.Get, entityId);
        if (currency.IsFavorite != isFavorite)
        {
            currency.IsFavorite = isFavorite;
            await currencyRepository.Update(currency);
        }

        await unitOfWork.SaveChanges();
    }

    private static void CheckIsoCodeString(string isoCode)
    {
        if (string.IsNullOrEmpty(isoCode))
        {
            throw new ArgumentNullException($"IsoCode as entity cannot be a null or empty");
        }
    }
}