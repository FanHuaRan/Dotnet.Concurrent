using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dotnet.Concurrent.Util;

namespace Dotnet.Concurrent.Locks
{
    /// <summary>
    /// 抽象队列同步器 库核心
    /// 2017/10/19
    /// </summary>
    [Serializable]
    public abstract class AbstractQueuedSynchronizer : AbstractOwnableSynchronizer
    {
       static readonly long spinForTimeoutThreshold = 1000L;

        /**
         * not instance
         */
        protected AbstractQueuedSynchronizer()
        {

        }
        #region 相关属性
        /// <summary>
        /// 队列头部节点 头部节点要么是个空节点  要么就是一个已经得到了资源的节点
        /// </summary>
        [NonSerialized]
        protected  Node head;
        [NonSerialized]
        protected  Node Head
        {
            get
            {
                return this.head;
            }
            set
            {
                //初始化操作
                value.Next = null;
                value.Prev = null;
                this.head = value;
            }
        }
        /// <summary>
        /// 队列尾部节点
        /// </summary>
        [NonSerialized]
        protected  Node Tail
        {
            get;
            set;
        }
        //同步器中所拥有的资源 核心
        [NonSerialized]
        private volatile int state;
        [NonSerialized]
        protected int State
        {
            get { return state; }
            set { this.state = value; }
        }
        #endregion
        #region Utils
        /// <summary>
        /// 节点入队 通过自旋+CAS 返回前面的节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private Node Enq(Node mode)
        {
            Node node = new Node(Thread.CurrentThread, mode);
            for (; ; )
            {
                Node t = Tail;
                //初始化头部
                if (t == null)
                {
                    if (CompareAndSetHead(new Node()))
                    {
                        Tail = Head;
                    }
                }
                else
                {
                    node.Prev = t;
                    if (CompareAndSetTail(t, node))
                    {
                        t.Next = node;
                        return t;
                    }
                }
            }
        }
        /// <summary>
        /// 添加等待节点
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        private Node AddWaiter(Node mode)
        {
            Node node = new Node(Thread.CurrentThread, mode);
            //先尝试快速入队
            Node pred = Tail;
            if (pred != null)
            {
                node.Prev = pred;
                if (CompareAndSetTail(pred, node))
                {
                    pred.Next = node;
                    return node;
                }
            }
            //快速入队失败则老老实实添加到队列尾部
            Enq(node);
            return node;
        }
        /// <summary>
        /// 唤醒下一个等待线程
        /// </summary>
        /// <param name="node"></param>
        private void UnparkSuccessor(Node node)
        {
            int ws = node.WaitStatus;
            if (ws < 0)
            {
                CompareAndSetWaitStatus(node, ws, 0);
            }
            Node s = node.Next;
            //节点为null或者已经取消 则寻找下一个节点
            if (s == null || s.WaitStatus > 0)
            {
                s = null;
                //从后向前找最前面可以被唤醒的节点 不知道为什么不从前往后找
                for (Node t = Tail; t != null && t != node; t = t.Prev)
                {
                    if (t.WaitStatus <= 0)
                    {
                        s = t;
                    }
                }
            }
            //唤醒该节点线程
            if (s != null)
            {
                LockSupport.unpark(s.Thread);
            }
        }
        /// <summary>
        /// 释放共享资源
        /// </summary>
        private void doReleaseShared()
        {
            for(;;){
                Node h = head;
                if (h != null && h != Tail)
                {
                    int ws = h.WaitStatus;
                    //节点如果等待被唤醒
                    if (ws == Node.SIGNAL)
                    {
                        if (!CompareAndSetWaitStatus(h, Node.SIGNAL, 0))
                        {
                            continue;
                        }
                        //唤醒该节点
                        UnparkSuccessor(h);
                    }
                    //节点如果已经被唤醒且不带传播行为
                    else if(ws==0&&!CompareAndSetWaitStatus(h,0,Node.PROPAGATE)){
                        continue;
                    }
                }
                //如果头部未改变 则跳出循环
                if(h==head){
                    break;
                }
            }
        }
        /// <summary>
        /// 设置头部节点并且如果是共享模式传播释放资源
        /// </summary>
        /// <param name="node"></param>
        /// <param name="propagate"></param>
        private void setHeadAndPropagate(Node node, int propagate)
        {
            // Record old head for check below
            Node h = head;
            //设置头部节点
            Head = node;
            if (propagate > 0 || h == null || h.WaitStatus < 0 || (h = head) == null || h.WaitStatus < 0)
            {
                Node s = node.Next;
                if (s == null || s.IsShared)
                {
                    doReleaseShared();
                }
            }
        }
        /// <summary>
        /// 取消请求资源
        /// </summary>
        /// <param name="node"></param>
        private void cancelAcquire(Node node)
        {
            // Ignore if node doesn't exist
            if (node == null)
            {
                return;
            }
            node.Thread = null;
            // Skip cancelled predecessors
            Node pred = node.Prev;
            while (pred.WaitStatus > 0)
            {
                node.Prev = pred = pred.Prev;
            }
            // predNext is the apparent node to unsplice. CASes below will
            // fail if not, in which case, we lost race vs another cancel
            // or signal, so no further action is necessary.
            Node predNext = pred.Next;
            // Can use unconditional write instead of CAS here.
            // After this atomic step, other Nodes can skip past us.
            // Before, we are free of interference from other threads.
            node.WaitStatus = Node.CANCELLED;
            // If we are the tail, remove ourselves.
            if (node == Tail && CompareAndSetTail(node, pred))
            {
                CompareAndSetNext(pred, predNext, null);
            }
            else
            {
                // If successor needs signal, try to set pred's next-link
                // so it will get one. Otherwise wake it up to propagate.
                int ws;
                if (pred != head && ((ws = pred.WaitStatus) == Node.SIGNAL || (ws <= 0 && CompareAndSetWaitStatus(pred, ws, Node.SIGNAL))) && pred.Thread != null)
                {
                    Node next = node.Next;
                    if (next != null && next.WaitStatus <= 0)
                    {
                        CompareAndSetNext(pred, predNext, next);
                    }
                }
                else
                {
                    UnparkSuccessor(node);//是第一个节点就唤醒后续节点？
                }
                node.Next = node; // help GC
            }
        }

        /// <summary>
        /// 判断线程是否可以安全休眠等待
        /// </summary>
        /// <param name="pred"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool shouldParkAfterFailedAcquire(Node pred, Node node)
        {
            int ws = pred.WaitStatus;
            if (ws == Node.SIGNAL)
            {
                /*
                 * This node has already set status asking a release
                 * to signal it, so it can safely park.
                 */
                return true;
            }
            if (ws > 0)
            {
                /*
                 * Predecessor was cancelled. Skip over predecessors and
                 * indicate retry.
                 */
                do
                {
                    node.Prev = pred = pred.Prev;
                } while (pred.WaitStatus > 0);
                pred.Next = node;
            }
            else
            {
                /*
                 * waitStatus must be 0 or PROPAGATE.  Indicate that we
                 * need a signal, but don't park yet.  Caller will need to
                 * retry to make sure it cannot acquire before parking.
                 */
                CompareAndSetWaitStatus(pred, ws, Node.SIGNAL);
            }
            return false;
        }
        /// <summary>
        /// 线程自我中断
        /// </summary>
        private void selfInterrupt()
        {
            Thread.CurrentThread.Interrupt();
        }
        /// <summary>
        /// 线程休眠且检查是否被中断唤醒 这儿可能有问题！！！！！
        /// </summary>
        /// <returns></returns>
        private  bool parkAndCheckInterrupt()
        {
            try
            {
                LockSupport.park(this);
                return true;
            }
            catch (ThreadInterruptedException e)
            {
                return false;
            }
        }
        #endregion
        #region 独占模式下请求资源的算法支持组建
        /// <summary>
        /// 独占模式下请求资源，忽略中断 
        /// 简单的说就是在等待队列中排队拿资源，中间会休眠，直到拿到资源后才返回。
        /// </summary>
        /// <param name="node"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool acquireQueued(Node node, int arg)
        {
            bool failed = true;
            try
            {
                bool interrupted = false;
                for (; ; )
                {
                    Node p = node.Predecessor;
                    //第一个节点就直接拿资源
                    if (p == head && tryAcquire(arg))
                    {
                        Head = node;
                        p.Next = null; // help GC
                        failed = false;
                        return interrupted;
                    }
                    //进入休眠等待唤醒，并且检查中断
                    if (shouldParkAfterFailedAcquire(p, node) && parkAndCheckInterrupt())
                    {
                        interrupted = true;
                    }
                }
            }
            finally
            {
                if (failed)
                {
                    cancelAcquire(node);
                }
            }
        }
        /// <summary>
        /// 独占模式下请求资源，带中断机制
        /// </summary>
        /// <param name="arg"></param>
        private void doAcquireInterruptibly(int arg)
        {
            Node node = AddWaiter(Node.EXCLUSIVE);
            bool failed = true;
            try
            {
                for (; ; )
                {
                    Node p = node.Predecessor;
                    if (p == head && tryAcquire(arg))
                    {
                        Head = node;
                        p.Next = null; // help GC
                        failed = false;
                        return;
                    }
                    if (shouldParkAfterFailedAcquire(p, node) && parkAndCheckInterrupt())
                        throw new ThreadInterruptedException();
                }
            }
            finally
            {
                if (failed)
                {

                    cancelAcquire(node);
                }
            }
        }

        /// <summary>
        /// 独占模式下请求资源 带超时时间
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="nanosTimeout"></param>
        /// <returns></returns>
        private bool doAcquireNanos(int arg, long nanosTimeout)
        {
            if (nanosTimeout <= 0L)
            {
                return false;
            }
            long deadline = DateTime.Now.Ticks + nanosTimeout;
            Node node = AddWaiter(Node.EXCLUSIVE);
            bool failed = true;
            try
            {
                for (; ; )
                {
                    Node p = node.Predecessor;
                    if (p == head && tryAcquire(arg))
                    {
                        Head = node;
                        p.Next = null; // help GC
                        failed = false;
                        return true;
                    }
                    nanosTimeout = deadline - DateTime.Now.Ticks;
                    if (nanosTimeout <= 0L)
                    {
                        return false;
                    }
                    if (shouldParkAfterFailedAcquire(p, node) && nanosTimeout > spinForTimeoutThreshold)
                    {
                        LockSupport.parkNanos(this, nanosTimeout);
                    }
                    //if (Thread.interrupted())
                    //{
                    //    throw new InterruptedException();
                    //}
                }
            }
             finally
            {
                if (failed)
                {
                    cancelAcquire(node);
                }
            }
        }
        /// <summary>
        /// 共享模式下请求资源 忽略中断
        /// </summary>
        /// <param name="arg"></param>
        private void doAcquireShared(int arg)
        {
            Node node = AddWaiter(Node.SHARED);
            bool failed = true;
            try
            {
                bool interrupted = false;
                for (; ; )
                {
                    Node p = node.Predecessor;
                    if (p == head)
                    {
                        int r = tryAcquireShared(arg);
                        //还有剩余资源会继续唤醒后续的节点
                        if (r >= 0)
                        {
                            setHeadAndPropagate(node, r);
                            p.Next = null; // help GC
                            if (interrupted)
                            {
                                selfInterrupt();
                            }
                            failed = false;
                            return;
                        }
                    }
                    if (shouldParkAfterFailedAcquire(p, node) && parkAndCheckInterrupt())
                    {
                        interrupted = true;
                    }
                }
            }
            finally
            {
                if (failed)
                {
                    cancelAcquire(node);
                }
            }
        }
        /// <summary>
        /// 共享模式下请求资源 带中断机制
        /// </summary>
        /// <param name="arg"></param>
        private void doAcquireSharedInterruptibly(int arg)
        {
            Node node = AddWaiter(Node.SHARED);
            bool failed = true;
            try
            {
                for (; ; )
                {
                    Node p = node.Predecessor;
                    if (p == head)
                    {
                        int r = tryAcquireShared(arg);
                        if (r >= 0)
                        {
                            setHeadAndPropagate(node, r);
                            p.Next = null; // help GC
                            failed = false;
                            return;
                        }
                    }
                    if (shouldParkAfterFailedAcquire(p, node) &&
                        parkAndCheckInterrupt())
                    {
                    }
                }
            }
            finally
            {
                if (failed)
                {
                    cancelAcquire(node);
                }
            }
        }

        /// <summary>
        /// 共享模式下请求资源 带超时机制
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="nanosTimeout"></param>
        /// <returns></returns>
        private bool doAcquireSharedNanos(int arg, long nanosTimeout)
        {
            if (nanosTimeout <= 0L)
            {
                return false;
            }
            long deadline = DateTime.Now.Ticks + nanosTimeout;
            Node node = AddWaiter(Node.SHARED);
            bool failed = true;
            try
            {
                for (; ; )
                {
                    Node p = node.Predecessor;
                    if (p == head)
                    {
                        int r = tryAcquireShared(arg);
                        if (r >= 0)
                        {
                            setHeadAndPropagate(node, r);
                            p.Next = null; // help GC
                            failed = false;
                            return true;
                        }
                    }
                    nanosTimeout = deadline - DateTime.Now.Ticks;
                    if (nanosTimeout <= 0L)
                    {
                        return false;
                    }
                    if (shouldParkAfterFailedAcquire(p, node) && nanosTimeout > spinForTimeoutThreshold)
                    {
                        LockSupport.parkNanos(this, nanosTimeout);
                    }
                }
            }
            finally
            {
                if (failed)
                {
                    cancelAcquire(node);
                }
            }
        }
        #endregion
        #region 请求-释放独占资源相关核心骨架
        /// <summary>
        /// 请求独占资源
        /// </summary>
        /// <param name="arg"></param>
        public void acquire(int arg)
        {
            if (!tryAcquire(arg) && acquireQueued(AddWaiter(Node.EXCLUSIVE), arg))
            {
                selfInterrupt();
            }
        }
        /// <summary>
        /// 请求独占资源  带中断机制
        /// </summary>
        /// <param name="arg"></param>
        public void acquireInterruptibly(int arg)
        {
            //if (Thread.interrupted()){
            //    throw new InterruptedException();
            //}
            if (!tryAcquire(arg))
            {
                doAcquireInterruptibly(arg);
            }
        }
        /// <summary>
        /// 尝试请求独占资源 带超时机制
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="nanosTimeout"></param>
        /// <returns></returns>
        public bool tryAcquireNanos(int arg, long nanosTimeout)
        {
            //if (Thread.interrupted()){
            //    throw new InterruptedException();
            //}
            return tryAcquire(arg) || doAcquireNanos(arg, nanosTimeout);
        }
        /// <summary>
        /// 释放独占资源
        /// </summary>
        public bool release(int arg)
        {
            if (tryRelease(arg))
            {
                Node h = head;
                if (h != null && h.WaitStatus != 0)
                {
                    UnparkSuccessor(h);
                }
                return true;
            }
            return false;
        }
        #endregion
        #region 请求-释放共享资源相关核心骨架
        /// <summary>
        /// 请求共享资源 
        /// </summary>
        /// <param name="arg"></param>
        public void acquireShared(int arg)
        {
            if (tryAcquireShared(arg) < 0)
            {
                doAcquireShared(arg);
            }
        }
        /// <summary>
        /// 请求共享资源 带中断机制
        /// </summary>
        /// <param name="arg"></param>
        public void acquireSharedInterruptibly(int arg)
        {
            //if (Thread.interrupted())
            //{
            //    throw new InterruptedException();
            //}
            if (tryAcquireShared(arg) < 0)
                doAcquireSharedInterruptibly(arg);
        }
        /// <summary>
        /// 尝试请求共享资源 带超时
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="nanosTimeout"></param>
        /// <returns></returns>
        public bool tryAcquireSharedNanos(int arg, long nanosTimeout)
        {
            //if (Thread.interrupted())
            //{
            //    throw new InterruptedException();
            //}
            return tryAcquireShared(arg) >= 0 || doAcquireSharedNanos(arg, nanosTimeout);
        }
        /// <summary>
        /// 释放共享资源
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public bool releaseShared(int arg)
        {
            if (tryReleaseShared(arg))
            {
                doReleaseShared();
                return true;
            }
            return false;
        }
        #endregion
        #region CAS方法
        /// <summary>
        ///  CAS操作更新state
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        protected bool CompareAndSetState(int expect, int update)
        {
            return Interlocked.CompareExchange(ref state, update, expect) == expect;
        }
        /// <summary>
        /// CAS方式更新队列节点的等待状态
        /// </summary>
        /// <param name="node"></param>
        /// <param name="expect"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        private bool CompareAndSetWaitStatus(Node node, int expect, int update)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// CAS方式更新队列头部
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool CompareAndSetHead(Node node)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// CAS方式更新节点后续节点
        /// </summary>
        /// <param name="pred"></param>
        /// <param name="expectNextNode"></param>
        /// <param name="updateNextNode"></param>
        /// <returns></returns>
        private bool CompareAndSetNext(Node pred, Node expectNextNode, Node updateNextNode)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// CAS方式更新队列尾部
        /// </summary>
        /// <param name="expectTail"></param>
        /// <param name="updateTail"></param>
        /// <returns></returns>
        private bool CompareAndSetTail(Node expectTail, Node updateTail)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region 同步器应该被重写的几个方法
        /// <summary>
        /// 尝试获取指定数量的资源 独占模式
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual bool tryAcquire(int arg)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 尝试释放指定数量的资源 独占模式
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual bool tryRelease(int arg)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 尝试获取指定数量的资源 共享模式
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual int tryAcquireShared(int arg)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 尝试释放指定数量的资源 共享模式
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual bool tryReleaseShared(int arg)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 资源当前是否被占有完
        /// </summary>
        /// <returns></returns>
        protected virtual bool isHeldExclusively()
        {
            throw new NotImplementedException();
        }
        #endregion
        #region 队列节点
        /// <summary>
        /// 抽象队列节点
        /// </summary>
        protected sealed class Node
        {
            #region 常量相关
            /// <summary>
            /// 独占模式的常量标识  后面最好修改为枚举型常量
            /// </summary>
            internal static readonly Node SHARED = new Node();
            /// <summary>
            /// 共享模式的常量标识 同上
            /// </summary>
            internal static readonly Node EXCLUSIVE = null;
            #region 节点状态常量标识
            /** waitStatus value to indicate thread has cancelled */
            internal static readonly int CANCELLED = 1;
            /** waitStatus value to indicate successor's thread needs unparking */
            internal static readonly int SIGNAL = -1;
            /** waitStatus value to indicate thread is waiting on condition */
            internal static readonly int CONDITION = -2;
            /** waitStatus value to indicate the next acquireShared should unconditionally propagate */
            internal static readonly int PROPAGATE = -3;
            #endregion
            /// <summary>
            /// 等待自旋超时时间
            /// </summary>
            private static readonly long SPING_FOR_TIMEOUT_THRESHOLD = 1000L;
            #endregion
            /// <summary>
            /// 节点状态
            /// </summary>
            internal  int WaitStatus
            {
                get;
                set;
            }
            /// <summary>
            /// 前节点
            /// </summary>
            internal  Node Prev
            {
                get;
                set;
            }
            /// <summary>
            /// 后节点
            /// </summary>
            internal  Node Next
            {
                get;
                set;
            }
            /// <summary>
            /// 节点所代表的线程
            /// </summary>
            internal  Thread Thread
            {
                get;
                set;
            }
            /// <summary>
            /// 下一个等待节点
            /// </summary>
            internal  Node NextWaiter
            {
                get;
                set;
            }
            /// <summary>
            /// 是否是独占模式
            /// </summary>
            internal bool IsShared
            {
                get
                {
                    return NextWaiter == SHARED;
                }
            }
            /// <summary>
            /// 获取前节点 如果为null则空指针异常
            /// </summary>
            internal Node Predecessor
            {
                get
                {
                    Node p = Prev;
                    if (p == null)
                    {
                        throw new NullReferenceException();
                    }
                    return p;
                }
            }
            internal Node()
            {

            }
            internal Node(Thread thread, Node mode)
            {
                this.NextWaiter = mode;
                this.Thread = thread;
            }
            internal Node(Thread thread, int waitStatus)
            {
                this.WaitStatus = waitStatus;
                this.Thread = thread;
            }
        }
        #endregion
        #region Condition
        //nothing
        #endregion
    }
}
