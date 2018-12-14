using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nemont.Explorer
{
    public static class StatusManager
    {
        internal static Dictionary<int, string> Status = new Dictionary<int, string>();

        public static void Add(int mode, string iconUri)
        {
            try {
                Status.Add(mode, iconUri);
            }
            catch {
                throw;
            }
        }
    }
}
