using System.Threading;

namespace AsyncAwait.Task1.CancellationTokens;

internal static class Calculator
{
    // todo: change this method to support cancellation token
    public static long Calculate(int n, CancellationToken token)
    {
        long sum = 0;

        for (var i = 0; i < n; i++)
        {
            if (token.IsCancellationRequested)
            {
                // or need to return result?
                token.ThrowIfCancellationRequested();
            }

            // i + 1 is to allow 2147483647 (Max(Int32)) 
            sum += (i + 1);
            Thread.Sleep(10);
        }

        return sum;
    }
}
