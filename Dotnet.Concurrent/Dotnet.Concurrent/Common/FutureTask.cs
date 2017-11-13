using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Dotnet.Concurrent.Util;
using Dotnet.Concurrent.Common;

namespace Dotnet.Concurrent.Common
{
    /// <summary>
    /// RunnableFuture的重要实现
    /// 2017/11/13 fhr
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FutureTask<T> : RunnableFuture<T>
    {
        //状态常量
        private static readonly int NEW = 0;
        private static readonly int COMPLETING = 1;
        private static readonly int NORMAL = 2;
        private static readonly int EXCEPTIONAL = 3;
        private static readonly int CANCELLED = 4;
        private static readonly int INTERRUPTING = 5;
        private static readonly int INTERRUPTED = 6;
        //当前状态
        private volatile int state;
        //被包裹的callable
        private Callable<T> callable;
        //结果值 non-volatile, protected by state reads/writes
        private Object outcome;
        //运行线程
        private volatile Thread runner;
        //下一个等待节点
        private volatile WaitNode waiters;

        public FutureTask(Callable<T> callable)
        {
            if (callable == null)
            {
                throw new NullReferenceException();
            }
            this.callable = callable;
            // ensure visibility of callable
            this.state = NEW;
        }
        
        public FutureTask(Runnable runnable, T result)
        {
            this.callable = RunableAdapters.Callable<T>(runnable, result);
            // ensure visibility of callable
            this.state = NEW;      
        }

        /// <summary>
        /// 获取结果 带最后的取消和执行检查
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
         
        private T report(int s)
        {
            Object x = outcome;
            if (s == NORMAL)
            {
                return (T)x;
            }
            if (s >= CANCELLED)
            {
                throw new CancellationException();
            }
            throw new ExecutionException((Exception)x);
        }
       
        /// <summary>
        /// 是否已经取消
        /// </summary>
        /// <returns></returns>
        public bool isCancelled()
        {
            return state >= CANCELLED;
        }
        /// <summary>
        /// 是否已经在做，已经做完，异常
        /// </summary>
        /// <returns></returns>
        public bool isDone()
        {
            return state != NEW;
        }
        /// <summary>
        /// 取消当前任务 注意这儿可能取消也可能中断
        /// </summary>
        /// <param name="mayInterruptIfRunning">是否中断</param>
        /// <returns></returns>
        public bool cancel(bool mayInterruptIfRunning)
        {
            if (!(state == NEW && Interlocked.CompareExchange(ref state, mayInterruptIfRunning ? INTERRUPTING : CANCELLED, NEW) == NEW))
            {
                return false;
            }
            try
            {    // in case call to interrupt throws exception
                if (mayInterruptIfRunning)
                {
                    try
                    {
                        Thread t = runner;
                        if (t != null)
                        {
                            t.Interrupt();
                        }
                    }
                    finally
                    { // final state
                        //没有像java一样防止指令重排序 不知道有没有问题！
                        this.state = INTERRUPTED;
                    }
                }
            }
            finally
            {
                finishCompletion();
            }
            return true;
        }
        /// <summary>
        /// 阻塞获取值 
        /// </summary>
        /// <returns></returns>
        public T get()
        {
            int s = state;
            if (s <= COMPLETING)
            {
                s = awaitDone(false, 0L);
            }
            return report(s);
        }
        
        /// <summary>
        /// 获取值 带超时机制
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public T get(long timeout)
        {
            int s = state;
            if (s <= COMPLETING && (s = awaitDone(true, timeout)) <= COMPLETING)
            {
                throw new TimeoutException();
            }
            return report(s);
        }
        /// <summary>
        /// 不知道有什么用
        /// </summary>
        protected void done() { }
        /// <summary>
        /// 设置结果为预期的运行结果
        /// </summary>
        /// <param name="v"></param>
        protected void set(T v)
        {
            if (Interlocked.CompareExchange(ref state, COMPLETING, NEW) == NEW)
            {
                outcome = v;
                //UNSAFE.putOrderedInt(this, stateOffset, NORMAL); // final state
                //没有防止指令重排序
                this.state = NORMAL;
                finishCompletion();
            }
        }
        /// <summary>
        /// 设置结果为异常
        /// </summary>
        /// <param name="t"></param>
        protected void setException(Exception t)
        {
            if (Interlocked.CompareExchange(ref state, COMPLETING, NEW) == NEW)
            {
                outcome = t;
                //UNSAFE.putOrderedInt(this, stateOffset, EXCEPTIONAL); // final state
                //没有防止指令重排序
                this.state = EXCEPTIONAL;
                finishCompletion();
            }
        }
        /// <summary>
        /// 运行任务 核心！！
        /// </summary>
        public void run()
        {
            if (state != NEW || !(Interlocked.CompareExchange<Thread>(ref runner, Thread.CurrentThread, null) == null))
            {
                return;
            }
            try
            {
                Callable<T> c = callable;
                if (c != null && state == NEW)
                {
                    T result;
                    bool ran;
                    try
                    {
                        result = c.call();
                        ran = true;
                    }
                    catch (Exception ex)
                    {
                        result = default(T);
                        ran = false;
                        setException(ex);
                    }
                    if (ran)
                    {
                        set(result);
                    }
                }
            }
            finally
            {
                // runner must be non-null until state is settled to
                // prevent concurrent calls to run()
                runner = null;
                // state must be re-read after nulling runner to prevent
                // leaked interrupts
                int s = state;
                if (s >= INTERRUPTING)
                {
                    handlePossibleCancellationInterrupt(s);
                }
            }
        }
        /// <summary>
        /// 运行然后重置 异常就终止 
        /// 是否可重用的区别
        /// </summary>
        /// <returns></returns>
        protected bool runAndReset()
        {
            if (state != NEW || !(Interlocked.CompareExchange<Thread>(ref runner, Thread.CurrentThread, null) == null))
            {
                return false;
            }
            bool ran = false;
            int s = state;
            try
            {
                Callable<T> c = callable;
                if (c != null && s == NEW)
                {
                    try
                    {
                        c.call(); // don't set result
                        ran = true;
                    }
                    catch (Exception ex)
                    {
                        setException(ex);
                    }
                }
            }
            finally
            {
                // runner must be non-null until state is settled to
                // prevent concurrent calls to run()
                runner = null;
                // state must be re-read after nulling runner to prevent
                // leaked interrupts
                s = state;
                if (s >= INTERRUPTING)
                {
                    handlePossibleCancellationInterrupt(s);
                }
            }
            return ran && s == NEW;
        }
        /// <summary>
        /// 正在中断的处理 实际上就是一直让出cpu
        /// </summary>
        /// <param name="s"></param>
        private void handlePossibleCancellationInterrupt(int s)
        {
            // It is possible for our interrupter to stall before getting a
            // chance to interrupt us.  Let's spin-wait patiently.
            if (s == INTERRUPTING)
            {
                while (state == INTERRUPTING)
                {
                    Thread.Yield(); // wait out pending interrupt
                }
            }
        }
        /**
        * 结束完成的相关善后工作
        */
        private void finishCompletion()
        {
            // assert state > COMPLETING;
            for (WaitNode q; (q = waiters) != null; )
            {
                if (Interlocked.CompareExchange<WaitNode>(ref waiters, null, q) == q)
                {
                    for (; ; )
                    {
                        Thread t = q.thread;
                        if (t != null)
                        {
                            q.thread = null;
                            LockSupport.unpark(t);
                        }
                        WaitNode next = q.next;
                        if (next == null)
                        {
                            break;
                        }
                        q.next = null; // unlink to help gc
                        q = next;
                    }
                    break;
                }
            }
            done();
            callable = null;        // to reduce footprint
        }
        /// <summary>
        /// 等待任务完成 带超时机制
        /// </summary>
        /// <param name="timed"></param>
        /// <param name="nanos"></param>
        /// <returns></returns>
        private int awaitDone(bool timed, long nanos)
        {
            long deadline = timed ? DateTime.Now.Ticks + nanos : 0L;
            WaitNode q = null;
            bool queued = false;
            for (; ; )
            {
                //if (Thread.interrupted()) {
                //    removeWaiter(q);
                //    throw new ThreadInterruptedException();
                //}
                int s = state;
                if (s > COMPLETING)
                {
                    if (q != null)
                    {
                        q.thread = null;
                    }
                    return s;
                }
                else if (s == COMPLETING)// cannot time out yet
                {
                    Thread.Yield();
                }
                else if (q == null)
                {
                    q = new WaitNode();
                }
                else if (!queued)
                {
                    queued = Interlocked.CompareExchange<WaitNode>(ref waiters, q, q.next = waiters) == q.next;
                }
                else if (timed)
                {
                    nanos = deadline - DateTime.Now.Ticks;
                    if (nanos <= 0L)
                    {
                        removeWaiter(q);
                        return state;
                    }
                    LockSupport.parkNanos(this, nanos);
                }
                else
                    LockSupport.park(this);
            }
        }
        /// <summary>
        /// 移除等待节点
        /// </summary>
        /// <param name="node"></param>
        private void removeWaiter(WaitNode node)
        {
            if (node != null)
            {
                node.thread = null;
            retry:
                for (; ; )
                {          // restart on removeWaiter race
                    for (WaitNode pred = null, q = waiters, s; q != null; q = s)
                    {
                        s = q.next;
                        if (q.thread != null)
                            pred = q;
                        else if (pred != null)
                        {
                            pred.next = s;
                            if (pred.thread == null) // check for race
                                goto retry;
                        }
                        if (!(Interlocked.CompareExchange<WaitNode>(ref waiters, s, q) == q))
                        {
                            goto retry;
                        }
                    }
                    break;
                }
            }
        }
        /// <summary>
        /// 等待节点
        /// </summary>
        internal sealed class WaitNode
        {
            internal volatile Thread thread;
            internal volatile WaitNode next;
            internal WaitNode() { thread = Thread.CurrentThread; }
        }
    }
}
