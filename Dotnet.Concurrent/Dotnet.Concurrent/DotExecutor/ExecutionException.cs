using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dotnet.Concurrent.DotExecutor
{
    /// <summary>
    /// 任务执行异常 封装了任务执行时遇到的异常
    /// 2018/01/15 fhr
    /// </summary>
    public class ExecutionException : Exception
    {

        internal ExecutionException() { }

        /**
         * Constructs an {@code ExecutionException} with the specified detail
         * message. The cause is not initialized, and may subsequently be
         * initialized by a call to {@link #initCause(Throwable) initCause}.
         *
         * @param message the detail message
         */
        internal ExecutionException(String message)
            : base(message)
        {
        }

        /**
         * Constructs an {@code ExecutionException} with the specified detail
         * message and cause.
         *
         * @param  message the detail message
         * @param  cause the cause (which is saved for later retrieval by the
         *         {@link #getCause()} method)
         */
        internal ExecutionException(String message, Exception cause)
            : base(message, cause)
        {
        }

        /**
         * Constructs an {@code ExecutionException} with the specified cause.
         * The detail message is set to {@code (cause == null ? null :
         * cause.toString())} (which typically contains the class and
         * detail message of {@code cause}).
         *
         * @param  cause the cause (which is saved for later retrieval by the
         *         {@link #getCause()} method)
         */
        internal ExecutionException(Exception cause)
            : base(null, cause)
        {

        }
    }
}
