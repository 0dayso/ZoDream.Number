using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Number.Model;

namespace ZoDream.Number.Comparer
{
    public class UrlItemComparer : IEqualityComparer<UrlItem>
    {
        public bool Equals(UrlItem x, UrlItem y)
        {
            return x.Url.Split('#')[0].Equals(y.Url.Split('#')[0], StringComparison.CurrentCultureIgnoreCase);
        }

        public int GetHashCode(UrlItem obj)
        {
            return base.GetHashCode();
        }
    }
}
