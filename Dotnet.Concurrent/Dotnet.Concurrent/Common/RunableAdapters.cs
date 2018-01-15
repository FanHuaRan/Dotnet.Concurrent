using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dotnet.Concurrent.Common
{
    internal class RunableAdapters
    {
        public static Callable<T> Callable<T>(Runnable runnable, T result)
        {
            return new RunnableAdapter<T>(runnable, result);
        }
    }
}
