using System.Collections.Generic;
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
        private readonly List<LiveVolData> _data;
        private readonly Browser _browser;

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(Browser browser, List<LiveVolData> data):this()
        {
            _browser = browser;
            _data = data;
        }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            await _browser.Initialize();
            var t = new Timer() { Interval = 1000 };
            t.Elapsed += T_Elapsed;
            t.Start();
        }

        private void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            MainGrid.Dispatcher.Invoke(() =>
            {
                MainGrid.ItemsSource = _data.ToArray().ToList();
            });
        }
    }
}
