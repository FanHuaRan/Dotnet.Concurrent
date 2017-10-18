using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dotnet.Concurrent.Atomic
{
    public class AtomicIneger
    {
        private static readonly int DEFAULT_INITAL_VALUE;
        private volatile int value;

        public int Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public int incrementAndGet()
        {
            int newValue = -1;
            while (true)
            {
                int oldValue = value;
                newValue = oldValue + 1;
                if (++value == newValue)
                {
                    break;
                }
            }
            return newValue;
        }

        public AtomicIneger(int inital)
        {
            this.value = inital;
        }
        public AtomicIneger()
            : this(DEFAULT_INITAL_VALUE)
        {

        }
    }
}
