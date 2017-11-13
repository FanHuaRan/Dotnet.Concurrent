using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dotnet.Concurrent.Common
{
    /// <summary>
    /// 综合Runable和Future的接口
    /// 2017/11/13 fhr
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface RunnableFuture<T>:Runnable,Future<T>
    {
        void run();
    }
}
