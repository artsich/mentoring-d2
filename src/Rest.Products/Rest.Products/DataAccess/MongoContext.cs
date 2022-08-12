using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Rest.Products.DataAccess;

public class DatabaseSetting
{
    public string ConnectionString { get; set; } = "";

    public string Name { get; set; } = "";

    public string[] Collections { get; set; } = Array.Empty<string>();
}

public interface IMongoContext
{
    IMongoCollection<T> Collection<T>();
}

public class MongoContext : IMongoContext
{
    private readonly IOptions<DatabaseSetting> _settings;
    private readonly IMongoDatabase _database;

    public MongoContext(IOptions<DatabaseSetting> settings)
    {
        _settings = settings;
        var mongoSettings = MongoClientSettings.FromUrl(
            new MongoUrl(settings.Value.ConnectionString ?? throw new ArgumentException(nameof(settings.Value.ConnectionString)))
        );
        var mongoClient = new MongoClient(mongoSettings);
        _database = mongoClient.GetDatabase(settings.Value.Name);
    }

    public IMongoCollection<T> Collection<T>()
    {
        return _database.GetCollection<T>(typeof(T).Name);
    }
}
