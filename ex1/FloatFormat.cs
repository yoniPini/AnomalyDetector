using System;
using System.Globalization;
using System.Windows.Data;
namespace ex1
{
    // class which was designed to become compoment in the way to show data in the xaml
    // [using is via xaml] for example:

    //xaml:
    // 1)in the header
    //xmlns:local="clr-namespace:ex1"
    // 2)
    /* <Window.Resources>
        <local:FloatFormat x:Key="Precision8"/>
    </Window.Resources> */
    // 3)
    // Text="{Binding ViewModel.DoubleProperty, Converter={StaticResource Precision8}, Mode=OneWay}"
    public class FloatFormat : IValueConverter
    {
        // get double and show it as "dddd.dddd" OR "-dddd.dddd" where d are (different) digits
        private string helper(double x)
        {
            string sign = x < 0 ? "-" : "";
            x = Math.Abs(x);
            x = x * 10000;
            string s = "000000000000" + ((long)x);
            s = s.Insert(s.Length - 4, ".");
            return sign + s.Substring(s.Length - 9);
        }

        // IValueConverter:

        // for Mode=OneWay, Mode=TwoWays
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is float) return helper((float)value);
            if (value is double) return helper((double)value);
            return "0000.0000";
        }

        // for Mode=OneWayToSource
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
