﻿using Dotnet.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dotnet.Concurrent
{
    /// <summary>
    /// 自定义初始值的ThreadLocal子类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class OldSuppliedThreadLocal<T> :OldThreadLocal<T>,ThreadLocal<T>
    {

        private readonly Supplier<T> supplier;

        internal OldSuppliedThreadLocal(Supplier<T> supplier)
        {
            this.supplier = Objects.RequireNonNull(supplier);
        }

        internal T initialValue()
        {
            return supplier.get();
        }
    }
}
