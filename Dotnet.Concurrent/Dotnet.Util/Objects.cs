using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dotnet.Util
{
    /// <summary>
    /// Object公共辅助方法
    /// 2017/11/21 fhr
    /// </summary>
    public class Objects
    {
        /// <summary>
        /// 带空指针检测的获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T RequireNonNull<T>(T obj)
        {
            if (obj == null)
            {
                throw new NullReferenceException();
            }
            return obj;
        }
    }
}
