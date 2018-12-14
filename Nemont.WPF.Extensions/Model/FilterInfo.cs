using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nemont.Explorer.Model
{
    internal class FilterInfo
    {
        internal Type Type;
        internal int Order;

        internal FilterInfo(Type type, int order)
        {
            Type = type;
            Order = order;
        }
    }
}
