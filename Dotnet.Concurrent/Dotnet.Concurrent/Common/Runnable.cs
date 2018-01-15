using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dotnet.Concurrent.Common
{
    /// <summary>
    /// 不带返回值的任务接口
    /// 2018/01/15 fhr
    /// </summary>
    public interface Runnable
    {
        
        /// <summary>
        ///  执行任务，一个任务就是一个Runnable
        /// </summary>
        void run();
    }
}
