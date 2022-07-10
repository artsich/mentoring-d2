namespace _05.LinqProvider.Models;

public class Product
{
	public int Id { get; set; }

	public string Name { get; set; } = string.Empty;

	public string Type { get; set; } = string.Empty;

	public decimal Price { get; set; }
}
