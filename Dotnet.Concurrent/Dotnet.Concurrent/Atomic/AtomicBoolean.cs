using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnet.Concurrent.Atomic
{
    /// <summary>
    /// 原子Boolean 基于CAS+自旋无锁化编程实现，核心Interlocked
    /// 2017/10/18 fhr
    /// </summary>
    [Serializable]
    public class AtomicBoolean
    {
        private static readonly int TRUE_INT = 1;

        private static readonly int FALSE_INT = 0;

        /// <summary>
        /// 被包装的boolean值 int表示 0为false 1为真
        /// </summary>
        private volatile int value;

        /// <summary>
        /// 比较并设置新值 成功返回true 失败返回false
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        public bool CompareAndSet(bool expect, bool update)
        {
            int e = expect ? TRUE_INT : FALSE_INT;
            int u = update ? TRUE_INT : FALSE_INT;
            return e == Interlocked.CompareExchange(ref value, u, e);
        }

        /// <summary>
        /// 设置新值，返回旧值
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public bool GetAndSet(bool newValue)
        {
            int n = newValue ? TRUE_INT : FALSE_INT;
            return Interlocked.Exchange(ref value, n) == TRUE_INT;
        }

         public AtomicBoolean(bool inital)
        {
            Value = inital;
         }

         public AtomicBoolean()
            : this(false)
        {

        }
        /// <summary>
        /// value getter&setter
        /// </summary>
        public bool Value
        {
            get { return value==TRUE_INT; }
            set
            {
                this.value = value==true?TRUE_INT:FALSE_INT;
            }
        }
        /// <summary>
        /// 重写hashcode value相关
        /// </summary>
        /// <returns></returns>
        public override long GetHashCode()
        {
            return value.GetHashCode();
        }
        /// <summary>
        /// 重写equals value相关
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is AtomicBoolean && obj != null)
            {
                AtomicBoolean atoObj = obj as AtomicBoolean;
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
