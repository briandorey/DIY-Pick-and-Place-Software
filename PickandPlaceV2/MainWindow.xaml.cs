using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
namespace PickandPlaceV2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        usbDevice usbController;
        kflop _kflop;

        public MainWindow()
        {
            InitializeComponent();
            App MyApplication = ((App)Application.Current);
            _kflop = MyApplication.GetKFlop();

            usbController = MyApplication.GetUSBDevice();

        }
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get TabControl reference.
            var item = sender as TabControl;
            // ... Set Title to selected tab header.
            var selected = item.SelectedItem as TabItem;
           // this.Title = selected.Header.ToString();
        }

        private void Button_Home_Click(object sender, System.Windows.RoutedEventArgs e)
        {
          if (usbController != null)
            {
                usbController.setResetFeeder();
                _kflop.HomeAll();
            }
            
        }

        private void Button_FlushVac_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            
            if (usbController != null)
            {
                usbController.setVAC1(true);
                usbController.setVAC2(true);
            }
    }
    }
}
