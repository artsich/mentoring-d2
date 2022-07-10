using System.Collections;
using System.Linq.Expressions;

namespace _05.LinqProvider.QueryProvider;

public class MongoQuery<T> : IQueryable<T>
{
	private readonly MongoProvider _provider;

	public MongoQuery(Expression expression, MongoProvider provider)
	{
		Expression = expression;
		_provider = provider;
	}

	public Type ElementType => typeof(T);

	public Expression Expression { get; }

	public IQueryProvider Provider => _provider;

	public IEnumerator<T> GetEnumerator()
	{
		return _provider.Execute<IEnumerable<T>>(Expression).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _provider.Execute<IEnumerable>(Expression).GetEnumerator();
	}
}
