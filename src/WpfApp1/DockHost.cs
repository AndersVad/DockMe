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

    public int SnapPixels
    {
      get => (int)GetValue(SnapPixelsProperty);
      set => SetValue(SnapPixelsProperty, value);
    }


    static DockHost()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(DockHost), new FrameworkPropertyMetadata(typeof(DockHost)));
    }

 private void WindowOnLocationChanged(object sender, EventArgs e)
    {
      Update();
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      window = VisualTreeHelpers.FindAncestor<Window>(this);

      if (window == null)
        return;

      window.MouseDown += WindowOnMouseDown;
      
      window.MouseUp += WindowOnMouseUp;
      window.LocationChanged += WindowOnLocationChanged;

      screens = Monitors.GetScreens();
    }

    private void WindowOnMouseDown(object sender, MouseButtonEventArgs e)
    {
      if (Left == DockState.Docking)
        return;
      if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
        window.DragMove();
    }

    private void WindowOnMouseUp(object sender, MouseButtonEventArgs e)
    {
      Left = Math.Abs(window.Left - screens[0].TopX) < 0.5
        ? DockState.Docked
        : DockState.Undocked;
    }


    private void Update()
    {
      lock (lockObject)
      {

        //made it this far, means we are dragging
        if (Left != DockState.Docking)
        {
          Debug.WriteLine("Not Docking");
          //first time dragging
          if (Math.Abs(window.Left - screens[0].TopX) < 5)
          {
            Debug.WriteLine("***DOCKING***");
            Left = DockState.Docking;
            leftEdgeSnap = screens[0].TopX;
            leftMouseSnap = window.PointToScreen(Mouse.GetPosition(window)).X;
          }

          return;
        }

        //here, we are docking and snapping
        var currentMousePosition = window.PointToScreen(Mouse.GetPosition(window));
        var mouseDiff = leftMouseSnap - currentMousePosition.X;
        if (Math.Abs(mouseDiff) < 20)
        {
          Debug.WriteLine(currentMousePosition);
          window.Left = leftEdgeSnap;
        }
        else
        {
          Debug.WriteLine("------undocked------");
          Left = DockState.Undocked;
          window.Left += mouseDiff;
        }

      }

    }
  }
}
