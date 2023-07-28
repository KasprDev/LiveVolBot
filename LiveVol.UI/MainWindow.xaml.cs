﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Data;
using DataGridExtensions;
using LiveVol.UI.Service;

namespace LiveVol.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Browser _browser;
        private HashSet<LiveVolData> _data = new HashSet<LiveVolData>() {};
        public ICollectionView CollectionView { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            CollectionView = CollectionViewSource.GetDefaultView(_data);
            CollectionView.SortDescriptions.Add(
                new SortDescription("Time", ListSortDirection.Descending));
            CollectionView.SortDescriptions.Add(
                new SortDescription("Date", ListSortDirection.Descending));
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
                MainGrid.ItemsSource = CollectionView;
            });

            _browser.OnRowChanged += _browser_OnRowChanged;
        }

        private void _browser_OnRowChanged(List<LiveVolData> data)
        {
            data.ForEach(x => _data.Add(x));
            MainGrid.Dispatcher.Invoke(() =>
            {
                CollectionView.Refresh();
            });
        }
    }
}
