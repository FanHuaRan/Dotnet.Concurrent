using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dotnet.Concurrent.DotExecutor
{
    public class FutureTask<T> : RunnableFuture<T>
    {
        private Runnable runnable;
        private T value;
        private Callable<T> callable;

        public FutureTask(Runnable runnable, T value)
        {
            // TODO: Complete member initialization
            this.runnable = runnable;
            this.value = value;
        }

        public FutureTask(Callable<T> callable)
        {
            // TODO: Complete member initialization
            this.callable = callable;
        }
    }
}
