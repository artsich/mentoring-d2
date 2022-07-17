using MongoDB.Bson;
using System.Linq.Expressions;

namespace _05.LinqProvider.Translator;

public class ExpressionToBsonTranslator : ExpressionVisitor
{
    private string query = "";

    public BsonDocument Translate(Expression exp)
    {
        Visit(exp);
        var bson = BsonDocument.Parse(query);
        return bson;
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

        var opMap = new Dictionary<ExpressionType, string>()
        {
            { ExpressionType.GreaterThan, "$gt" },
            { ExpressionType.LessThan, "$lt" }
        };

        var reverseMap = new Dictionary<ExpressionType, string>()
        {
            { ExpressionType.GreaterThan, "$lt" },
            { ExpressionType.LessThan, "$gt" }
        };

        switch (node.NodeType)
        {
            case ExpressionType.Equal:
                CheckBinaryNode(node);
                AddFieldJson("$eq", node);
                break;

			case ExpressionType.AndAlso:
                AddArrayJson("$and", node);
                break;

            case ExpressionType.LessThan:
            case ExpressionType.GreaterThan:
                if (node.Left.NodeType == ExpressionType.Constant)
				{
                    AddFieldJson(reverseMap[node.NodeType], node, reversed: true);
                }
                else
				{
    			    AddFieldJson(opMap[node.NodeType], node);
				}
                break;

            default:
                throw new NotSupportedException($"Operation '{node.NodeType}' is not supported");
        };

        return node;
    }

    private void CheckBinaryNode(BinaryExpression node)
    {
        if (node.Left.NodeType != ExpressionType.MemberAccess)
            throw new NotSupportedException($"Left operand should be property or field: {node.NodeType}");

        if (node.Right.NodeType != ExpressionType.Constant)
            throw new NotSupportedException($"Right operand should be constant: {node.NodeType}");
    }

    private void AddArrayJson(string op, BinaryExpression node)
	{
        query += $$"""{ "{{op}}": [""";

        Visit(node.Left);

        query += ",";

        Visit(node.Right);

        query += " ]}";
    }

    private void AddFieldJson(string op, BinaryExpression node, bool reversed = false)
	{
        var left = reversed ? node.Right : node.Left;
        var right = reversed ? node.Left : node.Right;

        query += "{";
        Visit(left);
        query += $$""" : { "{{op}}": """;
        Visit(right);
        query += "}}";
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        query += $$""" "{{node.Member.Name}}" """;
        return base.VisitMember(node);
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        if (node.Type == typeof(string))
		{
            query += $$""" "{{node.Value}}" """;
		} 
        else
		{
            query += $$""" {{node.Value}} """;
        }

        return base.VisitConstant(node);
	}
}
