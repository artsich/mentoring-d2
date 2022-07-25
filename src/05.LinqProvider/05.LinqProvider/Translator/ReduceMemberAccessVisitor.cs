using System.Linq.Expressions;
using System.Reflection;

namespace _05.LinqProvider.Translator;

class ReduceMemberAccessVisitor : ExpressionVisitor
{
	protected override Expression VisitMember(MemberExpression memberExpression)
	{
		var expression = memberExpression.Expression;

		if (expression is ConstantExpression constant)
		{
			var container = constant.Value;
			var member = memberExpression.Member;

			switch (member)
			{
				case FieldInfo field:
					{
						var value = field.GetValue(container);
						return Expression.Constant(value);
					}
				case PropertyInfo property:
					{
						var value = property.GetValue(container, null);
						return Expression.Constant(value);
					}
			}
		}

		return base.VisitMember(memberExpression);
	}
}
