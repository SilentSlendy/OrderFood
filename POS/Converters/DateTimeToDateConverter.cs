﻿using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Converters
{
    public class DateTimeToDateConverter: IValueConverter
    {
            public object Convert(object value, Type targetType, object parameter, string language)
        {
                if (value is DateTime dateTime)
                {
                    return dateTime.ToString("yyyy-MM-dd");
                }
                return null; 
            }

            public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
                throw new NotImplementedException("ConvertBack không được hỗ trợ trong converter này.");
            }
        }
    }

