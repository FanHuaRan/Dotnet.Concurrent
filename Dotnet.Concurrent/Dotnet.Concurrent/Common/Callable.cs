using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dotnet.Concurrent.Common
{
    /// <summary>
    /// 带返回值和可抛出异常的任务执行接口
    /// 2018/01/15 fhr
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public interface Callable<T>
    {
        T call();
    }
}
