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
            T value;
            if (concurrentDictionary.TryGetValue( Thread.CurrentThread, out value))
            {
                return value;
            }
            return SetInitial();
        }

        public void Set(T value)
        {
           concurrentDictionary.AddOrUpdate(Thread.CurrentThread, value, null);
        }

        public void Remove()
        {
            T oldValue;
            concurrentDictionary.TryRemove(Thread.CurrentThread, out oldValue);
        }

        private T SetInitial()
        {
            T value = InitialValue();
            concurrentDictionary.AddOrUpdate(Thread.CurrentThread, value, null);
            return value;
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
