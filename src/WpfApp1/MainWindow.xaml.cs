using System.Windows;
using System.Windows.Input;

namespace WpfApp1
{
  public partial class MainWindow
  {
    public MainWindow()
    {
      InitializeComponent();
      //MouseDown += OnMouseDown;
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
        DragMove();
    }
  }
}
