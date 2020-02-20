using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
  public class CollapsingContentControl : ContentControl
  {
    public static readonly DependencyProperty CanCollapseProperty = DependencyProperty.Register(
      "CanCollapse", typeof(bool), typeof(CollapsingContentControl), new PropertyMetadata(default(bool)));

    public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register(
      "Direction", typeof(CollapseDirection), typeof(CollapsingContentControl), new PropertyMetadata(default(CollapseDirection)));

    public CollapseDirection Direction
    {
      get => (CollapseDirection) GetValue(DirectionProperty);
      set => SetValue(DirectionProperty, value);
    }

    public bool CanCollapse
    {
      get => (bool) GetValue(CanCollapseProperty);
      set => SetValue(CanCollapseProperty, value);
    }

    static CollapsingContentControl()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(CollapsingContentControl), new FrameworkPropertyMetadata(typeof(CollapsingContentControl)));
    }
  }

  public enum CollapseDirection
  {
    None,
    Left,
    Right,
    Top,
  }
}
