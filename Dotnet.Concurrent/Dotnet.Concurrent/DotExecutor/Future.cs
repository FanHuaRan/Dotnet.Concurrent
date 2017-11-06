using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Dotnet.Concurrent.DotExecutor
{
    public interface Future<T>
    {

        bool cancel(bool mayInterruptIfRunning);

        bool isCancelled();

        bool isDone();

        T get();

        T get(long timeout, TimeUnit unit);
    }
}
