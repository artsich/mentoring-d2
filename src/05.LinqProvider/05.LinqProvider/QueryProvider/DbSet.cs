using MongoDB.Driver;
using System.Collections;
using System.Linq.Expressions;

namespace _05.LinqProvider.QueryProvider;

public class DbSet<T> : IQueryable<T>
{
	private readonly Expression Expr;
	private readonly IQueryProvider QueryProvider;

	public DbSet(IMongoDatabase db)
		: this(new MongoProvider(db))
	{
	}

	public DbSet(IQueryProvider queryProvider)
	{
		Expr = Expression.Constant(this);
		QueryProvider = queryProvider;
	}

	public Type ElementType => typeof(T);

	public Expression Expression => Expr;

	public IQueryProvider Provider => QueryProvider;

	public IEnumerator<T> GetEnumerator()
	{
		return QueryProvider.Execute<IEnumerable<T>>(Expr).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return QueryProvider.Execute<IEnumerable>(Expr).GetEnumerator();
	}
}
