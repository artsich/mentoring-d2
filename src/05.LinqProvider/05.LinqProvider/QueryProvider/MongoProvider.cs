using _05.LinqProvider.Helpers;
using _05.LinqProvider.Translator;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace _05.LinqProvider.QueryProvider;

public class MongoProvider : IQueryProvider
{
	private readonly IMongoDatabase db;

	public MongoProvider(IMongoDatabase db)
	{
		this.db = db;
	}

	public IQueryable CreateQuery(Expression expression)
	{
		throw new NotImplementedException();
	}

	public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
	{
		return new MongoQuery<TElement>(expression, this);
	}

	public object? Execute(Expression expression)
	{
		throw new NotImplementedException();
	}

	public TResult Execute<TResult>(Expression expression)
	{
		Type itemType = TypeHelper.GetElementType(expression.Type);
		var collection = db.GetCollection<BsonDocument>(itemType.Name);

		var query = new ExpressionToBsonTranslator()
			.Translate(expression);

		var items = collection.Find(query).ToList();

		var result = Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType)) as IList;

		foreach(var it in items)
		{
			result!.Add(BsonSerializer.Deserialize(it, itemType));
		}

		return (TResult)result!;
	}
}
