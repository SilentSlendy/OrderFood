using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;

namespace POS.Converters
{
    /// <summary>
    /// Chuyển đổi số sang tiền tệ Việt Nam
    /// </summary>
    class NumberToVnCurrencyConverter : IValueConverter
    {
        /// <summary>
        /// Chuyển đổi số sang tiền tệ Việt Nam
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int number = (int)value;
            CultureInfo vietnamCulture = new CultureInfo("vi-VN");
            string formattedCurrency = number.ToString("C", vietnamCulture);

            return formattedCurrency;
        }

        /// <summary>
        /// Chuyển đổi tiền tệ Việt Nam sang số
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string strValue)
            {
                strValue = strValue.Replace("₫", "").Trim();
                if (int.TryParse(strValue, NumberStyles.Currency, new CultureInfo("vi-VN"), out int result))
                {
                    return result;
                }
            }
            throw new NotImplementedException();
        }
    }
}
