using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dotnet.Concurrent.Common;

namespace Dotnet.Concurrent.DotExecutor
{
    /// <summary>
    ///  Producers submit tasks for execution. 
    ///  Consumers  take completed tasks and process their results in the order they omplete
    ///  完成任务的服务 基于生产者-消费者模型
    ///  2017/11/13 fhr
    /// </summary>
    public class CompletionService<V>
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
        Future<V> Poll(long timeout);
    }
}
