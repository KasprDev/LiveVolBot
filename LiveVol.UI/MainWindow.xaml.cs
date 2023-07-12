using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Timers;
using System.Windows;
using LiveVol.UI.Service;

namespace LiveVol.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Browser _browser;
        private readonly HashSet<LiveVolData> _data = new HashSet<LiveVolData>() {};

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(Browser browser):this()
        {
            _browser = browser;
        }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            await _browser.Initialize();

            MainGrid.Dispatcher.Invoke(() =>
            {
                MainGrid.ItemsSource = _data;
            });

            _browser.OnRowChanged += _browser_OnRowChanged;
        }

        private void _browser_OnRowChanged(List<LiveVolData> data)
        {
            data.ForEach(x => _data.Add(x));

            MainGrid.Dispatcher.Invoke(() =>
            {
                MainGrid.Items.Refresh();
            });
        }
    }
}
