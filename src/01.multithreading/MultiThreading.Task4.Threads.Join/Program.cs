/*
 * 4.	Write a program which recursively creates 10 threads.
 * Each thread should be with the same body and receive a state with integer number, decrement it,
 * print and pass as a state into the newly created thread.
 * Use Thread class for this task and Join for waiting threads.
 * 
 * Implement all of the following options:
 * - a) Use Thread class for this task and Join for waiting threads.
 * - b) ThreadPool class for this task and Semaphore for waiting threads.
 */

using System;
using System.Threading;

namespace MultiThreading.Task4.Threads.Join
{
    class Program
    {
        private static Semaphore _pool;

        static void Main(string[] args)
        {
            Console.WriteLine("4.	Write a program which recursively creates 10 threads.");
            Console.WriteLine("Each thread should be with the same body and receive a state with integer number, decrement it, print and pass as a state into the newly created thread.");
            Console.WriteLine("Implement all of the following options:");
            Console.WriteLine();
            Console.WriteLine("- a) Use Thread class for this task and Join for waiting threads.");
            Console.WriteLine("- b) ThreadPool class for this task and Semaphore for waiting threads.");

            Console.WriteLine();

            Process();

            Console.ReadLine();
        }

        static void Process()
        {
            Console.WriteLine("Thread:");
            ProcessThread(10);

            Console.WriteLine("\n\n---------------------\n\n");
            Console.WriteLine("Thread pool");

            _pool = new Semaphore(0, 1);
            ProcessThreadPool(20);

            _pool.WaitOne();
            Console.WriteLine("Finish.");
        }

        static void ProcessThread(int state)
        {
            if (state <= 0) return;
            state--;
            Console.WriteLine(state);

            var thread = new Thread(() => ProcessThread(state));
            thread.Start();
            thread.Join();
        }

        static void ProcessThreadPool(object state)
        {
            var number = (int)state;
            if (number <= 0)
            {
                _pool.Release();
                return;
            }

            number--;
            Console.WriteLine(number);

            ThreadPool.QueueUserWorkItem(ProcessThreadPool, number);
        }
    }
}
