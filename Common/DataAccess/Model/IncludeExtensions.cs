using System.Linq.Expressions;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Model
{
    public static class IncludeExtensions
    {
        public static Include<T> Include<T, TP>(this T entity, Func<T, TP> func)
        {
            Include<T> include = new Include<T>();
            Expression<Func<T, TP>> expression = t => func(t);
            include.Expressions.Add(expression);
            return include;
        }

        public static Include<T> Include<T, TP>(this Include<T> include, Func<T, TP> func)
        {
            Expression<Func<T, TP>> expression = t => func(t);
            include.Expressions.Add(expression);
            return include;
        }
    }
}
