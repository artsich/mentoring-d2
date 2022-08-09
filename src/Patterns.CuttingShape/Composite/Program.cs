public interface IXmlElement
{
	string ConvertToString();
}

public class InputText : IXmlElement
{
	private readonly string name;
	private readonly string value;

	public InputText(string name, string value)
	{
		this.name = name;
		this.value = value;
	}

	public string ConvertToString()
	{
		return $"<inputText name='{name}' value='{value}'/> ";
	}
}

public class LabelText : IXmlElement
{
	private readonly string value;

	public LabelText(string value)
	{
		this.value = value;
	}

	public string ConvertToString()
	{
		return $"<label value='{value}'/>";
	}
}

public class Form : IXmlElement
{
	private readonly string name;
	private readonly List<IXmlElement> elements = new();

	public Form(string name)

	{
		this.name = name;
	}

	public Form AddComponent(IXmlElement element)
	{
		elements.Add(element ?? throw new ArgumentNullException(nameof(element)));
		return this;
	}

	public string ConvertToString()
	{
		var body = "\t" + string.Join("\n\t", elements.Select(x => x.ConvertToString()));

		return $@"
<form name='{name}'>
{body}
</form>";
	}
}

class Program
{
	static void Main()
	{
		var xml = new Form("login")
			.AddComponent(new LabelText("Password"))
			.AddComponent(new InputText("password", "12345"))
			.AddComponent(new LabelText("Login"))
			.AddComponent(new InputText("login", "qwerty"))
			.ConvertToString();

		Console.WriteLine(xml);
	}
}