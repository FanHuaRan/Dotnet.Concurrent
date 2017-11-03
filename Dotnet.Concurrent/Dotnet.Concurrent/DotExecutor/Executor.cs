using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dotnet.Concurrent.DotExecutor
{
    /// <summary>
    /// 线程池基本接口
    /// 2017//11/03 fhr
    /// </summary>
    public interface Executor
    {
        void Execute(Action command);
    }
}
