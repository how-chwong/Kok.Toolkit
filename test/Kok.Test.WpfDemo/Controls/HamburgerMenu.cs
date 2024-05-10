using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Kok.Test.WpfDemo.Controls;

public class HamburgerMenu : Control
{
    static HamburgerMenu()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(HamburgerMenu), new FrameworkPropertyMetadata(typeof(HamburgerMenu)));
    }

    public HamburgerMenu()
    {
        Width = 0;
    }

    public FrameworkElement Content
    {
        get { return (FrameworkElement)GetValue(ContentProperty); }
        set { SetValue(ContentProperty, value); }
    }

    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(nameof(Content), typeof(object), typeof(HamburgerMenu), new PropertyMetadata(null));

    public Duration OpenCloseDuration
    {
        get { return (Duration)GetValue(OpenCloseDurationProperty); }
        set { SetValue(OpenCloseDurationProperty, value); }
    }

    public static readonly DependencyProperty OpenCloseDurationProperty =
            DependencyProperty.Register(nameof(OpenCloseDuration), typeof(Duration), typeof(HamburgerMenu), new PropertyMetadata(Duration.Automatic));

    public double DefaultWidth
    {
        get { return (double)GetValue(DefaultWidthProperty); }
        set { SetValue(DefaultWidthProperty, value); }
    }

    public static readonly DependencyProperty DefaultWidthProperty =
        DependencyProperty.Register(nameof(DefaultWidth), typeof(double), typeof(HamburgerMenu), new PropertyMetadata(130.0));

    public bool IsOpen
    {
        get { return (bool)GetValue(IsOpenProperty); }
        set { SetValue(IsOpenProperty, value); }
    }

    public static readonly DependencyProperty IsOpenProperty =
        DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(HamburgerMenu), new PropertyMetadata(false, OnIsOpenedPropertyChanged));

    private static void OnIsOpenedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is HamburgerMenu menu) menu.ChangeMenuStatus();
    }

    private void ChangeMenuStatus()
    {
        if (IsOpen) OpenMenuAnimated();
        else CloseMenuAnimated();
    }

    private void CloseMenuAnimated()
    {
        var closing = new DoubleAnimation(0, OpenCloseDuration);
        BeginAnimation(WidthProperty, closing);
    }

    private void OpenMenuAnimated()
    {
        var opening = new DoubleAnimation(GetDesiredContentWidth(), OpenCloseDuration);
        BeginAnimation(WidthProperty, opening);
    }

    private double GetDesiredContentWidth()
    {
        if (Content == null) return DefaultWidth;
        Content.Measure(new Size(MaxWidth, MaxHeight));
        return Content.DesiredSize.Width < DefaultWidth ? DefaultWidth : Content.DesiredSize.Width;
    }
}
