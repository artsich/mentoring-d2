using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace _05.LinqProvider.Translator;

internal struct Filter
{
    public string Left;
    public string Op;
    public object Right;

	public Filter(string left, string op, object right)
	{
        Left = left;
        Op = op;
        Right = right;
	}
}

public class ExpressionToBsonTranslator : ExpressionVisitor
{
    private static readonly FilterDefinitionBuilder<BsonDocument> Builder = 
        Builders<BsonDocument>.Filter;

    private FilterDefinition<BsonDocument> bsonFilter;

    Stack<FilterDefinition<BsonDocument>> left;
    Stack<FilterDefinition<BsonDocument>> right;

    List<Filter> filters = new ();

    public FilterDefinition<BsonDocument> Translate(Expression exp)
    {
        bsonFilter = Builder.Empty;
        Visit(exp);
        return bsonFilter;
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
                if (node.Left.NodeType != ExpressionType.MemberAccess)
                    throw new NotSupportedException($"Left operand should be property or field: {node.NodeType}");

                if (node.Right.NodeType != ExpressionType.Constant)
                    throw new NotSupportedException($"Right operand should be constant: {node.NodeType}");

                Visit(node.Left);
                Visit(node.Right);
				break;

			case ExpressionType.AndAlso:
                Visit(node.Left);
                Visit(node.Right);

                bsonFilter = left.Pop() & right.Pop();
                break;

            case ExpressionType.LessThan:
                Visit(node.Left);
                Visit(node.Right);
                break;

            default:
                throw new NotSupportedException($"Operation '{node.NodeType}' is not supported");
        };

        return node;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        //var filter = filters[filters.Count - 1];
        //filter.Left = node.Member.Name;
        //filters[filters.Count - 1] = filter;

        return base.VisitMember(node);
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        var filter = filters[filters.Count - 1];
        filter.Right = node.Value ?? throw new ArgumentNullException("Node can not be null!"); ;
        filters[filters.Count - 1] = filter;

        return node;
    }
}
