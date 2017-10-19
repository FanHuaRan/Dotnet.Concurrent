using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnet.Concurrent.Atomic
{
    /// <summary>
    /// 原子long 基于CAS+自旋无锁化编程实现，核心Interlocked
    /// 2017/10/18 fhr
    /// </summary>
    [Serializable]
    public class AtomicLong
    {
         /// <summary>
        /// 缺省默认值
        /// </summary>
        private static readonly long DEFAULT_LONG_VALUE=0;
        /// <summary>
        /// 被包装的long值
        /// </summary>
        private volatile long value;
        /// <summary>
        /// 比较并设置新值 成功返回true 失败返回false
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        public bool CompareAndSet(long expect, long update)
        {
            return expect == Interlocked.CompareExchange(ref value, update, expect);
        }
        /// <summary>
        /// 设置新值，返回旧值
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public long GetAndSet(long newValue)
        {
            return Interlocked.Exchange(ref value, newValue);
        }
        /// <summary>
        /// 自增1，返回新值
        /// </summary>
        /// <returns></returns>
        public long IncrementAndGet()
        {
            return Interlocked.Increment(ref value);
        }
        /// <summary>
        /// 自增1，返回旧值
        /// </summary>
        /// <returns></returns>
        public long GetAndIncrement()
        {
            return Interlocked.Increment(ref value)-1;
        }
        /// <summary>
        /// 自减一，返回新值
        /// </summary>
        /// <returns></returns>
        public long DecrementAndGet()
        {
            return Interlocked.Decrement(ref value);
        }
        /// <summary>
        /// 自减一，返回旧值
        /// </summary>
        /// <returns></returns>
        public long GetAndDecrement()
        {
            return Interlocked.Decrement(ref value)+1;
        }
        /// <summary>
        /// 加上add,返回旧值
        /// </summary>
        /// <param name="add"></param>
        /// <returns></returns>
        public long GetAndAdd(long add)
        {
            for (; ; )
            {
                long current = value;
                long next=current+add;
                if (CompareAndSet(current,next))
                {
                    return current;
                }
            }
        }
        /// <summary>
        /// 加上add,返回新值
        /// </summary>
        /// <param name="add"></param>
        /// <returns></returns>
        public long AddAndGet(long add)
        {
            for (; ; )
            {
                long current = value;
                long next = current + add;
                if (CompareAndSet(current, next))
                {
                    return current;
                }
            }
        }
        
        public AtomicLong(long inital)
        {
            this.value = inital;
        }

        public AtomicLong()
            : this(DEFAULT_LONG_VALUE)
        {

        }
        /// <summary>
        /// value getter&setter
        /// </summary>
        public long Value
        {
            get { return value; }
            set { this.value = value; }
        }
        /// <summary>
        /// 重写hashcode value相关
        /// </summary>
        /// <returns></returns>
        public override long GetHashCode()
        {
            return value;
        }
        /// <summary>
        /// 重写equals value相关
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is AtomicLong&&obj!=null)
            {
                AtomicLong atoObj = obj as AtomicLong;
                if (atoObj.Value == Value)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// toString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return value.ToString();
        }
    }
}
