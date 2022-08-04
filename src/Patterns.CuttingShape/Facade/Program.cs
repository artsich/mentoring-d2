new Facade().PlaceOrder("Id1", 20, "my@gmail.com");

class Facade
{
	private readonly ProductCatalog productCatalog = new();
	private readonly PaymentSystem paymentSystem = new();
	private readonly InvoiceSystem invoiceSystem = new();

	public void PlaceOrder(string productId, int quantity, string email)
	{
		var product = productCatalog.GetProductDetails(productId);
		var cost = quantity * product.Price;

		if (!paymentSystem.MakePayment(new Payment(productId, cost)))
		{
			throw new InvalidOperationException("Oops, payment error");
		}

		invoiceSystem.SendInvoice(new Invoice(product.Name, quantity, cost, email));
	}
}

public record ProductDetails(string Id, string Name, double Price);
public record Payment(string ProductId, double Cost);
public record Invoice(string ProductName, int Quantity, double Cost, string Email);

class ProductCatalog
{
	public ProductDetails GetProductDetails(string productId)
	{
		return new ProductDetails("Id1", "Milk", 100);
	}
}

class PaymentSystem
{
    public bool MakePayment(Payment payment)
	{
		return true;
	}
}

class InvoiceSystem
{
    public void SendInvoice(Invoice invoice)
	{
		Console.WriteLine($@"
Email: {invoice.Email}
ProductName: {invoice.ProductName}
Quantity: {invoice.Quantity}
Cost: {invoice.Cost}
");

	}
}
