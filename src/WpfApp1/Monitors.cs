using System;
using System.Collections.Generic;

namespace WpfApp1
{
  //https://stackoverflow.com/a/35681472
  /// <summary>
  /// This class deals with monitors.
  /// </summary>
  internal static class Monitors
  {
    private static List<Screen> screens;

    internal static List<Screen> GetScreens()
    {
      screens = new List<Screen>();

      var handler = new NativeMethods.DisplayDevicesMethods.EnumMonitorsDelegate(MonitorEnumProc);
      NativeMethods.DisplayDevicesMethods.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, handler, IntPtr.Zero); // should be sequential

      return screens;
    }

    private static bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, NativeMethods.DisplayDevicesMethods.RECT rect, IntPtr dwData)
    {
      NativeMethods.DisplayDevicesMethods.MONITORINFO mi = new NativeMethods.DisplayDevicesMethods.MONITORINFO();

      if (NativeMethods.DisplayDevicesMethods.GetMonitorInfo(hMonitor, mi))
      {
        screens.Add(new Screen(
          (mi.dwFlags & 1) == 1, // 1 = primary monitor
          mi.rcMonitor.Left - 7, //todo bug, why is this offset necessary?
          mi.rcMonitor.Top,
          Math.Abs(mi.rcMonitor.Right - mi.rcMonitor.Left),
          Math.Abs(mi.rcMonitor.Bottom - mi.rcMonitor.Top)));
      }

      return true;
    }

    /// <summary>
    /// Represents a display device on a single system.
    /// </summary>
    internal sealed class Screen
    {
      /// <summary>
      /// Initializes a new instance of the Screen class.
      /// </summary>
      /// <param name="primary">A value indicating whether the display is the primary screen.</param>
      /// <param name="x">The display's top corner X value.</param>
      /// <param name="y">The display's top corner Y value.</param>
      /// <param name="w">The width of the display.</param>
      /// <param name="h">The height of the display.</param>
      internal Screen(bool primary, int x, int y, int w, int h)
      {
        IsPrimary = primary;
        TopX = x;
        TopY = y;
        Width = w;
        Height = h;
      }

      /// <summary>
      /// Gets a value indicating whether the display device is the primary monitor.
      /// </summary>
      internal bool IsPrimary { get; }

      /// <summary>
      /// Gets the display's top corner X value.
      /// </summary>
      internal int TopX { get; }

      /// <summary>
      /// Gets the display's top corner Y value.
      /// </summary>
      internal int TopY { get; }

      /// <summary>
      /// Gets the width of the display.
      /// </summary>
      internal int Width { get; }

      /// <summary>
      /// Gets the height of the display.
      /// </summary>
      internal int Height { get; }
    }
  }
}