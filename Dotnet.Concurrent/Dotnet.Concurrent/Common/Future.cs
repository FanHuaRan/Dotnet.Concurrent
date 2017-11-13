using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Dotnet.Concurrent.Common
{
    /// <summary>
    /// Futute接口
    /// 2017/11/13 fhr
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface Future<T>
    {
        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="mayInterruptIfRunning"></param>
        /// <returns></returns>
        bool cancel(bool mayInterruptIfRunning);
        /// <summary>
        /// 是否取消
        /// </summary>
        /// <returns></returns>
        bool isCancelled();
        /// <summary>
        /// 是否完成
        /// </summary>
        /// <returns></returns>
        bool isDone();
        /// <summary>
        /// 获取值 阻塞or非阻塞
        /// </summary>
        /// <returns></returns>
        T get();
        /// <summary>
        /// 获取值 带超时
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        T get(long timeout);
    }
}
