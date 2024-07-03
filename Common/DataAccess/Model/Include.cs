using System.Linq.Expressions;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Model
{
    public class Include<T>
    {
        public List<LambdaExpression> Expressions { get; } = new();

        private Include()
        {

        }

        public static Include<T> Create<TP>(Func<T, TP> func)
        {
            Include<T> include = new Include<T>();
            Expression<Func<T, TP>> expression = t => func(t);
            include.Expressions.Add(expression);
            return include;
        }

        public static Include<T> Create<TP>(Func<T, ICollection<TP>> func)
        {
            Include<T> include = new Include<T>();
            Expression<Func<T, ICollection<TP>>> expression = t => func(t);
            include.Expressions.Add(expression);
            return include;
        }

        public Include<T> Add<TP>(Func<T, TP> func)
        {
            Expression<Func<T, TP>> expression = t => func(t);
            Expressions.Add(expression);
            return this;
        }

        public Include<T> Add<TP>(Func<T, ICollection<TP>> func)
        {
            Expression<Func<T, ICollection<TP>>> expression = t => func(t);
            Expressions.Add(expression);
            return this;
        }
    }
}
