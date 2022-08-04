public class Printer
{
	public void Print<T>(IContainer<T> container)
	{
		foreach (var item in container.Items)
		{
			Console.WriteLine(item?.ToString());
		}
	}
}

public interface IContainer<T>
{
	IEnumerable<T> Items { get; }

	int Count { get; }
}

public interface IElements<T>

{
	IEnumerable<T> GetElements();
}

public class ElementsToContainerAdapter<T> : IContainer<T>
{
	private readonly IElements<T> origin;

	public ElementsToContainerAdapter(IElements<T> origin)
	{
		this.origin = origin;
	}

	public IEnumerable<T> Items => origin.GetElements();

	public int Count => Items.Count();
}

public class Elements<T> : IElements<T>
{
	private readonly IEnumerable<T> _items;

	public Elements(IEnumerable<T> items)
	{
		_items = items;
	}

	public IEnumerable<T> GetElements() => _items;
}

internal class Program
{
	static void Main()
	{
		var elements = new Elements<string>(new[] { "one", "two", "three" });

		var printer = new Printer();
		printer.Print(new ElementsToContainerAdapter<string>(elements));
	}
}