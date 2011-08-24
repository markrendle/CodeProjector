using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MarkRendle.CodeProjector
{
    /// <summary>
    /// Interaction logic for ProjectorWindow.xaml
    /// </summary>
    public partial class ProjectorWindow : Window
    {
        public ProjectorWindow(Brush brush)
        {
            InitializeComponent();
            TheGrid.Background = brush;
        }

        private void FullScreenCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            if (FullScreenCheckbox.IsChecked.GetValueOrDefault())
            {
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
                ResizeMode = ResizeMode.NoResize;
            }
            else
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = WindowState.Normal;
                ResizeMode = ResizeMode.CanResize;
            }
        }
    }
}
