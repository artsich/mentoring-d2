/*
*  Create a Task and attach continuations to it according to the following criteria:
   a.    Continuation task should be executed regardless of the result of the parent task.
   b.    Continuation task should be executed when the parent task finished without success.
   c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation
   d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled
   Demonstrate the work of the each case with console utility.
*/
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task6.Continuation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Create a Task and attach continuations to it according to the following criteria:");
            Console.WriteLine("a.    Continuation task should be executed regardless of the result of the parent task.");
            Console.WriteLine("b.    Continuation task should be executed when the parent task finished without success.");
            Console.WriteLine("c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation.");
            Console.WriteLine("d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled.");
            Console.WriteLine("Demonstrate the work of the each case with console utility.");
            Console.WriteLine();

            Process();

            //Console.ReadLine();
        }

        private static void Process()
        {
            void Info()
            {
                Console.WriteLine("Use A, B, C or D key to run the case!");
            }

            Info();

            ConsoleKeyInfo c;
            while((c = Console.ReadKey()).Key != ConsoleKey.Escape)
            {
                Console.WriteLine();
 
                Task task = default;
                switch (c.Key)
                {
                    case ConsoleKey.A:
                        {
                            task = RegardlessCase();
                            break;
                        }
                    case ConsoleKey.B:
                        {
                            WhenParentSuccess();
                            break;
                        }
                    case ConsoleKey.C:
                        {
                            ReuseParentTask();
                            break;
                        }
                    case ConsoleKey.D:
                        {
                            OutsideOfTheThreadPool();
                            break;
                        }
                    default:
                        Info();
                        break;
                }

                task?.Wait();
            }
        }

        private static Task RegardlessCase()
        {
            Console.WriteLine(nameof(RegardlessCase));
            return Task.Run(() =>
            {
                Console.WriteLine("OOps");
                throw new Exception("Something went wrong...");
            }).ContinueWith((t) =>
            {
                Console.WriteLine($"I don't care about my parant task!\n\tParent task info: \n\t\tstatus <{t.Status}>, \n\t\tmsg: {t.Exception.Message}");
            });
        }

        private static Task WhenParentSuccess()
        {
            Console.WriteLine(nameof(WhenParentSuccess));
            return Task.Run(() =>
            {
                Console.WriteLine("I will finish w/o any problem.");
            })
            .ContinueWith((t) =>
            {
                Console.WriteLine("All is well!");
            }, TaskContinuationOptions.NotOnFaulted);
        }

        private static Task ReuseParentTask()
        {
            Console.WriteLine(nameof(ReuseParentTask));

            return Task.Run(() =>
            {
                Console.WriteLine("Thread number: " + Thread.CurrentThread.ManagedThreadId);

                // help check that child is runnig on the same thread
                //
                // Thread.Sleep(3000); 

                throw new Exception("Something went wrong...");
            })
            .ContinueWith((t) =>
            {
                Console.WriteLine("Thread number: " + Thread.CurrentThread.ManagedThreadId);
            },
            TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
        }

        private static Task OutsideOfTheThreadPool()
        {
            Console.WriteLine(nameof(OutsideOfTheThreadPool));

            var ct = new CancellationTokenSource();
            var token = ct.Token;
            ct.Cancel();

            var task = Task.Run(() =>
            {
                Thread.Sleep(1000);
                token.ThrowIfCancellationRequested();
            }, token)
            .ContinueWith((t) => 
            {
                Console.Write($"Parent task status: {t.Status}");
            }, 
            TaskContinuationOptions.OnlyOnCanceled, scheduler: );
            

            return task;
        }
    }

    public class CustomTaskScheduler : TaskScheduler
    {
        protected override IEnumerable<Task> GetScheduledTasks()
        {

        }

        protected override void QueueTask(Task task)
        {

        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {

        }
    }
}
