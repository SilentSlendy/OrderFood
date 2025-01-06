using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Profile;

namespace POS.Converters
{
    class InvoiceIDToParagraphConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string result;
            if (value is int invoiceID)
            {
                if (invoiceID == -1)
                {
                    result = "Hóa đơn mới";
                }
                else
                {
                    result = $"Mã hóa đơn: #{invoiceID}";
                }
            }
            else result = string.Empty;
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string inputString)
            {
                if (inputString == "Hóa đơn mới")
                {
                    return -1;
                }
                if (inputString.StartsWith("Mã hóa đơn: #"))
                {
                    string idString = inputString.Replace("Mã hóa đơn: #", "").Trim();

                    if (int.TryParse(idString, out int invoiceID))
                    {
                        return invoiceID;
                    }
                }
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
