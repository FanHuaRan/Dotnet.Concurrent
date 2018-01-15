using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dotnet.Concurrent.Common;

namespace Dotnet.Concurrent.DotExecutor
{
    /// <summary>
    /// 任务执行接口
    /// 2017//11/03 fhr
    /// </summary>
    public interface Executor
    {
        void Execute(Runnable runable);
    }
}
