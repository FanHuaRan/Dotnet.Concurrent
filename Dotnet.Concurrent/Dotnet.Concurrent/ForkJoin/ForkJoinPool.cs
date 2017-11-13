using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dotnet.Concurrent.DotExecutor;

namespace Dotnet.Concurrent.ForkJoin
{
    /// <summary>
    /// ForkJoin工作池实现
    /// 由ForkJoinTask数组和ForkJoinWorkerThread数组组成，ForkJoinTask数组负责存放程序提交给ForkJoinPool的任务，而ForkJoinWorkerThread数组负责执行这些任务
    /// 2017/11/07 fhr
    /// </summary>
    public class ForkJoinPool : AbsractExecutorService
    {

    }
}
