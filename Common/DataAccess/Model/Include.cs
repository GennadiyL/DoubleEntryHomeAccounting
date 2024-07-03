using System.Linq.Expressions;
using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Model
{
    public class Include<T, TP>
        where T : IEntity
        where TP : IEntity
    {
        public LambdaExpression Expressions { get; set; }

        public static implicit operator Include<T, TP>(Func<T, TP> func)
        {
            return new Include<T, TP>(func);
        }

        public static implicit operator Include<T, TP>(Func<T, IList<TP>> func)
        {
            //ICollection<T> l = new List<T>();

            //IOrderedEnumerable<T> orderedEnumerable = l.OrderBy(e => e);
            //IOrderedEnumerable<T> thenBy = orderedEnumerable.ThenBy(e => e);

            Func<IList<T>, IOrderedEnumerable<T>> func1 = t => t.OrderBy(e => e.Id);

            return new Include<T, TP>(func);
        }

        private Include(Func<T, TP> func)
        {
            Expression<Func<T, TP>> expression = t => func(t);
            Expressions = expression;
        }

        private Include(Func<T, IList<TP>> func)
        {
            Expression<Func<T, IList<TP>>> expression = t => func(t);
            Expressions = expression;
        }
    }
}
