using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dotnet.Concurrent
{
    /// <summary>
    /// ThreadLocal接口，两种实现
    /// 2017/11/21 fhr
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ThreadLocal<T>
    {
        T InitialValue();

        T Get();

        void Set(T value);
    }
}
