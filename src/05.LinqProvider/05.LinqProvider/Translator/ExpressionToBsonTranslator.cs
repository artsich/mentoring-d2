using MongoDB.Bson;
using System.Linq.Expressions;
using System.Text;

namespace _05.LinqProvider.Translator;

public class ExpressionToBsonTranslator : ExpressionVisitor
{
	private readonly Dictionary<ExpressionType, string> _opMap = new()
	{
		{ ExpressionType.Equal, "$eq" },
		{ ExpressionType.LessThan, "$lt" },
		{ ExpressionType.GreaterThan, "$gt" },
	};

	private readonly Dictionary<ExpressionType, string> _reversedOpMap = new()
	{
		{ ExpressionType.Equal, "$eq" },
		{ ExpressionType.LessThan, "$gt" },
		{ ExpressionType.GreaterThan, "$lt" },
	};

	private StringBuilder _query;

	public BsonDocument Translate(Expression exp)
	{
		_query = new StringBuilder();
		Visit(exp);
		return BsonDocument.Parse(_query.ToString());
	}

	protected override Expression VisitMethodCall(MethodCallExpression node)
	{
		if (node.Method.DeclaringType == typeof(Queryable)
			&& node.Method.Name == "Where")
		{
			var predicate = node.Arguments[1];
			Visit(predicate);

			return node;
		}

		return base.VisitMethodCall(node);
	}
	
	protected override Expression VisitBinary(BinaryExpression node)
	{
		if (node.Left.NodeType == ExpressionType.MemberAccess &&
			node.Right.NodeType == ExpressionType.MemberAccess)
		{
			throw new NotSupportedException("Left and Right can not be members!");
		}

		switch (node.NodeType)
		{

			case ExpressionType.AndAlso:
				AddArrayJson("$and", node);
				break;

			case ExpressionType.Equal:
			case ExpressionType.LessThan:
			case ExpressionType.GreaterThan:
				AddFieldJson(node);
				break;

			default:
				throw new NotSupportedException($"Operation '{node.NodeType}' is not supported");
		};

		return node;
	}

	protected override Expression VisitMember(MemberExpression node)
	{
		_query.Append($""" "{node.Member.Name}" """);
		return base.VisitMember(node);
	}

	protected override Expression VisitConstant(ConstantExpression node)
	{
		if (node.Type == typeof(string))
		{
			_query.Append($""" "{node.Value}" """);
		}
		else
		{
			_query.Append($""" {node.Value} """);
		}

		return base.VisitConstant(node);
	}

	private void AddArrayJson(string op, BinaryExpression node)
	{
		_query.Append($$"""{ "{{op}}": [""");

		Visit(node.Left);

		_query.Append(',');

		Visit(node.Right);

		_query.Append(" ]}");
	}

	private void AddFieldJson(BinaryExpression node)
	{
		if (node.Left.NodeType == ExpressionType.Constant)
		{
			AddFieldJson(_reversedOpMap[node.NodeType], node.Right, node.Left);
		}
		else
		{
			AddFieldJson(_opMap[node.NodeType], node.Left, node.Right);
		}
	}

	private void AddFieldJson(string op, Expression left, Expression right)
	{
		var visitor = new ReduceMemberAccessVisitor();

		_query.Append('{');

		Visit(visitor.Visit(left));

		_query.Append(" : {\"");
		_query.Append(op);
		_query.Append("\": ");

		Visit(visitor.Visit(right));
		_query.Append("} }");
	}
}
