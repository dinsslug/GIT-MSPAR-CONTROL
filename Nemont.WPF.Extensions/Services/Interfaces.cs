using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nemont.Explorer
{
    internal interface IFile
    {
        string RelativePath { get; set; }
    }
}
