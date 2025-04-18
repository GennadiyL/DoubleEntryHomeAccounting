namespace GLSoft.DoubleEntryHomeAccounting.Common.Infrastructure;

public delegate object ServiceFactory(Type serviceType);

public static class ServiceFactoryExtensions
{
    public static T GetInstance<T>(this ServiceFactory serviceFactory)
    {
        Type type = typeof(T);
        return (T)serviceFactory(type);
    }
}