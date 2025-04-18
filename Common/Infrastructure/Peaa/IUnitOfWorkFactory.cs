namespace GLSoft.DoubleEntryHomeAccounting.Common.Infrastructure.Peaa;

public interface IUnitOfWorkFactory
{
    IUnitOfWork Create(bool isReadOnly = false);
}