using FluentValidation.AspNetCore;
using Rest.Products.DataAccess;
using Rest.Products.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddTransient<MongoContext>();
builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<ICategoryService, CategoryService>();

builder.Services.AddControllers();

builder.Services.AddFluentValidation(s =>
{
    s.RegisterValidatorsFromAssemblyContaining<Program>();
});

// Add settings
builder.Services.Configure<DatabaseSetting>(
    builder.Configuration.GetSection(nameof(DatabaseSetting)));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
