using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dotnet.Concurrent.Common;

namespace Dotnet.Concurrent.Common
{
    /// <summary>
    /// Runable适配器 让runable接口与callable接口统一
    /// 2017/11/13 fhr
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class RunnableAdapter<T> : Callable<T>
    {
        private readonly Runnable task;
        private readonly T result;
        internal RunnableAdapter(Runnable task, T result)
        {
            this.task = task;
            this.result = result;
        }
        public T call()
        {
            task.run();
            return result;
        }
    }
}
