using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp1
{
  public class DockHost : ContentControl
  {
    private readonly object lockObject = new object();
    private Window window;
    private List<Monitors.Screen> screens;

    public static readonly DependencyProperty LeftProperty = DependencyProperty.Register(
      "Left", typeof(DockState), typeof(DockHost), new PropertyMetadata(default(DockState)));

    public static readonly DependencyProperty RightProperty = DependencyProperty.Register(
      "Right", typeof(DockState), typeof(DockHost), new PropertyMetadata(default(DockState)));

    public static readonly DependencyProperty TopProperty = DependencyProperty.Register(
      "Top", typeof(DockState), typeof(DockHost), new PropertyMetadata(default(DockState)));

    public static readonly DependencyProperty SnapPixelsProperty = DependencyProperty.Register(
      "SnapPixels", typeof(int), typeof(DockHost), new PropertyMetadata(default(int)));

    private double leftMouseSnap;
    private int leftEdgeSnap;

    private bool isDragging;
    private Point anchor;
    private Point leftSnapPoint;

    public DockState Top
    {
      get => (DockState)GetValue(TopProperty);
      set => SetValue(TopProperty, value);
    }

    public DockState Right
    {
      get => (DockState)GetValue(RightProperty);
      set => SetValue(RightProperty, value);
    }

    public DockState Left
    {
      get => (DockState)GetValue(LeftProperty);
      set => SetValue(LeftProperty, value);
    }

    public int SnapPixels
    {
      get => (int)GetValue(SnapPixelsProperty);
      set => SetValue(SnapPixelsProperty, value);
    }


    static DockHost()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(DockHost), new FrameworkPropertyMetadata(typeof(DockHost)));
    }


    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      window = VisualTreeHelpers.FindAncestor<Window>(this);

      if (window == null)
        return;

      window.MouseDown += WindowOnMouseDown;
      window.MouseMove += WindowOnMouseMove;


      window.MouseUp += WindowOnMouseUp;

      screens = Monitors.GetScreens();
    }



    private void WindowOnMouseDown(object sender, MouseButtonEventArgs e)
    {
      isDragging = true;
      CaptureMouse();
      e.Handled = true;
      anchor = GetMousePosition();
      var delta = GetMouseWindowDelta();
    }

    private void WindowOnMouseUp(object sender, MouseButtonEventArgs e)
    {
      isDragging = false;

      Left = Math.Abs(window.Left - screens[0].TopX) < 0.5
        ? DockState.Docked
        : DockState.Free;
    }

    private void WindowOnMouseMove(object sender, MouseEventArgs e)
    {
      if (!isDragging)
        return;

      var currentPoint = PointToScreen(e.GetPosition(this));

      if (Left == DockState.Free)
      {
        if (Math.Abs(window.Left - screens[0].TopX) < 25)
        {
          Debug.WriteLine("SnappedLeft");
          Left = DockState.Docking;
          leftSnapPoint = currentPoint;
          window.Left = screens[0].TopX;
          return;
        }
      }
      else //Snapped
      {
        var delta = leftSnapPoint - currentPoint;
        Debug.WriteLine($"Delta {delta.X}");
        if (Math.Abs(delta.X) < 50)
          return;
        Left = DockState.Free;
      }

      window.Left = window.Left + currentPoint.X - anchor.X;
      window.Top = window.Top + currentPoint.Y - anchor.Y;
      anchor = currentPoint;

    }

    private Point GetMousePosition()
    {
      Debug.WriteLine($"GetMousePosition = {window.PointToScreen(Mouse.GetPosition(window)).X} : {window.PointToScreen(Mouse.GetPosition(window)).Y}");
      return window.PointToScreen(Mouse.GetPosition(window));
    }

    private Point GetMouseWindowDelta()
    {
      Debug.WriteLine($"GetMouseWindowDelta = {Mouse.GetPosition(window).X} : {Mouse.GetPosition(window).Y}");
      return Mouse.GetPosition(window);
    }
  }
}
