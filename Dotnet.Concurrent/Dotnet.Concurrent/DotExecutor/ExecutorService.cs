using Dotnet.Concurrent.Common;
using Dotnet.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dotnet.Concurrent.DotExecutor
{
    /// <summary>
    /// Executor接口只提供了一个基本的任务执行接口，
    /// ExecutorService继承自Executor,提供了一系列的服务周期控制方法和对任务执行的扩展接口
    /// 2018/01/15 fhr
    /// </summary>
    public interface ExecutorService:Executor
    {
        /// <summary>
        /// 关闭任务执行服务，新任务将不会被接受，但已经提交的任务将会被全部执行
        /// </summary>
        void Shutdown();

        /// <summary>
        /// 立即关闭任务执行服务，新任务将不会被接受，已经提交的任务将不会执行，正在执行的服务将会被打断。
        /// 返回已经被提交的，但是还没有执行的任务
        /// </summary>
        /// <returns></returns>
        List<Runnable> ShutdownNow();

        /// <summary>
        /// 判断执行服务是否被关闭
        /// </summary>
        /// <returns></returns>
        bool IsShutdown();

        /// <summary>
        /// 判断当前所有任务是否都已经在shutdown之后完成，包括被中断的任务，但不包括尚未执行的任务
        /// </summary>
        /// <returns></returns>
        bool IsTerminated();

        /// <summary>
        /// 这个方法在shutdown之后可以调用
        /// 目的是阻塞当前线程直到所有任务都已经完成或者时间超时
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        bool AwaitTermination(long timeout, TimeUnit unit);

        /// <summary>
        /// 提交一个带返回值的任务，返回future类型的对象，该对象封装了运行结果、运行异常且支持取消执行等操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        Future<T> Submit<T>(Callable<T> task);

        /// <summary>
        /// 提交一个不带返回值的任务，不过会将第二个参数作为该任务的返回值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        Future<T> Submit<T>(Runnable task, T result);

        /// <summary>
        /// 提交一个不带返回值的任务，不过会返回一个future对象，用于实现任务执行的控制
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        Future<Object> Submit(Runnable task);

        /// <summary>
        /// 批量执行带返回值任务，阻塞直到全部任务完成
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tasks"></param>
        /// <returns></returns>
        List<Future<T>> InvokeAll<T>(IEnumerable<Callable<T>> tasks);

        /// <summary>
        /// 批量执行带返回值任务，阻塞直到全部任务完成或者超时
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tasks"></param>
        /// <param name="timeout"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        List<Future<T>> InvokeAll<T>(IEnumerable<Callable<T>> tasks, long timeout, TimeUnit unit);

        /// <summary>
        /// 批量执行带返回值的任务，直到一个任务运行完成则返回该任务的运行结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tasks"></param>
        /// <returns></returns>
        T InvokeAny<T>(IEnumerable<Callable<T>> tasks);

        /// <summary>
        /// 批量执行带返回值，不过只返回一个成功执行的任务的结果,带超时
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tasks"></param>
        /// <param name="timeout"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        T InvokeAny<T>(IEnumerable<Callable<T>> tasks, long timeout, TimeUnit unit);
    }
}
