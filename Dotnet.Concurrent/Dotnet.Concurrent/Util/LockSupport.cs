using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnet.Concurrent.Util
{
    /// <summary>
    /// 锁工具锁
    /// 2017/10/19 fhr
    /// </summary>
    public class LockSupport
    {
        /// <summary>
        /// be sure Cannot be instantiated
        /// </summary>
        private LockSupport()
        {
        }

        /// <summary>
        /// 中断线程的等待状态
        /// </summary>
        /// <param name="thread"></param>
        public static void unpark(Thread thread)
        {
            if (thread != null)
            {
                //UNSAFE.unpark(thread);
            }
            throw new NotImplementedException();
        }

       /// <summary>
       /// 让线程进入等待状态
       /// </summary>
       /// <param name="blocker"></param>
        public static void park(Object blocker)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 让线程进入有限等待状态 纳秒为单位
        /// </summary>
        /// <param name="blocker"></param>
        /// <param name="nanos"></param>
        public static void parkNanos(Object blocker, long nanos)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 让线程进入有限等待状态 毫秒为单位
        /// </summary>
        /// <param name="blocker"></param>
        /// <param name="deadline"></param>
        public static void parkUntil(Object blocker, long deadline)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 获取线程当前的阻塞对象
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Object getBlocker(Thread t)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 让线程进入等待状态
        /// </summary>
        public static void park()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 让线程进入有限等待状态 纳秒为单位
        /// </summary>
        /// <param name="nanos"></param>
        public static void parkNanos(long nanos)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 让线程进入有限等待状态 毫秒为单位
        /// </summary>
        /// <param name="deadline"></param>
        public static void parkUntil(long deadline)
        {
            throw new NotImplementedException();
        }
    }
}
