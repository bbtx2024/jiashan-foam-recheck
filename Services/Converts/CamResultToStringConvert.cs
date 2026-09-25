using System;
using System.Globalization;
using System.Windows.Data;
using QA.Business.Define;

namespace QA.Business.Converts
{
    public class CamResultToStringConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((EN_CamResult)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
