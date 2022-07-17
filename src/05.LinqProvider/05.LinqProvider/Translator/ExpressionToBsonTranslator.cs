using MongoDB.Bson;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;

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
        switch (node.NodeType)
        {
            case ExpressionType.Equal:
                CheckBinaryNode(node);
                AddFieldJson("$eq", node);

                break;

			case ExpressionType.AndAlso:
                //CheckBinaryNode(node);
                AddArrayJson("$and", node);

                break;

            case ExpressionType.GreaterThan:
                //CheckBinaryNode(node);
                AddFieldJson("$gt", node);

                break;

            case ExpressionType.LessThan:
                //CheckBinaryNode(node);
                AddFieldJson("$lt", node);

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
        var sb = new StringBuilder();

        query += "{ \"";
        query += op;
        query += "\" : [";

        Visit(node.Left);

        query += ",";

        Visit(node.Right);

        query += " ]";
        query += "}";
    }

    private void AddFieldJson(string op, BinaryExpression node)
	{
        query += "{";
        Visit(node.Left);
        query += " : {\"";
        query += op;
        query += "\": ";
        Visit(node.Right);
        query += "}";
        query += "}";
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        query += $"\"{node.Member.Name}\"";
        return base.VisitMember(node);
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        var f = Expression.Lambda(node).Compile();
        var value = f.DynamicInvoke();

        query += $"\"{value}\"";
        return node;
	}
}
