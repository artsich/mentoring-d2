/*
*  Create a Task and attach continuations to it according to the following criteria:
   a.    Continuation task should be executed regardless of the result of the parent task.
   b.    Continuation task should be executed when the parent task finished without success.
   c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation
   d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled
   Demonstrate the work of the each case with console utility.
*/
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task6.Continuation
{
    public class CustomTaskScheduler : TaskScheduler
    {
        private readonly BlockingCollection<Task> tasksCollection = new BlockingCollection<Task>();
        private readonly Thread mainThread = null;

        public CustomTaskScheduler()
        {
            mainThread = new Thread(new ThreadStart(Execute));
            mainThread.Start();
        }

        private void Execute()
        {
            foreach (var task in tasksCollection.GetConsumingEnumerable())
            {
                TryExecuteTask(task);
            }
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return tasksCollection.ToArray();
        }

        protected override void QueueTask(Task task)
        {
            if (task != null)
                tasksCollection.Add(task);
        }
        
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return false;
        }

        private void Dispose(bool disposing)
        {
            if (!disposing) return;
            tasksCollection.CompleteAdding();
            tasksCollection.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
