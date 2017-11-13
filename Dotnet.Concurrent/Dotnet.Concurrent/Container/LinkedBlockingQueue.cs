using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dotnet.Concurrent.Container
{
    /// <summary>
    /// 阻塞队列实现
    /// 2017/11/13 fhr
    /// </summary>
    public class LinkedBlockingQueue<T> : BlockingQueue<T>
    {

        public bool add(T e)
        {
            throw new NotImplementedException();
        }

        public bool offer(T e)
        {
            throw new NotImplementedException();
        }

        public void put(T e)
        {
            throw new NotImplementedException();
        }

        public bool offer(T e, long timeout)
        {
            throw new NotImplementedException();
        }

        public T take()
        {
            throw new NotImplementedException();
        }

        public T poll(long timeout)
        {
            throw new NotImplementedException();
        }

        public int remainingCapacity()
        {
            throw new NotImplementedException();
        }

        public bool remove(object o)
        {
            throw new NotImplementedException();
        }

        public bool contains(object o)
        {
            throw new NotImplementedException();
        }

        public int drainTo<V>(IEnumerable<V> c) where V : T
        {
            throw new NotImplementedException();
        }

        public int drainTo<V>(IEnumerable<V> c, int maxElements) where V : T
        {
            throw new NotImplementedException();
        }
    }
}
