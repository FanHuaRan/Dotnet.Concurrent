using Dotnet.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnet.Concurrent
{
    /// <summary>
    /// java老版本的ThreadLocal的实现
    /// 有内存泄露的问题  
    /// 2017/11/21
    /// </summary>
    public class OldThreadLocal<T>:ThreadLocal<T>
    {

        private ConcurrentDictionary<Thread, T> concurrentDictionary = new ConcurrentDictionary<Thread, T>();

        public virtual T InitialValue()
        {
            return default(T);
        }

        public T Get()
        {
            var currentThread = Thread.CurrentThread;
            T value;
            if (concurrentDictionary.TryGetValue(currentThread, out value))
            {
                return value;
            }
            return default(T);
        }

        public void Set(T value)
        {
           var currentThread = Thread.CurrentThread;
           concurrentDictionary.AddOrUpdate(currentThread, value, null);
        }

        /// <summary>
        /// Creates a thread local variable. The initial value of the variable is
        /// determined by invoking the {@code get} method on the {@code Supplier}.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="supplier"></param>
        /// <returns></returns>
        public static ThreadLocal<S> withInitial<S>(Supplier<S> supplier)
        {
            return new OldSuppliedThreadLocal<S>(supplier);
        }
    }
}
