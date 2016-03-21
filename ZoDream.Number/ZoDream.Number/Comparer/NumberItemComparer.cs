using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Number.Model;

namespace ZoDream.Number.Comparer
{
    public class NumberItemComparer : IEqualityComparer<NumberItem>
    {
        public bool Equals(NumberItem x, NumberItem y)
        {
            return x.Number == y.Number;
        }

        public int GetHashCode(NumberItem obj)
        {
            return base.GetHashCode();
        }
    }
}
