using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dotnet.Concurrent
{
    /// <summary>
    /// ThreadLocal存储变量所需要的map,这儿并没有实现map接口，因为我们只需要极少的API
    /// jdk1.6以后就是每个线程一个ThreadLocal，省去额外的同步
    /// 2017/11/21 fhr
    /// </summary>
    internal class ThreadLocalMap<T>
    {
        /// <summary>
        /// 桶/表的初始最大容量默认值 -一定要是2的倍方
        /// </summary>
        private static readonly int INITIAL_CAPACITY = 16;

        /// <summary>
        /// 桶/表
        /// </summary>
        private Entry<T>[] table;

        /// <summary>
        /// 桶元素的数量
        /// </summary>
        private int size = 0;

        /// <summary>
        /// 下一个触发重分配大小的数量值
        /// </summary>
        private int threshold;

        /// <summary>
        /// Set the resize threshold to maintain at worst a 2/3 load factor.
        /// </summary>
        /// <param name="len"></param>
        private void setThreshold(int len)
        {
            threshold = len * 2 / 3;
        }
        
        /// <summary>
        ///  Increment i modulo len. mo
        /// </summary>
        /// <param name="i"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        private static int nextIndex(int i, int len)
        {
            return ((i + 1 < len) ? i + 1 : 0);
        }

        /// <summary>
        /// Decrement i modulo len.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        private static int prevIndex(int i, int len)
        {
            return ((i - 1 >= 0) ? i - 1 : len - 1);
        }

        /// <summary>
        /// Construct a new map initially containing (firstKey, firstValue).
        /// ThreadLocalMaps are constructed lazily, so we only create
        /// one when we have at least one entry to put in it.
        /// </summary>
        /// <param name="firstKey"></param>
        /// <param name="firstValue"></param>
       internal ThreadLocalMap(NewThreadLocal<T> firstKey, T firstValue) {
            table = new Entry<T>[INITIAL_CAPACITY];
            var i = firstKey.threadLocalHashCode & (INITIAL_CAPACITY - 1);
            table[i] = new Entry<T>(firstKey, firstValue);
            size = 1;
            setThreshold(INITIAL_CAPACITY);
        }

        /// <summary>
        /// Construct a new map including all Inheritable ThreadLocals
        /// from given parent map. Called only by createInheritedMap.
        /// </summary>
        /// <param name="parentMap"></param>
        private ThreadLocalMap(ThreadLocalMap<T> parentMap) {
            var parentTable = parentMap.table;
            var len = parentTable.Length;
            setThreshold(len);
            table = new Entry<T>[len];
            for (int j = 0; j < len; j++) {
                var e = parentTable[j];
                if (e != null) {
                    ThreadLocal<T> key = (ThreadLocal<T>)e.Get(); ;
                    //if (key != null) {
                    //    Object value = key.childValue(e.Value);
                    //    Entry<T> c = new Entry<T>(key,value);
                    //    int h = key.threadLocalHashCode & (len - 1);
                    //    while (table[h] != null)
                    //        h = nextIndex(h, len);
                    //    table[h] = c;
                    //    size++;
                    //}
                }
            }
        }
    }
    /// <summary>
    /// 利用弱引用构建的元素
    /// 2017/11/21 fhr
    /// </summary>
    /// <typeparam name="T"></typeparam>
     internal class Entry<T>
     {
         private readonly WeakReference<ThreadLocal<T>> weak;
         internal T Value { get; set; }

         internal Entry(ThreadLocal<T> threadLocal, T value)
         {
             this.weak.SetTarget(threadLocal);
             this.Value = value;
         }
         internal ThreadLocal<T> Get()
         {
             ThreadLocal<T> target;
             if (this.weak.TryGetTarget(out target))
             {
                 return target;
             }
             return null;
         }
     }
}
