using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Nemont.WPF.AppService;
using Nemont.WPF.AppService.Threading;

namespace Nemont.Demo
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        public static LogDialogFactory Log;
        public static MessageTask Task => Log.Task;
    }
}
