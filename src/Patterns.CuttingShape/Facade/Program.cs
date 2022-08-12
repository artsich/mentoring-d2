new OrderSysem(
	new ProductCatalog(),
	new PaymentSystem(),
	new InvoiceSystem())
.PlaceOrder("Id1", 20, "my@gmail.com");

class OrderSysem
{
	private readonly IProductCatalog productCatalog;
	private readonly IPaymentSystem paymentSystem;
	private readonly IInvoiceSystem invoiceSystem;

	public OrderSysem(IProductCatalog productCatalog, IPaymentSystem paymentSystem, IInvoiceSystem invoiceSystem)
	{
		this.productCatalog = productCatalog;
		this.paymentSystem = paymentSystem;
		this.invoiceSystem = invoiceSystem;
	}

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

interface IProductCatalog
{
	ProductDetails GetProductDetails(string productId);
}

interface IPaymentSystem
{
	bool MakePayment(Payment payment);
}

interface IInvoiceSystem
{
	void SendInvoice(Invoice invoice);
}

public class ProductCatalog : IProductCatalog
{
	public ProductDetails GetProductDetails(string productId) =>
		new("Id1", "Milk", 100);
}

public class PaymentSystem : IPaymentSystem
{
	public bool MakePayment(Payment payment) => true;
}

public class InvoiceSystem : IInvoiceSystem
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
