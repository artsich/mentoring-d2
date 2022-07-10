using System.Linq.Expressions;

namespace _05.LinqProviderTests;

public class DbSetTests
{
	private readonly string DbName = "Products";
	private readonly IMongoDatabase db;

	private IMongoCollection<Product> CollectionProduct => db.GetCollection<Product>(nameof(Product));

	public DbSetTests()
	{
		var mongoSettings = MongoClientSettings.FromUrl(
			new MongoUrl("mongodb://localhost:27017")
		);
		mongoSettings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
		var mongoClient = new MongoClient(mongoSettings);

		db = mongoClient.GetDatabase("DbName");
		db.DropCollection(nameof(Product));

		PrepareData(db);
	}

	[Fact]
	public void GivenWhenMethod_CheckProductsByExpr_ReturnCorrespondingProductCount()
	{
		Expression<Func<Product, bool>> expr = x => x.Price > 60;

		var set = new DbSet<Product>(db);
		var result = set.Where(expr).ToList();

		Assert.Equal(
			expected: CollectionProduct.CountDocuments(expr),
			actual: result.Count);
	}


	[Fact]
	public void GivenWhenMethod_CheckProductsByExpr_ReturnCorrespondingProductCount__Second()
	{
		Expression<Func<Product, bool>> expr = x => x.Price < 50 && x.Type == "Type2";

		var set = new DbSet<Product>(db);
		var result = set.Where(expr).ToList();

		Assert.Equal(
			expected: CollectionProduct.CountDocuments(expr),
			actual: result.Count);
	}

	private static void PrepareData(IMongoDatabase db)
	{
		var col = db.GetCollection<Product>(nameof(Product));

		col.InsertMany(new List<Product>()
		{
			new Product()
			{
				Id = 1,
				Name = "Name1",
				Price = 10M,
				Type = "Type2"
			},
			new Product()
			{
				Id = 2,
				Name = "Name2",
				Price = 60M,
				Type = "Type2"
			},
			new Product()
			{
				Id = 3,
				Name = "Name3",
				Price = 5M,
				Type = "Type2"
			},
			new Product()
			{
				Id = 4,
				Name = "Name4",
				Price = 40M,
				Type = "Type1"
			},
			new Product()
			{
				Id = 5,
				Name = "Name5",
				Price = 100M,
				Type = "Type2"
			},
			new Product()
			{
				Id = 6,
				Name = "Name6",
				Price = 10M,
				Type = "Type3"
			},
		});
	}
}