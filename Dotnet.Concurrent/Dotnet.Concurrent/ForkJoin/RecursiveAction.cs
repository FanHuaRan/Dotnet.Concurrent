using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dotnet.Concurrent.ForkJoin
{
    /// <summary>
    /// 不带返回值的fork-join任务
    /// 2017/11/07 fhr
    /// </summary>
    public class RecursiveAction : ForkJoinTask<Object>
    {

        /**
         * The main computation performed by this task.
         */
        protected abstract void compute();

        /**
         * Always returns {@code null}.
         *
         * @return {@code null} always
         */
        public sealed object getRawResult() { return null; }

        /**
         * Requires null completion value.
         */
        protected sealed void setRawResult(Object mustBeNull) { }

        /**
         * Implements execution conventions for RecursiveActions.
         */
        protected sealed bool exec()
        {
            compute();
            return true;
        }
    }
}
