/*
 * 5. Write a program which creates two threads and a shared collection:
 * the first one should add 10 elements into the collection and the second should print all elements
 * in the collection after each adding.
 * Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.
 */
using System;
using System.Collections.Generic;
using System.Threading;

namespace MultiThreading.Task5.Threads.SharedCollection
{
    class Program
    {
        private static readonly Random Random = new Random();
        private static readonly Semaphore Read = new Semaphore(0, 1);
        private static readonly Semaphore Write = new Semaphore(1, 1);

        static void Main()
        {
            Console.WriteLine("5. Write a program which creates two threads and a shared collection:");
            Console.WriteLine("the first one should add 10 elements into the collection and the second should print all elements in the collection after each adding.");
            Console.WriteLine("Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.");
            Console.WriteLine();

            Process();

            Console.WriteLine("\nPress to finish.");
            Console.ReadLine();
        }

        private static void Process()
        {
            var list = new List<int>();
            var maxSize = 10;

            var t1 = new Thread(() => WriteToArray(list, maxSize));
            var t2 = new Thread(() => ReadFromArray(list, maxSize));

            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();
        }

        private static void ReadFromArray(IList<int> list, int maxPrintCount)
        {
            while (list.Count < maxPrintCount)
            {
                Read.WaitOne();
                try
                {
                    foreach (var item in list)
                    {
                        Console.Write($"{item} ");
                    }
                    Console.WriteLine();
                }
                finally
                {
                    Write.Release();
                }
            }
        }

        private static void WriteToArray(IList<int> list, int size)
        {
            while (list.Count < size)
            {
                Write.WaitOne();
                try
                {
                    list.Add(Random.Next(10));
                }
                finally
                {
                    Read.Release();
                }
            }
        }
    }
}
