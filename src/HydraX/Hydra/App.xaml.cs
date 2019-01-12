using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using HydraLib;

namespace HydraX
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Exports selected assets on double click
        /// </summary>
        private void ListViewItemMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Get Main Window
            MainWindow mainWindow = (MainWindow)Current.MainWindow;
            // Get listed assets
            var assets = mainWindow.AssetListView.SelectedItems.Cast<Asset>().ToList();
            // Check for hits
            if (assets.Count == 0)
            {
                MessageBox.Show("No assets selected to export.", "HydraX", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Send to exporter
            mainWindow.ExportAssets(assets);
        }
    }
}
