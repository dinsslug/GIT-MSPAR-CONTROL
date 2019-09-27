using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Nemont.WPF.Controls.Explorer;

namespace Nemont.WPF.Service
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

    public class StatusIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var index = System.Convert.ToInt32(value);
            var uri = StatusManager.Status[index];
            var defaultUri = "pack://application:,,,/Nemont.WPF.Controls.Explorer;Component/Asset/status_empty.png";

            try {
                if (!string.IsNullOrEmpty(uri) && Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute)) {
                    return new BitmapImage(new Uri(uri));
                }
                return new BitmapImage(new Uri(defaultUri));
            }
            catch {
                return new BitmapImage(new Uri(defaultUri));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value as BitmapImage).UriSource.AbsolutePath;
        }
    }

    public class IconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var uri = value.ToString();
            var defaultUri = "pack://application:,,,/Nemont.WPF.Controls.Explorer;Component/Asset/icon_empty.png";
            
            try {
                if (!string.IsNullOrEmpty(uri) && Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute)) {
                    return new BitmapImage(new Uri(uri));
                }
                return new BitmapImage(new Uri(defaultUri));
            }
            catch {
                return new BitmapImage(new Uri(defaultUri));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value as BitmapImage).UriSource.AbsolutePath;
        }
    }
}