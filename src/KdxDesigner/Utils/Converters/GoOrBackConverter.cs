using System.Globalization;
using System.Windows.Data;

namespace KdxDesigner.Utils.Converters
{
    /// <summary>
    /// GoOrBack値を表示名に変換するコンバーター
    /// 0: Go&Back, 1: GoOnly, 2: BackOnly
    /// </summary>
    public class GoOrBackConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                return intValue switch
                {
                    0 => "Go&Back",
                    1 => "GoOnly",
                    2 => "BackOnly",
                    _ => intValue.ToString()
                };
            }
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue)
            {
                return strValue switch
                {
                    "Go&Back" => 0,
                    "GoOnly" => 1,
                    "BackOnly" => 2,
                    _ => 0
                };
            }
            return 0;
        }
    }
}
