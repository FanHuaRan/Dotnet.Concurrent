using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dotnet.Concurrent.Common
{
    /// <summary>
    /// 封装任务执行结果和任务控制的运行任务Runnable接口
    /// 该接口继承自Runable和Future,是任务执行框架当中每个任务执行时的抽象
    /// 2018/01/15 fhr
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface RunnableFuture<T>:Runnable,Future<T>
    {
        //再一次申明run方法，表示在运行结束后将会设置运行结果，除非被取消
        void run();
    }
}
