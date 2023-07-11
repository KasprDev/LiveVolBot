using System.Timers;
using System.Windows;
using System.Windows.Controls;
using LiveVol.UI.Service;
using TraderTape;

namespace LiveVol.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Browser _browser;
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            _browser = new Browser();
            await _browser.Initialize();

            var t = new Timer()
            {
                Interval = 1000
            };

            t.Elapsed += T_Elapsed;
            t.Start();
        }

        private void T_Elapsed(object? sender, ElapsedEventArgs e)
        {
            MainGrid.Dispatcher.Invoke(() =>
            {
                MainGrid.ItemsSource = _browser.GridRows;
            });
        }
    }
}
