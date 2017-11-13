using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dotnet.Concurrent
{
    /// <summary>
    /// 阻塞队列接口
    /// 2017/11/13 fhr
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface BlockingQueue<T>
    {
        /// <summary>
        /// 添加元素，成功返回true 失败就抛出异常
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        bool add(T e);
        /// <summary>
        /// 添加元素，成功返回true 容量不够返回false
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        bool offer(T e);
        /// <summary>
        /// 添加元素 如果容量不够就阻塞 
        /// </summary>
        /// <param name="e"></param>
        void put(T e);
        /// <summary>
        /// 添加元素，带超时，成功返回true,失败返回false
        /// </summary>
        /// <param name="e"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        bool offer(T e, long timeout);
        /// <summary>
        /// 拿走队头元素 阻塞
        /// </summary>
        /// <returns></returns>
        T take();
        /// <summary>
        /// 拿走队头元素,没有获取到就返回null
        /// </summary>
        /// <returns></returns>
        T poll();
        /// <summary>
        /// 拿走队头元素指定timeout时间都没有获取到就返回null
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        T poll(long timeout);
        /// <summary>
        /// 扩容 返回新容量
        /// </summary>
        /// <returns></returns>
        int remainingCapacity();
        /// <summary>
        /// 移除元素
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        bool remove(Object o);
        /// <summary>
        /// 判断是否包含该元素
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public bool contains(Object o);
        /// <summary>
        /// 移除所有可用元素到新集合当中 返回移除的元素个数
        /// </summary>
        /// <param name="super"></param>
        /// <returns></returns>
        int drainTo<V>(IEnumerable<V> c) where V : T;
        /// <summary>
        /// 移除所有可用元素到新集合当中 返回移除的元素个数,带最多元素限制
        /// </summary>
        /// <param name="super"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        int drainTo<V>(IEnumerable<V> c, int maxElements) where V : T;
    }
}
