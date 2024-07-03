using Business.Services.Base;
using Common.Infrastructure.Peaa;
using Common.Models;
using Common.Params;
using Common.Services;

namespace Business.Services;

public class CorrespondentGroupService 
    : ReferenceParentEntityService<CorrespondentGroup, Correspondent, GroupEntityParam>, ICorrespondentGroupService
{
    public CorrespondentGroupService(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }
}