using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnet.Concurrent.Locks
{
    /// <summary>
    /// 抽象资源占有同步器
    /// 2017/10/19 fhr
    /// </summary>
    [Serializable]
    public  abstract class AbstractOwnableSynchronizer
    {
        //Empty constructor for use by subclasses.
        protected AbstractOwnableSynchronizer() { }

        // 当前占有独享资源的线程
        [NonSerialized]
        internal sealed Thread ExclusiveOwnerThread
        {
            get;
            set;
        }

    }
}
