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
        private void ListViewItemMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Current.MainWindow;
            if(mainWindow.AssetListView.SelectedItem is Asset asset)
            {
                asset.ExportFunction(asset);
            }
        }
    }
}
