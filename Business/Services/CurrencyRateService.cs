using GLSoft.DoubleEntryHomeAccounting.Common.Services;
using GLSoft.DoubleEntryHomeAccounting.Common.Params;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Repositories;
using GLSoft.DoubleEntryHomeAccounting.Common.Utils.Check;
using GLSoft.DoubleEntryHomeAccounting.Common.Exceptions;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;

namespace GLSoft.DoubleEntryHomeAccounting.Business.Services;

internal class CurrencyRateService : ICurrencyRateService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public CurrencyRateService(IUnitOfWorkFactory unitOfWorkFactory) => _unitOfWorkFactory = unitOfWorkFactory;

    public async Task<Guid> AddOrUpdate(CurrencyRateParam param)
    {
        Guard.CheckParamForNull(param);
        
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ISystemConfigRepository systemConfigRepository = unitOfWork.GetRepository<ISystemConfigRepository>();
        ICurrencyRateRepository currencyRateRepository = unitOfWork.GetRepository<ICurrencyRateRepository>();

        CheckParamRate(param);
        await CheckParamDate(param, systemConfigRepository);

        CurrencyRate currencyRate = await Guard.CheckAndGetEntityById(
            g => currencyRateRepository.GetRateOnDate(g, DateOnly.FromDateTime(DateTime.Today)),
            param.CurrencyId);

        if (currencyRate.Date == DateOnly.FromDateTime(DateTime.Today))
        {
            currencyRate.Comment = param.Comment;
            currencyRate.Rate = param.Rate;
            await currencyRateRepository.Update(currencyRate);
        }
        else
        {
            currencyRate = new CurrencyRate
            {
                Id = Guid.NewGuid(),
                Currency = currencyRate.Currency,
                Comment = param.Comment,
                Date = param.Date,
                Rate = param.Rate,
            };
            await currencyRateRepository.Add(currencyRate);
        }

        await unitOfWork.SaveChanges();

        return currencyRate.Id;
    }

    public async Task Delete(Guid currencyId, DateOnly fromDate, DateOnly toDate)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ISystemConfigRepository systemConfigRepository = unitOfWork.GetRepository<ISystemConfigRepository>();
        ICurrencyRateRepository currencyRateRepository = unitOfWork.GetRepository<ICurrencyRateRepository>();

        Currency currency = await Guard.CheckAndGetEntityById(
            g => currencyRateRepository.GetRatesByPeriod(g, fromDate, toDate), currencyId);

        DateOnly minDate = await systemConfigRepository.GetMinDate();

        List<CurrencyRate> currencyRates = currency.Rates.Where(e => e.Date != minDate).ToList();

        await currencyRateRepository.Delete(currencyRates.Select(e => e.Id).ToList());

        await unitOfWork.SaveChanges();
    }

    private static void CheckParamRate(CurrencyRateParam param)
    {
        if (param.Rate <= 0)
        {
            throw new InvalidCurrencyRateException(param.Rate);
        }
    }

    private static async Task CheckParamDate(CurrencyRateParam param, ISystemConfigRepository systemConfigRepository)
    {
        DateOnly minDate = await systemConfigRepository.GetMinDate();
        DateOnly maxDate = await systemConfigRepository.GetMaxDate();
        if (param.Date < minDate || param.Date > maxDate)
        {
            throw new DateTimeOutOfRangeException(minDate, maxDate, param.Date);
        }
    }
}