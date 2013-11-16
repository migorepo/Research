using System;
using System.Linq.Expressions;

namespace Wp8.Framework.Utils.Utils
{
    public static class ExpressionHelper
    {
        public static string GetName<TClass, TProp>(Expression<Func<TClass, TProp>> expression)
        {
            var body = expression.Body as UnaryExpression;
            if (body != null)
                return ((MemberExpression)(body.Operand)).Member.Name;
            return ((MemberExpression)expression.Body).Member.Name;
        }
    }
}
