/*
 * 2.	Write a program, which creates a chain of four Tasks.
 * First Task – creates an array of 10 random integer.
 * Second Task – multiplies this array with another random integer.
 * Third Task – sorts this array by ascending.
 * Fourth Task – calculates the average value. All this tasks should print the values to console.
 */
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MultiThreading.Task2.Chaining
{
    interface IPrinter<T>
    {
        void Print(T data, string tag);
    }

    class ArrayPrinter : IPrinter<int[]>
    {
        public void Print(int[] data, string tag)
        {
            Console.WriteLine(tag);

            foreach (var item in data)
            {
                Console.Write(item);
                Console.Write(" ");
            }

            Console.WriteLine();
        }
    }

    class DoublePrinter : IPrinter<double>
    {
        public void Print(double data, string tag)
        {
            Console.WriteLine($"{tag}: {data}");
        }
    }

    class Program
    {
        private static readonly Random Random = new Random();
        private static readonly ArrayPrinter ArrayPrinter = new ArrayPrinter();
        private static readonly DoublePrinter DoublePrinter = new DoublePrinter();

        static void Main()
        {
            Console.WriteLine(".Net Mentoring Program. MultiThreading V1 ");
            Console.WriteLine("2.	Write a program, which creates a chain of four Tasks.");
            Console.WriteLine("First Task – creates an array of 10 random integer.");
            Console.WriteLine("Second Task – multiplies this array with another random integer.");
            Console.WriteLine("Third Task – sorts this array by ascending.");
            Console.WriteLine("Fourth Task – calculates the average value. All this tasks should print the values to console");
            Console.WriteLine();


            Process().Wait();

            Console.ReadLine();
        }

        private static async Task Process()
        {
            var array = await WithPrint(GenerateIntArray(10, 20), ArrayPrinter, "Array generated.");

            var multipliedArray = await WithPrint(Multiply(array, multiplier: 10), ArrayPrinter, "Array multiplied.");

            var sorted = await WithPrint(Sort(multipliedArray), ArrayPrinter, "Array sorted.");

            await WithPrint(Average(sorted), DoublePrinter, "Average");
        }

        private static Task<double> Average(int[] data) 
        {
            return Task.Run(() => data.Average());
        }

        private static Task<int[]> Sort(int[] data)
        {
            return Task.Run(() => data.OrderBy(x => x).ToArray());
        }

        private static Task<int[]> Multiply(int[] data, int multiplier)
        {
            return Task.Run(() =>
                data.Select(x => x * multiplier).ToArray()
            );
        }

        private static Task<int[]> GenerateIntArray(int num, int maxValue)
        {
            return Task.Run(() =>
                Enumerable.Range(0, num)
                    .Select(x => Random.Next(maxValue))
                    .ToArray()
            );
        }

        private static async Task<T> WithPrint<T>(Task<T> task, IPrinter<T> printer, string tag)
        {
            var result = await task;
            printer.Print(result, tag);
            return result;
        }
    }
}
