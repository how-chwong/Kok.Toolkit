using System.Windows;
using System.Windows.Controls;

namespace Kok.Test.WpfDemo.Controls;

public class HamburgerMenuItem : RadioButton
{
    static HamburgerMenuItem()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(HamburgerMenuItem), new FrameworkPropertyMetadata(typeof(HamburgerMenuItem)));
    }

    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public static readonly DependencyProperty TextProperty =
         DependencyProperty.Register(nameof(Text), typeof(string), typeof(HamburgerMenuItem), new PropertyMetadata(string.Empty));
}
