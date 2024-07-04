using Business.Services.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.Infrastructure.Peaa;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Params;
using GLSoft.DoubleEntryHomeAccounting.Common.Services;

namespace Business.Services;

public class TemplateGroupService : ReferenceDataGroupService<TemplateGroup, Template, GroupParam>, ITemplateGroupService
{
    public TemplateGroupService(
        IUnitOfWorkFactory unitOfWorkFactory,
        ITemplateGroupRepository groupRepository,
        ITemplateRepository elementRepository)
        : base(unitOfWorkFactory, groupRepository, elementRepository)
    {
    }
}