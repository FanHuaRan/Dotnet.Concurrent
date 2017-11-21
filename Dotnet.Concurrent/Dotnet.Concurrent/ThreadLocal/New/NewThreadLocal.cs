using Dotnet.Concurrent.Atomic;
using Dotnet.Concurrent.Util;
using Dotnet.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dotnet.Concurrent
{
    /// <summary>
    /// ThreadLocal容器
    /// 2017/11/21
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NewThreadLocal<T> : ThreadLocal<T>
    {
        internal readonly int threadLocalHashCode = NextHashCode();

        /// <summary>
        /// The next hash code to be given out. Updated atomically. Starts at zero.
        /// </summary>
        private static AtomicInteger nextHashCode = new AtomicInteger();

        /// <summary>
        /// The difference between successively generated hash codes - turns
        /// implicit sequential thread-local IDs into near-optimally spread
        /// multiplicative hash values for power-of-two-sized tables.
        /// </summary>
        private static readonly int HASH_INCREMENT = 0x61c88647;

        /// <summary>
        ///  Returns the next hash code.
        /// </summary>
        /// <returns></returns>
        private static int NextHashCode()
        {
            return nextHashCode.GetAndAdd(HASH_INCREMENT);
        }

        public T InitialValue()
        {
            return default(T);
        }

        public T Get()
        {
            throw new NotImplementedException();
        }

        public void Set(T value)
        {
            throw new NotImplementedException();
        }

        public void Remove()
        {
            throw new NotImplementedException();
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
            return new NewSuppliedThreadLocal<S>(supplier);
        }
    }
}
