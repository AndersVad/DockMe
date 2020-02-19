using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
  public class DockHost : ContentControl
  {
    public static readonly DependencyProperty LeftProperty = DependencyProperty.Register(
      "Left", typeof(DockState), typeof(DockHost), new PropertyMetadata(default(DockState)));

    public static readonly DependencyProperty RightProperty = DependencyProperty.Register(
      "Right", typeof(DockState), typeof(DockHost), new PropertyMetadata(default(DockState)));

    public static readonly DependencyProperty TopProperty = DependencyProperty.Register(
      "Top", typeof(DockState), typeof(DockHost), new PropertyMetadata(default(DockState)));

    private Window window;
    private List<Monitors.Screen> screens;

    public DockState Top
    {
      get => (DockState) GetValue(TopProperty);
      set => SetValue(TopProperty, value);
    }

    public DockState Right
    {
      get => (DockState) GetValue(RightProperty);
      set => SetValue(RightProperty, value);
    }

    public DockState Left
    {
      get => (DockState) GetValue(LeftProperty);
      set => SetValue(LeftProperty, value);
    }

    static DockHost()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(DockHost), new FrameworkPropertyMetadata(typeof(DockHost)));
    }

    public DockHost()
    {
     
    }

    private void WindowOnLocationChanged(object sender, EventArgs e)
    {
      if (window.Left < screens[0].TopX)
        Left = DockState.Docked;
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      window = VisualTreeHelpers.FindAncestor<Window>(this);

      if (window == null)
        return;

      window.LocationChanged += WindowOnLocationChanged;

      screens = Monitors.GetScreens();
    }

  }
}
