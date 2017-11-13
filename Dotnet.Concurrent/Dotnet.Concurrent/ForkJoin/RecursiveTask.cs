using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dotnet.Concurrent.ForkJoin
{
    /// <summary>
    /// 带返回值的fork-join任务
    /// 2017/11/07 fhr
    /// </summary>
    public class RecursiveTask<T> : ForkJoinTask<T>
    {
        /**
         * The result of the computation.
         */
        T result;

        /**
         * The main computation performed by this task.
         * @return the result of the computation
         */
        protected abstract T compute();

        public sealed T getRawResult()
        {
            return result;
        }

        protected sealed void setRawResult(T value)
        {
            result = value;
        }

        /**
         * Implements execution conventions for RecursiveTask.
         */
        protected sealed bool exec()
        {
            result = compute();
            return true;
        }
    }
}
