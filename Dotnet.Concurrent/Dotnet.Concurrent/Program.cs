using Dotnet.Concurrent.Atomic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnet.Concurrent
{
    class Program
    {
        static void Main(string[] args)
        {
            AtomicIneger atomicInt = new AtomicIneger(0);
            Thread t1 = new Thread(() =>
            {
                for (int i = 0; i < 10000; i++)
                {
                    atomicInt.incrementAndGet();
                }
            });
            t1.Start();
            Thread t2 = new Thread(() =>
            {
                for (int i = 0; i < 10000; i++)
                {
                    atomicInt.incrementAndGet();
                }
            });
            t2.Start();
            Thread t3 = new Thread(() =>
            {
                for (int i = 0; i < 10000; i++)
                {
                    atomicInt.incrementAndGet();
                }
            });
            t3.Start();
            while (t1.ThreadState == ThreadState.Running || t2.ThreadState == ThreadState.Running || t3.ThreadState==ThreadState.Running)
            {
                //nothing
            }
            Console.WriteLine("atomicInt is " + atomicInt.Value);
            Console.ReadLine();
        }
    }
}
