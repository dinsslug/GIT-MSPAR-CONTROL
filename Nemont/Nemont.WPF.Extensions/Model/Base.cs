using Nemont.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Nemont.Model.ExplorerView
{
    public abstract class Base : INotifyPropertyChanged
    {
        // Binding Variables
        protected string name = "";
        protected FontWeight selectedFontWeight = FontWeights.Normal;
        ///protected CheckMode checkMode = CheckMode.EMPTY;
        protected bool isNodeSelected = false;
        public string Name { get { return name; } set { name = value; OnPropertyChanged("Name"); } }
        public FontWeight SelectedFontWeight { get { return selectedFontWeight; } set { selectedFontWeight = value; OnPropertyChanged("SelectedFontWeight"); } }
        ///public CheckMode CheckMode { get { return checkMode; } set { checkMode = value; OnPropertyChanged("CheckMode"); } }
        public bool IsNodeSelected { get { return isNodeSelected; } set { isNodeSelected = value; OnPropertyChanged("IsNodeSelected"); } }

        public RelayCommand RcClick { get; set; }
        public RelayCommand RcRightClick { get; set; }

        public Action Click;
        public Action RightClick;
        
        private void OnClick()
        {
            Click?.Invoke();
        }

        private void OnRightClick()
        {
            RightClick?.Invoke();
        }

        protected Base(string name)
        {
            Name = name;

            RcClick = new RelayCommand(OnClick);
            RcRightClick = new RelayCommand(OnRightClick);
        }

        internal void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
