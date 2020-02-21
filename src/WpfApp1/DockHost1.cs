﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp1
{
  public class DockHost1 : ContentControl
  {
    private readonly object lockObject = new object();
    private Window window;
    private List<Monitors.Screen> screens;

    public static readonly DependencyProperty LeftProperty = DependencyProperty.Register(
      "Left", typeof(DockState1), typeof(DockHost1), new PropertyMetadata(default(DockState1)));

    public static readonly DependencyProperty RightProperty = DependencyProperty.Register(
      "Right", typeof(DockState1), typeof(DockHost1), new PropertyMetadata(default(DockState1)));

    public static readonly DependencyProperty TopProperty = DependencyProperty.Register(
      "Top", typeof(DockState1), typeof(DockHost1), new PropertyMetadata(default(DockState1)));

    private bool isDragging;
    //private Point anchor;
    private Point horizontalSnapPoint;
    private Point verticalSnapPoint;

    private double anchorX;
    private double anchorY;

    public DockState1 Top
    {
      get => (DockState1)GetValue(TopProperty);
      set => SetValue(TopProperty, value);
    }

    public DockState1 Right
    {
      get => (DockState1)GetValue(RightProperty);
      set => SetValue(RightProperty, value);
    }

    public DockState1 Left
    {
      get => (DockState1)GetValue(LeftProperty);
      set => SetValue(LeftProperty, value);
    }

    static DockHost1()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(DockHost1), new FrameworkPropertyMetadata(typeof(DockHost1)));
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

      Left = Left == DockState1.Docked ? DockState1.Docking : DockState1.Free;
      Right = Right == DockState1.Docked ? DockState1.Docking : DockState1.Free;
      Top = Top == DockState1.Docked ? DockState1.Docking : DockState1.Free;
      isDragging = true;
    }

    private void WindowOnMouseUp(object sender, MouseButtonEventArgs e)
    {
      isDragging = false;

      Left = Left == DockState1.Docking ? DockState1.Docked : DockState1.Free;
      Right = Right == DockState1.Docking ? DockState1.Docked : DockState1.Free;
      Top = Top == DockState1.Docking ? DockState1.Docked : DockState1.Free;

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

      if (Left == DockState1.Free && Right == DockState1.Free)
      {
        window.Left = window.Left + currentPoint.X - anchorX;
        anchorX = currentPoint.X;
      }

      if (Top == DockState1.Free)
      {
        window.Top = window.Top + currentPoint.Y - anchorY;
        anchorY = currentPoint.Y;
      }
    }

    private DockState1 UpdateDraggingState(Point currentPoint, DockState1 currentDockState, int dockLine)
    {
      if (currentDockState == DockState1.Free)
      {
        if (Math.Abs(window.Left - dockLine) < 25)
        {
          currentDockState = DockState1.Docking;
          horizontalSnapPoint = currentPoint;
          window.Left = dockLine;
        }
      }
      else //Snapped
      {
        var delta = horizontalSnapPoint - currentPoint;
        if (Math.Abs(delta.X) > 50)
          currentDockState = DockState1.Free;
      }

      return currentDockState;
    }

    private DockState1 UpdateTopDockState(Point currentPoint, DockState1 currentDockState, int dockLine)
    {
      if (currentDockState == DockState1.Free)
      {
        if (Math.Abs(window.Top - dockLine) < 25)
        {
          currentDockState = DockState1.Docking;
          verticalSnapPoint = currentPoint;
          window.Top = dockLine;
        }
      }
      else //Snapped
      {
        var delta = verticalSnapPoint - currentPoint;
        if (Math.Abs(delta.Y) > 50)
          currentDockState = DockState1.Free;
      }

      return currentDockState;
    }
  }
}