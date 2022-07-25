namespace ExpressionTrees.Task2.ExpressionMapping;

public interface ITypeConverter<in TSource, out TDest>
{
    TDest From(TSource source);
}
