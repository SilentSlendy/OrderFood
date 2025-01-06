using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Converters
{
    class DecimalToCurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is decimal decimalValue)
            {
                
                return string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:C0}", decimalValue).Replace("₫", " ₫");
            }

            
            return "0 ₫";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string stringValue)
            {
                string sanitizedValue = stringValue.Replace("₫", "").Trim().Replace(",", "");
                if (decimal.TryParse(sanitizedValue, out decimal result))
                {
                    return result;
                }
            }

            return 0.0m;
        }
    }
}
