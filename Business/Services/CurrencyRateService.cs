using GLSoft.DoubleEntryHomeAccounting.Common.Services;
using GLSoft.DoubleEntryHomeAccounting.Common.Params;
using GLSoft.DoubleEntryHomeAccounting.Common.Infrastructure.Peaa;

namespace GLSoft.DoubleEntryHomeAccounting.Business.Services
{
    internal class CurrencyRateService : ICurrencyRateService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public CurrencyRateService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        //TODO: Implementation
        public Task<Guid> Add(CurrencyRateParam param)
        {
            throw new NotImplementedException();
        }

        //TODO: Implementation
        public Task Update(Guid entityId, CurrencyRateParam param)
        {
            throw new NotImplementedException();
        }

        //TODO: Implementation
        public Task Delete(Guid entityId)
        {
            throw new NotImplementedException();
        }
    }
}
