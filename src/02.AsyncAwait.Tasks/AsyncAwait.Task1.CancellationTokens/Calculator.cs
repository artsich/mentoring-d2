using System.Threading;

namespace AsyncAwait.Task1.CancellationTokens;

internal static class Calculator
{
    public static long Calculate(int n, CancellationToken token)
    {
        long sum = 0;

        for (var i = 0; i < n; i++)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            sum += (i + 1);
            Thread.Sleep(10);
        }

        return sum;
    }
}
