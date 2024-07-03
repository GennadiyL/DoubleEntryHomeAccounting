namespace GLSoft.DoubleEntryHomeAccounting.Common.Infrastructure;

public delegate object ServiceFactory(Type serviceType);
public static class ServiceFactoryExtensions
{
    public static T GetInstance<T>(this ServiceFactory serviceFactory)
    {
        var type = typeof(T);
        return (T)serviceFactory(type);
    }
}