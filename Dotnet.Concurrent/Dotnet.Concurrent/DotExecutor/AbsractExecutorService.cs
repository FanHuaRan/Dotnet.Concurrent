using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dotnet.Concurrent.Common;
using Dotnet.Util;

namespace Dotnet.Concurrent.DotExecutor
{
    /// <summary>
    /// 执行服务的抽象实现AbstractExecutorService
    /// 该类实现自ExecutorService的大部分方法，但未实现核心的几个execute,shutdown等方法，
    /// 2018/01/15 fhr
    /// </summary>
    public abstract class AbsractExecutorService : ExecutorService
    {
        public RunnableFuture<T> newTaskFor<T>(Runnable runnable, T value)
        {
            return new FutureTask<T>(runnable, value);
        }

        public RunnableFuture<T> newTaskFor<T>(Callable<T> callable)
        {
            return new FutureTask<T>(callable);
        }

        public Future<T> submit<T>(Runnable task)
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }
            RunnableFuture<T> ftask = newTaskFor<T>(task, default(T));
            return ftask;
        }

        public Future<T> submit<T>(Runnable task, T result) where T : class
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }
            RunnableFuture<T> ftask = newTaskFor(task, result);
            Execute(ftask);
            return ftask;
        }

        /**
         * @throws RejectedExecutionException {@inheritDoc}
         * @throws NullPointerException       {@inheritDoc}
         */
        public Future<T> submit<T>(Callable<T> task) where T : class
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }
            RunnableFuture<T> ftask = newTaskFor(task);
            Execute(ftask);
            return ftask;
        }

        /**
         * the main mechanics of invokeAny.
         */
        private T doInvokeAny<T>(IEnumerable<Callable<T>> tasks,
                                  bool timed, long nanos)
        {
            if (tasks == null)
            {
                throw new NullReferenceException();
            }
            int ntasks = tasks.Count();
            if (ntasks == 0)
            {
                throw new ArgumentException();
            }
            List<Future<T>> futures = new List<Future<T>>(ntasks);
            ExecutorCompletionService<T> ecs = new ExecutorCompletionService<T>(this);

            // For efficiency, especially in executors with limited
            // parallelism, check to see if previously submitted tasks are
            // done before submitting more of them. This interleaving
            // plus the exception mechanics account for messiness of main
            // loop.

            try
            {
                // Record exceptions so that if we fail to obtain any
                // result, we can throw the last exception we got.
                ExecutionException ee = null;
                //const long deadline = timed ? System.nanoTime() + nanos : 0L;
                const long deadline = 0;
                IEnumerator<Callable<T>> it = tasks.GetEnumerator();
                // Start one task for sure; the rest incrementally
                futures.Add(ecs.submit(it.Current));
                it.MoveNext();
                --ntasks;
                int active = 1;

                for (; ; )
                {
                    Future<T> f = ecs.poll();
                    if (f == null)
                    {
                        if (ntasks > 0)
                        {
                            --ntasks;
                            futures.Add(ecs.submit(it.Current));
                            it.MoveNext();
                            ++active;
                        }
                        else if (active == 0)
                            break;
                        else if (timed)
                        {
                            f = ecs.poll(nanos, TimeUnit.NANOSECONDS);
                            if (f == null)
                                throw new TimeoutException();
                            //nanos = deadline - System.nanoTime();
                            nanos = 0;
                        }
                        else
                            f = ecs.take();
                    }
                    if (f != null)
                    {
                        --active;
                        try
                        {
                            return f.get();
                        }
                        catch (ExecutionException eex)
                        {
                            ee = eex;
                        }
                        catch (Exception rex)
                        {
                            ee = new ExecutionException(rex);
                        }
                    }
                }

                if (ee == null)
                    ee = new ExecutionException();
                throw ee;

            }
            finally
            {
                for (int i = 0, size = futures.Count; i < size; i++)
                    futures[i].cancel(true);
            }
        }

        public T invokeAny<T>(IEnumerable<Callable<T>> tasks)
        {
            try
            {
                return doInvokeAny(tasks, false, 0);
            }
            catch (TimeoutException cannotHappen)
            {
                return default(T);
            }
        }

        public T invokeAny<T>(IEnumerable<Callable<T>> tasks,
                               long timeout, TimeUnit unit)
        {
            return doInvokeAny(tasks, true, unit.ToNanos(timeout));
        }

        public List<Future<T>> invokeAll<T>(IEnumerable<Callable<T>> tasks)
        {
            if (tasks == null)
            {
                throw new NullReferenceException();
            }
            List<Future<T>> futures = new List<Future<T>>(tasks.Count());
            bool done = false;
            try
            {
                foreach (var t in tasks)
                {
                    RunnableFuture<T> f = newTaskFor(t);
                    futures.Add(f);
                    Execute(f);
                }
                for (int i = 0, size = futures.Count(); i < size; i++)
                {
                    Future<T> f = futures[i];
                    if (!f.isDone())
                    {
                        try
                        {
                            f.get();
                        }
                        catch (CancellationException ignore)
                        {
                        }
                        catch (ExecutionException ignore)
                        {
                        }
                    }
                }
                done = true;
                return futures;
            }
            finally
            {
                if (!done)
                    for (int i = 0, size = futures.Count(); i < size; i++)
                    {
                        futures[i].cancel(true);
                    }

            }
        }


        public List<Future<T>> invokeAll<T>(IEnumerable<Callable<T>> tasks,
                                             long timeout, TimeUnit unit)
        {
            if (tasks == null)
            {
                throw new NullReferenceException();
            }

            long nanos = unit.ToNanos(timeout);
            List<Future<T>> futures = new List<Future<T>>(tasks.Count());
            bool done = false;
            try
            {
                foreach (var t in tasks)
                {
                    futures.Add(newTaskFor(t));
                }
                //const long deadline = System.nanoTime() + nanos;
                const long deadline = 0;
                int size = futures.Count;

                // Interleave time checks and calls to execute in case
                // executor doesn't have any/much parallelism.
                for (int i = 0; i < size; i++)
                {
                    Execute((Runnable)futures[i]);
                    //nanos = deadline - System.nanoTime();
                    nanos = 0;
                    if (nanos <= 0L)
                        return futures;
                }

                for (int i = 0; i < size; i++)
                {
                    Future<T> f = futures[i];
                    if (!f.isDone())
                    {
                        if (nanos <= 0L)
                            return futures;
                        try
                        {
                            f.get(nanos, TimeUnit.NANOSECONDS);
                        }
                        catch (CancellationException ignore)
                        {
                        }
                        catch (ExecutionException ignore)
                        {
                        }
                        catch (TimeoutException toe)
                        {
                            return futures;
                        }
                        //nanos = deadline - System.nanoTime();
                        nanos = 0;
                    }
                }
                done = true;
                return futures;
            }
            finally
            {
                if (!done)
                    for (int i = 0, size = futures.Count; i < size; i++)
                        futures[i].cancel(true);
            }
        }

        public abstract void Execute(Runnable runable);

        public abstract void Shutdown();

        public abstract List<Runnable> ShutdownNow();

        public abstract bool IsShutdown();

        public abstract bool IsTerminated();

        public abstract bool AwaitTermination(long timeout, TimeUnit unit);

    }
}
