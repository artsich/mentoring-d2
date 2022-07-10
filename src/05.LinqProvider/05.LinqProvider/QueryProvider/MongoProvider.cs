using MongoDB.Driver;
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
		var collection = db.GetCollection<TResult>(typeof(TResult).Name);

		throw new NotImplementedException();
	}
}
