using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dotnet.Concurrent
{
    /// <summary>
    /// Exception thrown when attempting to retrieve the result of a task
    /// that aborted by throwing an exception. This exception can be
    /// inspected using the {@link #getCause()} method.
    /// 当试图获取一个因为异常中止的任务的结果所引发的异常
    /// 2017/11/13 fhr
    /// </summary>
    /// 
    [Serializable]
    public class ExecutionException : Exception
    {
        protected ExecutionException() { }

        protected ExecutionException(String message)
            : base(message)
        {
        }

        public ExecutionException(String message, Exception cause)
            : base(message, cause)
        {
        }

        public ExecutionException(Exception cause)
            : base("", cause)
        {
        }
    }
}
