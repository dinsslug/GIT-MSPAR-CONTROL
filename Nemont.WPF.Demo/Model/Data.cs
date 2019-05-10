using Nemont.Demo.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nemont.Demo.Model
{
    public class Data : ModelBase
    {
        private string name = "";
        private string type = "";
        private string desc = "";

        public string Name { get { return name; } set { name = value; OnPropertyChanged(nameof(Name)); } }
        public string Type { get { return type; } set { type = value; OnPropertyChanged(nameof(Type)); } }
        public string Desc { get { return desc; } set { desc = value; OnPropertyChanged(nameof(Desc)); } }

        public Data() { }
        public Data(string value1, string value2, string value3)
        {
            Name = value1;
            Type = value2;
            Desc = value3;
        }
    }
}
