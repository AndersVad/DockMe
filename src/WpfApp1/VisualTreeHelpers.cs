using System.Windows;
using System.Windows.Media;

namespace WpfApp1
{
  internal class VisualTreeHelpers
  {
    public static T FindAncestor<T>(DependencyObject current)
      where T : DependencyObject
    {
      current = VisualTreeHelper.GetParent(current);

      while (current != null)
      {
        if (current is T dependencyObject)
        {
          return dependencyObject;
        }
        current = VisualTreeHelper.GetParent(current);
      }
      return null;
    }
  }
}