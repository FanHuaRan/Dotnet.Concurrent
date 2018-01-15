using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dotnet.Concurrent.Common;
using Dotnet.Util;

namespace Dotnet.Concurrent.DotExecutor
{
    /// <summary>
    ///  用于批量任务执行管理的服务
    ///  是用于获取完成任务的服务 基于生产者-消费者模型
    ///  2018/01/15 fhr
    /// </summary>
    public interface CompletionService<V>
    {
        /// <summary>
        /// 提交任务
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        Future<V> Submit(Callable<V> task);
        /// <summary>
        /// 提交任务
        /// </summary>
        /// <param name="task"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        Future<V> Submit(Runnable task, V result);
        /// <summary>
        /// 获取返回结果 带阻塞
        /// </summary>
        /// <returns></returns>
        Future<V> Take();
        /// <summary>
        /// 获取返回结果 如果没有执行完就返回null
        /// </summary>
        /// <returns></returns>
        Future<V> Poll();
        /// <summary>
        /// 获取返回结果 带超时时间
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        Future<V> Poll(long timeout,TimeUnit unit);
    }
}
