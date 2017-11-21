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
        /// <summary>
        /// 获取通用初始值
        /// </summary>
        /// <returns></returns>
        T InitialValue();

        /// <summary>
        /// 获取本线程对应的值
        /// </summary>
        /// <returns></returns>
        T Get();

        /// <summary>
        /// 设置本线程对应的值
        /// </summary>
        /// <param name="value"></param>
        void Set(T value);

        /// <summary>
        /// 移除本线程对应的值
        /// </summary>
        void Remove();
    }
}
