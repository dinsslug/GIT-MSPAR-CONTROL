using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Nemont.Model.ExplorerView
{
    public abstract class Item : Base
    {
        protected string iconUri;
        public BitmapImage Icon { get { return new BitmapImage(new Uri(iconUri)); } }

        public Item(string name, string iconUri) : base(name)
        {
            this.iconUri = iconUri;
        }
    }
}
