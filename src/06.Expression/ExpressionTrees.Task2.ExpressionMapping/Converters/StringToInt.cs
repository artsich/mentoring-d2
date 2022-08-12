namespace ExpressionTrees.Task2.ExpressionMapping.Converters;

public class StringToInt : ITypeConverter<string, int>
{
    public int From(string source)
    {
        return int.Parse(source);
    }
}
