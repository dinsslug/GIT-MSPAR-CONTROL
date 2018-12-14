using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Nemont.Explorer
{
    internal class MarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new Thickness(-System.Convert.ToDouble(value), 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    internal class StatusIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try {
                return new BitmapImage(new Uri(StatusManager.Status[(int)value]));
            }
            catch {
                return new BitmapImage(new Uri("pack://application:,,,/Nemont.WPF.Extensions;Component/Asset/status_empty.png"));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value as BitmapImage).UriSource.AbsolutePath;
        }
    }

    internal class IconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try {
                return new BitmapImage(new Uri(value.ToString()));
            }
            catch {
                return new BitmapImage(new Uri("pack://application:,,,/Nemont.WPF.Extensions;Component/Asset/icon_empty.png"));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value as BitmapImage).UriSource.AbsolutePath;
        }
    }
}