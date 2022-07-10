using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace _05.LinqProvider.Translator;

public class ExpressionToBsonTranslator : ExpressionVisitor
{
    private static readonly FilterDefinitionBuilder<BsonDocument> Builder = 
        Builders<BsonDocument>.Filter;

    private FilterDefinition<BsonDocument> filter; 

    public FilterDefinition<BsonDocument> Translate(Expression exp)
    {
        filter = Builder.Empty;
        Visit(exp);
        return filter;
    }
}
