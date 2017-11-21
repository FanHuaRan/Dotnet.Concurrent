using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dotnet.Util
{
    /// <summary>
    /// 提供器接口
    /// 2017/11/21 fhr
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface Supplier<T>
    {
        T get();
    }
}
