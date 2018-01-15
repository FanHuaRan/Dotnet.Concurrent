using Dotnet.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Dotnet.Concurrent.Common
{
    /// <summary>
    /// 封装任务执行结果和任务控制的接口
    /// 2018/01/15 fhr
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface Future<T>
    {
        
        /// <summary>
        /// //尝试取消任务的执行，在以下场景将会失败：
        /// 1.任务已经完成
        /// 2.任务已经被取消
        /// 3.一些其他的原因
        ///  对于没有开始运行的任务，这些任务后面将不会运行，如果任务已经开始运行，那么mayInterruptIfRunning参数将决定是否打断该线
        ///  程，从而让该任务自己在合适的时候检测中断。从而自己停止任务
        /// </summary>
        /// <param name="mayInterruptIfRunning"></param>
        /// <returns></returns>
        bool cancel(bool mayInterruptIfRunning);

        /// <summary>
        ///判断任务是否已经被取消
        /// </summary>
        /// <returns></returns>
        bool isCancelled();

        /// <summary>
        /// 判断任务是否已经完成
        /// </summary>
        /// <returns></returns>
        bool isDone();

        /// <summary>
        /// 获取运行结果，
        ///  如果尚未运行完毕，将会阻塞直到运行完毕或者爆出异常
        ///  get方法将会获取运行结果或者抛出运行时遇到的异常（被封装在ExecutionException中）
        /// </summary>
        /// <returns></returns>
        T get();

        /// <summary>
        /// 获取运行结果，带超时机制
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        T get(long timeout,TimeUnit unit);
    }
}
