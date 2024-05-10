using System.Windows;
using System.Windows.Controls;

namespace Kok.Test.WpfDemo.Controls;

public class HamburgerMenuItem : RadioButton
{
    static HamburgerMenuItem()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(HamburgerMenuItem), new FrameworkPropertyMetadata(typeof(HamburgerMenuItem)));
    }
}
