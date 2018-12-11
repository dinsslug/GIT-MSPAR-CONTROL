using MsparControlDemo.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsparControlDemo.Model
{
    public class Data : ModelBase
    {
        private string value1 = "";
        private string value2 = "";
        private string value3 = "";

        public string Value1 { get { return value1; } set { value1 = value; OnPropertyChanged("Value1"); } }
        public string Value2 { get { return value2; } set { value2 = value; OnPropertyChanged("Value2"); } }
        public string Value3 { get { return value3; } set { value3 = value; OnPropertyChanged("Value3"); } }

        public Data() { }
        public Data(string value1, string value2, string value3)
        {
            Value1 = value1;
            Value2 = value2;
            Value3 = value3;
        }
    }
}
