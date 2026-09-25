using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using QA.Business.Define;

namespace QA.Business.Converts
{
    public class TrayStatusToColorConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((EN_TrayStatus)value)
            {
                case EN_TrayStatus.禁用:
                case EN_TrayStatus.空穴:
                    return Brushes.LightGray;
                case EN_TrayStatus.待料:
                case EN_TrayStatus.等待处理:
                    return Brushes.AliceBlue;
                case EN_TrayStatus.OK:
                case EN_TrayStatus.拍照完成:
                    return Brushes.LightGreen;
                default:
                    return Brushes.OrangeRed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TrayStatusToStringConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((EN_TrayStatus)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
