using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dotnet.Concurrent.DotExecutor
{
    /// <summary>
    /// Callable委托 含有可返回值
    /// 2017/11/06 fhr
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public interface Callable<T>
    {
        T call();
    }
}
