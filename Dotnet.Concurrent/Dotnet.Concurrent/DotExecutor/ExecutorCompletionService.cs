using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dotnet.Concurrent.Common;
using Dotnet.Concurrent.Container;

namespace Dotnet.Concurrent.DotExecutor
{
    /// <summary>
    /// 任务队列集合处理实现 
    /// 2017/11/13 fhr
    /// </summary>
    /// <typeparam name="V"></typeparam>
    public class ExecutorCompletionService<V> : CompletionService<V> where V : class
    {
        /// <summary>
        /// 包装的线程池服务
        /// </summary>
        private readonly Executor executor;
        /// <summary>
        /// 抽象线程池服务 如果能够转换的化
        /// </summary>
        private readonly AbsractExecutorService aes;
        /// <summary>
        /// 已经完成的任务队列集合
        /// </summary>
        private readonly BlockingQueue<Future<V>> completionQueue;

        /**
         * Creates an ExecutorCompletionService using the supplied executor for base task execution 
         * 
         */
        public ExecutorCompletionService(Executor executor)
        {
            if (executor == null)
            {
                throw new NullReferenceException();
            }
            this.executor = executor;
            this.aes = (executor is AbsractExecutorService) ? (AbsractExecutorService)executor : null;
            this.completionQueue = new LinkedBlockingQueue<Future<V>>();
        }

        /**
         * Creates an ExecutorCompletionService using the supplied
         * executor for base task execution and the supplied queue as its completion queue.
         */
        public ExecutorCompletionService(Executor executor,
                                         BlockingQueue<Future<V>> completionQueue)
        {
            if (executor == null || completionQueue == null)
            {
                throw new NullReferenceException();
            }
            this.executor = executor;
            this.aes = (executor is AbsractExecutorService) ? (AbsractExecutorService)executor : null;
            this.completionQueue = completionQueue;
        }

        private RunnableFuture<V> newTaskFor(Callable<V> task)
        {
            if (aes == null)
            {
                return new FutureTask<V>(task);
            }
            else
            {
                return aes.newTaskFor(task);
            }
        }

        private RunnableFuture<V> newTaskFor(Runnable task, V result)
        {
            if (aes == null)
            {
                return new FutureTask<V>(task, result);
            }
            else
            {
                return aes.newTaskFor(task, result);
            }
        }

        public Future<V> submit(Callable<V> task)
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }
            RunnableFuture<V> f = newTaskFor(task);
            executor.Execute(new QueueingFuture(f, completionQueue));
            return f;
        }

        public Future<V> submit(Runnable task, V result)
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }
            RunnableFuture<V> f = newTaskFor(task, result);
            executor.Execute(new QueueingFuture(f, completionQueue));
            return f;
        }

        public Future<V> take()
        {
            return completionQueue.take();
        }

        public Future<V> poll()
        {
            return completionQueue.poll();
        }
        
        public Future<V> poll(long timeout)
        {
            return completionQueue.poll(timeout);
        }

        /// <summary>
        /// FutureTask extension to enqueue upon completion
        /// 包含完成任务队列记录的Future task
        /// </summary>
        private class QueueingFuture : FutureTask<V>
        {
            public QueueingFuture(RunnableFuture<V> task, BlockingQueue<Future<V>> completionQueue)
                : base(task, default(V))
            {
                this.task = task;
            }
            protected void done()
            {
                completionQueue.add(task);
            }
            private readonly Future<V> task;
            private readonly BlockingQueue<Future<V>> completionQueue;
        }

    }
}
