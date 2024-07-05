using System.Linq.Expressions;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Model
{
    public class Include<T>
    {
        internal List<LambdaExpression> Expressions { get; } = new();
        internal Include()
        {
        }
    }
}
