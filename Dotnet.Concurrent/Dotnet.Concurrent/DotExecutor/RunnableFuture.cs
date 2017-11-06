using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dotnet.Concurrent.DotExecutor
{
    public interface RunnableFuture<T>:Runnable,Future<T>
    {
    }
}
