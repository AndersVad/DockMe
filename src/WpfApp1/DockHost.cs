using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp1
{
  public class DockHost : ContentControl
  {
    private Window window;
    private double anchorX;
    private double anchorY;
    private double snapX;
    private double snapY;
    private bool isDragging;
    private IEnumerable<int> leftEdges; //todo refactor into own class and subscribe to changed screen size events
    private IEnumerable<int> rightEdges;//
    private IEnumerable<int> topEdges;  //

    public static readonly DependencyProperty IsStickingLeftProperty = DependencyProperty.Register(
      "IsStickingLeft", typeof(bool), typeof(DockHost), new PropertyMetadata(default(bool)));

    public static readonly DependencyProperty IsStickingRightProperty = DependencyProperty.Register(
      "IsStickingRight", typeof(bool), typeof(DockHost), new PropertyMetadata(default(bool)));

    public static readonly DependencyProperty IsStickingTopProperty = DependencyProperty.Register(
      "IsStickingTop", typeof(bool), typeof(DockHost), new PropertyMetadata(default(bool)));

    public static readonly DependencyProperty DockEdgeProperty = DependencyProperty.Register(
      "DockEdge", typeof(DockEdge), typeof(DockHost), new PropertyMetadata(default(DockEdge)));

    public static readonly DependencyProperty IsPinnedProperty = DependencyProperty.Register(
      "IsPinned", typeof(bool), typeof(DockHost), new PropertyMetadata(default(bool)));

    public bool IsPinned
    {
      get => (bool)GetValue(IsPinnedProperty);
      set => SetValue(IsPinnedProperty, value);
    }

    public DockEdge DockEdge
    {
      get => (DockEdge)GetValue(DockEdgeProperty);
      set => SetValue(DockEdgeProperty, value);
    }

    public bool IsStickingTop
    {
      get => (bool)GetValue(IsStickingTopProperty);
      set => SetValue(IsStickingTopProperty, value);
    }

    public bool IsStickingRight
    {
      get => (bool)GetValue(IsStickingRightProperty);
      set => SetValue(IsStickingRightProperty, value);
    }

    public bool IsStickingLeft
    {
      get => (bool)GetValue(IsStickingLeftProperty);
      set => SetValue(IsStickingLeftProperty, value);
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

      
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
      base.OnRenderSizeChanged(sizeInfo);
      var screens = Monitors.GetScreens();
      leftEdges = screens.Select(screen => screen.TopX - (int)Margin.Left).ToList();
      rightEdges = screens.Select(screen => screen.TopX + screen.Width - (int)window.ActualWidth + (int)Margin.Right).ToList();
      topEdges = screens.Select(screen => screen.TopY - (int)Margin.Top).ToList();
    }

    private void WindowOnMouseMove(object sender, MouseEventArgs e)
    {
      if (!isDragging)
        return;

      var currentPoint = PointToScreen(e.GetPosition(this));
      UpdateSticking(currentPoint);
      MoveWindow(currentPoint);
    }

    private void WindowOnMouseUp(object sender, MouseButtonEventArgs e)
    {
      isDragging = false;
      DockEdge = GetDockEdge();
      IsStickingLeft = false;
      IsStickingRight = false;
      IsStickingTop = false;
      ReleaseMouseCapture();
    }

    private void WindowOnMouseDown(object sender, MouseButtonEventArgs e)
    {
      CaptureMouse();
      e.Handled = true;
      var anchor = window.PointToScreen(Mouse.GetPosition(window));
      anchorX = anchor.X;
      anchorY = anchor.Y;

      UpdateSticking(new Point(window.Left, window.Top));
      isDragging = true;
    }

    private void UpdateSticking(Point currentLocation)
    {
      IsStickingLeft = IsStickingHorizontally(currentLocation, IsStickingLeft, leftEdges);
      IsStickingRight = IsStickingHorizontally(currentLocation, IsStickingRight, rightEdges);
      IsStickingTop = IsStickingVertically(currentLocation, IsStickingTop, topEdges);
    }

    private bool IsStickingVertically(Point currentLocation, bool isCurrentlySticking, IEnumerable<int> edges)
    {
      if (isCurrentlySticking)
      {
        return IsWithinStickingDistance(snapY, currentLocation.Y);
      }

      foreach (var edge in edges)
      {
        if (!IsWithinSnappingDistance(edge, window.Top))
          continue;

        SnapVertically(edge, currentLocation.Y);
        return true;
      }

      return false;
    }

    private bool IsStickingHorizontally(Point currentLocation, bool isCurrentlySticking, IEnumerable<int> edges)
    {
      if (isCurrentlySticking)
      {
        return IsWithinStickingDistance(snapX, currentLocation.X);
      }

      foreach (var edge in edges)
      {
        if (!IsWithinSnappingDistance(edge, window.Left))
          continue;

        SnapHorizontally(edge, currentLocation.X);
        return true;
      }

      return false;
    }

    private static bool IsWithinStickingDistance(double snapLocation, double currentLocation)
    {
      var delta = Math.Abs(snapLocation - currentLocation);
      return !(delta > 50);
    }

    private static bool IsWithinSnappingDistance(double screenEdge, double windowEdge)
    {
      var distanceToEdge = Math.Abs(windowEdge - screenEdge);
      return distanceToEdge < 25;
    }

    private void MoveWindow(Point currentPoint)
    {
      if (!IsStickingLeft && !IsStickingRight)
      {
        window.Left = window.Left + currentPoint.X - anchorX;
        anchorX = currentPoint.X;
      }

      if (!IsStickingTop)
      {
        window.Top = window.Top + currentPoint.Y - anchorY;
        anchorY = currentPoint.Y;
      }
    }

    private void SnapHorizontally(double edgeLocation, double currentLocation)
    {
      snapX = currentLocation;
      window.Left = edgeLocation;
    }

    private void SnapVertically(double edgeLocation, double currentLocation)
    {
      snapY = currentLocation;
      window.Top = edgeLocation;
    }

    private DockEdge GetDockEdge()
    {
      if (IsStickingTop)
        return DockEdge.Top;
      if (IsStickingLeft)
        return DockEdge.Left;
      if (IsStickingRight)
        return DockEdge.Right;
      return DockEdge.None;
    }
  }
}
