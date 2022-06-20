/*
* Study the code of this application to calculate the sum of integers from 0 to N, and then
* change the application code so that the following requirements are met:
* 1. The calculation must be performed asynchronously.
* 2. N is set by the user from the console. The user has the right to make a new boundary in the calculation process,
* which should lead to the restart of the calculation.
* 3. When restarting the calculation, the application should continue working without any failures.
*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwait.Task1.CancellationTokens;

internal class ConsoleNumberReader
{
    private readonly List<char> buff = new();

    public bool TryGetNumber(out int value)
    {
        value = int.MinValue;

        if (!Console.KeyAvailable)
            return false;

        var key = Console.ReadKey();

        if (char.IsDigit(key.KeyChar))
        {
            buff.Add(key.KeyChar);
        }
        else if (key.Key == ConsoleKey.Enter)
        {
            var span = CollectionsMarshal.AsSpan(buff);
            value = int.Parse(new string(span));
            buff.Clear();

            return true;
        }

        return false;
    }
}

internal class Program
{
    /// <summary>
    /// The Main method should not be changed at all.
    /// </summary>
    /// <param name="args"></param>
    private static void Main(string[] args)
    {
        Console.WriteLine("Mentoring program L2. Async/await.V1. Task 1");
        Console.WriteLine("Calculating the sum of integers from 0 to N.");
        Console.WriteLine("Use 'q' key to exit...");
        Console.WriteLine();

        Console.WriteLine("Enter N: ");

        var input = Console.ReadLine();
        while (input.Trim().ToUpper() != "Q")
        {
            if (int.TryParse(input, out var n))
            {
                CalculateSum(n);
            }
            else
            {
                Console.WriteLine($"Invalid integer: '{input}'. Please try again.");
                Console.WriteLine("Enter N: ");
            }

            input = Console.ReadLine();
        }

        Console.WriteLine("Press any key to continue");
        Console.ReadLine();
    }

    private static Task TryStopCalculation(CancellationToken inputCt, Action<int> onRead)
    {
        return Task.Run(() =>
        {
            var reader = new ConsoleNumberReader();
            while (!inputCt.IsCancellationRequested)
            {
                if (reader.TryGetNumber(out var value))
                {
                    onRead?.Invoke(value);
                    break;
                }
            }
        },
        inputCt);
    }

    private static void CalculateSum(int n)
    {
        Task.Run(async () =>
        {
            var finised = false;
            var processingNubmer = n;
            var newProcessingNumber = n;
            while (!finised)
            {
                processingNubmer = newProcessingNumber;

                var calculationTokenSource = new CancellationTokenSource();
                var inputCts = new CancellationTokenSource();

                var sumTask = Calculate(processingNubmer, calculationTokenSource.Token);

                Console.WriteLine($"The task for {processingNubmer} started... Enter N to cancel the request:");

                _ = TryStopCalculation(inputCts.Token, (value) =>
                {
                    calculationTokenSource.Cancel();
                    newProcessingNumber = value;
                });

                try
                {
                    var sum = await sumTask;
                    Console.WriteLine($"Sum for {processingNubmer} = {sum}.");
                    finised = true;
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine($"Sum for {processingNubmer} cancelled...");
                }

                inputCts.Cancel();
            }
        }).GetAwaiter().GetResult();
    }

    private static Task<long> Calculate(int n, CancellationToken ct)
    {
        return Task.Run(() => Calculator.Calculate(n, ct), ct);
    }
}
