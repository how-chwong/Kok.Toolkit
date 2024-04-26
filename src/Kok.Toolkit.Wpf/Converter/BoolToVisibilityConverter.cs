using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Kok.Toolkit.Wpf.Converter;

/// <summary>
/// 布尔值到Visible的转换器
/// </summary>
public class BoolToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// 将布尔值转为Visibility枚举
    /// </summary>
    /// <param name="value">待转换的布尔值</param>
    /// <param name="targetType">目标类型</param>
    /// <param name="parameter">是否取反，布尔值，若为true，则可见性与value值相反，否则相同</param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool result) return Visibility.Collapsed;
        if (parameter is not bool isReverse) return result ? Visibility.Visible : Visibility.Collapsed;
        if (isReverse) return result ? Visibility.Collapsed : Visibility.Visible;
        return result ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <summary>
    /// 将Visibility枚举转为布尔值
    /// </summary>
    /// <param name="value">待转换的Visibility枚举值</param>
    /// <param name="targetType">目标类型</param>
    /// <param name="parameter">是否取反，布尔值，若为true，则布尔值与value值相反，否则相同</param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not Visibility result) return false;
        if (parameter is not bool isReverse) return result == Visibility.Visible;
        if (isReverse) return result != Visibility.Visible;
        return result == Visibility.Visible;
    }
}
