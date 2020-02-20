using System;
using System.Collections.Generic;
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

    private bool isDragging;
    //private Point anchor;
    private Point horizontalSnapPoint;
    private Point verticalSnapPoint;

    private double anchorX;
    private double anchorY;

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

      CaptureMouse();
      e.Handled = true;
      var anchor = window.PointToScreen(Mouse.GetPosition(window));
      anchorX = anchor.X;
      anchorY = anchor.Y;

      Left = Left == DockState.Docked ? DockState.Docking : DockState.Free;
      Right = Right == DockState.Docked ? DockState.Docking : DockState.Free;
      Top = Top == DockState.Docked ? DockState.Docking : DockState.Free;
      isDragging = true;
    }

    private void WindowOnMouseUp(object sender, MouseButtonEventArgs e)
    {
      isDragging = false;

      Left = Left == DockState.Docking ? DockState.Docked : DockState.Free;
      Right = Right == DockState.Docking ? DockState.Docked : DockState.Free;
      Top = Top == DockState.Docking ? DockState.Docked : DockState.Free;

      ReleaseMouseCapture();
    }

    private void WindowOnMouseMove(object sender, MouseEventArgs e)
    {
      if (!isDragging)
        return;

      var currentPoint = PointToScreen(e.GetPosition(this));

      Left = UpdateDraggingState(currentPoint, Left, screens[0].TopX);
      Right = UpdateDraggingState(currentPoint, Right, screens[0].TopX+screens[0].Width-(int)window.Width);
      Top = UpdateTopDockState(currentPoint, Top, screens[0].TopY);

      if (Left == DockState.Free && Right == DockState.Free)
      {
        window.Left = window.Left + currentPoint.X - anchorX;
        anchorX = currentPoint.X;
      }

      if (Top == DockState.Free)
      {
        window.Top = window.Top + currentPoint.Y - anchorY;
        anchorY = currentPoint.Y;
      }
    }

    private DockState UpdateDraggingState(Point currentPoint, DockState currentDockState, int dockLine)
    {
      if (currentDockState == DockState.Free)
      {
        if (Math.Abs(window.Left - dockLine) < 25)
        {
          currentDockState = DockState.Docking;
          horizontalSnapPoint = currentPoint;
          window.Left = dockLine;
        }
      }
      else //Snapped
      {
        var delta = horizontalSnapPoint - currentPoint;
        if (Math.Abs(delta.X) > 50)
          currentDockState = DockState.Free;
      }

      return currentDockState;
    }

    private DockState UpdateTopDockState(Point currentPoint, DockState currentDockState, int dockLine)
    {
      if (currentDockState == DockState.Free)
      {
        if (Math.Abs(window.Top - dockLine) < 25)
        {
          currentDockState = DockState.Docking;
          verticalSnapPoint = currentPoint;
          window.Top = dockLine;
        }
      }
      else //Snapped
      {
        var delta = verticalSnapPoint - currentPoint;
        if (Math.Abs(delta.Y) > 50)
          currentDockState = DockState.Free;
      }

      return currentDockState;
    }
  }
}
