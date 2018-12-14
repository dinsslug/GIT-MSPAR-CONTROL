using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nemont.Explorer.Model
{
    internal class FilterInfo
    {
        internal int Order;
        internal string IconUri;

        internal FilterInfo(int order, string iconUri)
        {
            Order = order;
            IconUri = iconUri;
        }
    }
}
