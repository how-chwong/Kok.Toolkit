using System.Windows.Controls;
using System.Windows;

namespace Kok.Toolkit.Wpf.Control
{
    /// <summary>
    /// 有渲染大小的画布
    /// </summary>
    public class AutoSizeCanvas : Canvas
    {
        ///<inheritdoc />
        protected override Size MeasureOverride(System.Windows.Size constraint)
        {
            base.MeasureOverride(constraint);
            double width = base
                .InternalChildren
                .OfType<UIElement>()
                .Max(i => i.RenderSize.Width);

            double height = base
                .InternalChildren
                .OfType<UIElement>()
                .Max(i => i.RenderSize.Height);

            return new Size(width, height);
        }
    }
}
