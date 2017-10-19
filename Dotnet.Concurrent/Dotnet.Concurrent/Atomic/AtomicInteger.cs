using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnet.Concurrent.Atomic
{
    /// <summary>
    /// 原子int 基于CAS+自旋无锁化编程实现，核心Interlocked
    /// 2017/10/18 fhr
    /// </summary>
   [Serializable]
    public class AtomicInteger
    {
        /// <summary>
        /// 缺省默认值
        /// </summary>
        private static readonly int DEFAULT_INITAL_VALUE=0;
        /// <summary>
        /// 被包装的int值
        /// </summary>
        private volatile int value;
        /// <summary>
        /// 比较并设置新值 成功返回true 失败返回false
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        public bool CompareAndSet(int expect, int update)
        {
            return expect == Interlocked.CompareExchange(ref value, update, expect);
        }
        /// <summary>
        /// 设置新值，返回旧值
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public int GetAndSet(int newValue)
        {
            return Interlocked.Exchange(ref value, newValue);
        }
        /// <summary>
        /// 自增1，返回新值
        /// </summary>
        /// <returns></returns>
        public int IncrementAndGet()
        {
            return Interlocked.Increment(ref value);
        }
        /// <summary>
        /// 自增1，返回旧值
        /// </summary>
        /// <returns></returns>
        public int GetAndIncrement()
        {
            return Interlocked.Increment(ref value)-1;
        }
        /// <summary>
        /// 自减一，返回新值
        /// </summary>
        /// <returns></returns>
        public int DecrementAndGet()
        {
            return Interlocked.Decrement(ref value);
        }
        /// <summary>
        /// 自减一，返回旧值
        /// </summary>
        /// <returns></returns>
        public int GetAndDecrement()
        {
            return Interlocked.Decrement(ref value)+1;
        }
        /// <summary>
        /// 加上add,返回旧值
        /// </summary>
        /// <param name="add"></param>
        /// <returns></returns>
        public int GetAndAdd(int add)
        {
            for (; ; )
            {
                int current = value;
                int next=current+add;
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
        public int AddAndGet(int add)
        {
            for (; ; )
            {
                int current = value;
                int next = current + add;
                if (CompareAndSet(current, next))
                {
                    return current;
                }
            }
        }
        
        public AtomicInteger(int inital)
        {
            this.value = inital;
        }

        public AtomicInteger()
            : this(DEFAULT_INITAL_VALUE)
        {

        }
        /// <summary>
        /// value getter&setter
        /// </summary>
        public int Value
        {
            get { return value; }
            set { this.value = value; }
        }
        /// <summary>
        /// 重写hashcode value相关
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
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
            if (obj is AtomicInteger&&obj!=null)
            {
                AtomicInteger atoObj = obj as AtomicInteger;
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
