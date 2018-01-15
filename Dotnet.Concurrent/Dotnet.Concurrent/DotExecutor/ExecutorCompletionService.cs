using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dotnet.Concurrent.Common;
using Dotnet.Concurrent.Container;
using Dotnet.Util;

namespace Dotnet.Concurrent.DotExecutor
{
    /// <summary>
    /// ExecutorCompletionService是对CompletionService的实现
    /// 2018/01/15 fhr
    /// </summary>
    /// <typeparam name="V"></typeparam>
    public class ExecutorCompletionService<V> : CompletionService<V>
    {
        /// <summary>
        /// 被包装Executor
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

        /// <summary>
        /// Creates an ExecutorCompletionService using the supplied executor for base task execution 
        /// </summary>
        /// <param name="executor"></param>
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

        /// <summary>
        /// Creates an ExecutorCompletionService using the supplied
        /// executor for base task execution and the supplied queue as its completion queue.
        /// </summary>
        /// <param name="executor"></param>
        /// <param name="completionQueue"></param>
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

        /// <summary>
        /// 包装callable
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private  RunnableFuture<V> newTaskFor(Callable<V> task)
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

        /// <summary>
        /// 包装runnable
        /// </summary>
        /// <param name="task"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private  RunnableFuture<V> newTaskFor(Runnable task, V result)
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

        /// <summary>
        /// 提交任务
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public  Future<V> Submit(Callable<V> task)
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }
            RunnableFuture<V> f = newTaskFor(task);
            executor.Execute(new QueueingFuture(f, completionQueue));
            return f;
        }

        /// <summary>
        /// 提交任务
        /// </summary>
        /// <param name="task"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public Future<V> Submit(Runnable task, V result)
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }
            RunnableFuture<V> f = newTaskFor(task, result);
            executor.Execute(new QueueingFuture(f, completionQueue));
            return f;
        }

        /// <summary>
        /// 获取一个执行结果（并移除） 阻塞
        /// </summary>
        /// <returns></returns>
        public Future<V> Take()
        {
            return completionQueue.take();
        }

        /// <summary>
        /// 获取一个执行结果（并移除） 不阻塞
        /// </summary>
        /// <returns></returns>
        public Future<V> Poll()
        {
            return completionQueue.poll();
        }

        /// <summary>
        /// 获取一个执行结果（并移除） 带超时
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public Future<V> Poll(long timeout,TimeUnit unit)
        {
            return completionQueue.poll(timeout,unit);
        }

        /// <summary>
        /// 扩展FutureTask，重写了其done方法用于将完成任务放置在阻塞队列！！！
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
