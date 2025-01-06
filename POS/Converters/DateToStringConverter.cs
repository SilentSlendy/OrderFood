using System;
using Microsoft.UI.Xaml;
using System.Globalization;
using Microsoft.UI.Xaml.Data;

namespace POS.Converters
{
    /// <summary>
    /// Chuyển đổi ngày thành chuỗi và ngược lại
    /// </summary>
    public class DateToStringConverter : IValueConverter
    {
        /// <summary>
        /// Chuyển đổi ngày thành chuỗi
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime date)
            {
                return date.ToString("dd/MM/yyyy"); 
            }
            return string.Empty;
        }

        /// <summary>
        /// Chuyển đổi chuỗi thành ngày
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string dateString)
            {
                if (DateTime.TryParseExact(
                    dateString,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime date))
                {
                    return date;
                }
            }
            return DependencyProperty.UnsetValue; // Indicate that conversion failed
        }
    }
}
