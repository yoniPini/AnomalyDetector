using System;
using System.Globalization;
using System.Windows.Data;
namespace ex1
{
    //xaml:
     //    xmlns:local="clr-namespace:ex1"

    /* <Window.Resources>
        <local:ConvertorNiceString x:Key="Precision8"/>
    </Window.Resources> */

    // Text="{Binding qf, Converter={StaticResource Precision8}, Mode=OneWay}"
    public class FloatFormat : IValueConverter
    {
        private string helper(double x)
        {
            string sign = x < 0 ? "-" : "";
            x = Math.Abs(x);
            x = x * 10000;
            string s = "000000000000" + ((long)x);
            s = s.Insert(s.Length - 4, ".");
            return sign + s.Substring(s.Length - 9);
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //float s = value as float;
            if (value is float) return helper((float)value);
            if (value is double) return helper((double)value);
            return "0000.0000";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
