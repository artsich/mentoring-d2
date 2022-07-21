/*
 * Create a class based on ExpressionVisitor, which makes expression tree transformation:
 * 1. converts expressions like <variable> + 1 to increment operations, <variable> - 1 - into decrement operations.
 * 2. changes parameter values in a lambda expression to constants, taking the following as transformation parameters:
 *    - source expression;
 *    - dictionary: <parameter name: value for replacement>
 * The results could be printed in console or checked via Debugger using any Visualizer.
 */

using System;
using System.Linq.Expressions;

namespace ExpressionTrees.Task1.ExpressionsTransformer;

static class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Expression Visitor for increment/decrement.");
        Console.WriteLine();

        Expression<Func<int, int>> exprInc = i => i + 1;
        Expression<Func<int, int>> exprDec = i => i - 1;

        var visitor = new IncDecExpressionVisitor();

        Console.WriteLine($"Inc: {visitor.Visit(exprInc)}");
        Console.WriteLine($"Dec: {visitor.Visit(exprDec)}");

        var value = 10;
        Console.WriteLine($"value + 1 = {exprInc.Compile().Invoke(value)}");
        Console.WriteLine($"value - 1 = {exprDec.Compile().Invoke(value)}");
    }
}