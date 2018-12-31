using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using HydraLib;
using PhilLibX;

namespace HydraX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Whether or not we need to end any active threads
        /// </summary>
        public bool EndThread = false;

        /// <summary>
        /// Active Log
        /// </summary>
        Logger ActiveLogger = new Logger("HydraX", "HydraLog.txt");

        /// <summary>
        /// List View (For Filtering)
        /// </summary>
        public static CollectionView View { get; set; }

        /// <summary>
        /// Initializes Main Window
        /// </summary>
        public MainWindow()
        {
            // Put the updater on a different thread
            new Thread(() =>
            {
                // Check for updates, basic but does the job
                if(HydraUpdater.CheckForUpdates(Assembly.GetExecutingAssembly().GetName().Version))
                {
                    // Tell the chere we has update
                    var result = MessageBox.Show("A new version of HydraX is available, do you want to download it now?", "Update Available", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    // Check do we want to go to release
                    if (result == MessageBoxResult.Yes)
                        System.Diagnostics.Process.Start("https://github.com/Scobalula/HydraX/releases");
                }
            }).Start();
            InitializeComponent();
        }

        /// <summary>
        /// Filters Sounds by text in Search Box
        /// </summary>
        public bool ViewFilter(object obj)
        {
            return string.IsNullOrEmpty(SearchBox.Text) ? true : (obj.ToString().IndexOf(SearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        /// <summary>
        /// Updates View Filter on Search
        /// </summary>
        private void SearchBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(AssetListView.ItemsSource)?.Refresh();
        }

        /// <summary>
        /// Loads Assets from the Game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadGameClick(object sender, RoutedEventArgs e)
        {
            // Set UI Stuff
            AssetListView.ItemsSource = null;
            AssetLoadedLabel.Content = "0 Assets Loaded";
            // Attempt to load game
            try
            {
                // Sent to Hydra Lib
                var status = Hydra.LoadAssets();
                switch (status)
                {
                    case HydraStatus.Success:
                        {
                            // Set it
                            AssetListView.ItemsSource = Hydra.LoadedAssets;
                            AssetLoadedLabel.Content = String.Format("{0} Assets Loaded", Hydra.LoadedAssets.Count);
                            // Log it
                            ActiveLogger.Log(String.Format("Loaded {0} Assets from {1} Successfully", Hydra.LoadedAssets.Count, Hydra.ActiveGameProcessName), Logger.MessageType.INFO);
                            View = CollectionViewSource.GetDefaultView(AssetListView.ItemsSource) as CollectionView;
                            View.Filter = ViewFilter;
                            break;
                        }
                    // Unsupported game
                    case HydraStatus.UnsupportedBinary:
                        {
                            ActiveLogger.Log("HydraX supports this game, but not this executable, if the game was recently updated, please wait for an update to HydraX.", Logger.MessageType.ERROR);
                            MessageBox.Show("HydraX supports this game, but not this executable, if the game was recently updated, please wait for an update to HydraX.", "HydraX", MessageBoxButton.OK, MessageBoxImage.Error);
                            Hydra.Clear();
                            break;
                        }
                    // Couldn't even find the madness
                    case HydraStatus.FailedToFindGame:
                        {
                            ActiveLogger.Log("HydraX failed to find a supported game, please ensure one of them is running.", Logger.MessageType.ERROR);
                            MessageBox.Show("HydraX failed to find a supported game, please ensure one of them is running.", "HydraX", MessageBoxButton.OK, MessageBoxImage.Error);
                            Hydra.Clear();
                            break;
                        }
                }
            }
            catch(Exception exception)
            {
                AssetListView.ItemsSource = null;
                AssetLoadedLabel.Content = "0 Assets Loaded";
                ActiveLogger.Log(String.Format("An unhandled exception occured while loading the game:\n\n{0}", exception), Logger.MessageType.ERROR);
                MessageBox.Show(String.Format("An unhandled exception occured while loading the game:\n\n{0}", exception), "HydraX", MessageBoxButton.OK, MessageBoxImage.Error);
                Hydra.Clear();
            }
            // Write the log
            ActiveLogger.Flush();
        }

        private void ExportSelectedClick(object sender, RoutedEventArgs e)
        {
            // Get listed assets
            var assets = AssetListView.SelectedItems.Cast<Asset>().ToList();
            // Check for hits
            if (assets.Count == 0)
            {
                MessageBox.Show("No assets selected to export.", "HydraX", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Send to exporter
            ExportAssets(assets);
        }

        private void ExportAllClick(object sender, RoutedEventArgs e)
        {
            // Get listed assets
            var assets = AssetListView.Items.Cast<Asset>().ToList();
            // Check for hits
            if (assets.Count == 0)
            {
                MessageBox.Show("No assets listed to export.", "HydraX", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Send to exporter
            ExportAssets(assets);
        }

        public void ExportAssets(List<Asset> assets)
        {
            // Attempt to validate the game
            var result = Hydra.ValidateGame();
            // Check result
            switch(result)
            {
                case HydraStatus.MemoryChanged:
                    {
                        ActiveLogger.Log("The game's Process ID has changed, click Load Game to refresh the asset list.", Logger.MessageType.ERROR);
                        MessageBox.Show("The game's Process ID has changed, click Load Game to refresh the asset list.", "HydraX", MessageBoxButton.OK, MessageBoxImage.Warning);
                        ActiveLogger.Flush();
                        return;
                    }
                case HydraStatus.GameClosed:
                    {
                        ActiveLogger.Log("HydraX failed to find the game's process, this is an indication that the game has been closed.", Logger.MessageType.ERROR);
                        MessageBox.Show("HydraX failed to find the game's process, this is an indication that the game has been closed.", "HydraX", MessageBoxButton.OK, MessageBoxImage.Warning);
                        ActiveLogger.Flush();
                        return;
                    }
            }
            // Create Progress Window
            var progressWindow = new ProgressWindow()
            {
                Owner = this,
                Title = "Working"
            };
            // Open Progress (invoke dispatcher to avoid halting us)
            Dispatcher.BeginInvoke(new Action(() => progressWindow.ShowDialog()));
            // Set Value
            progressWindow.SetProgressCount(assets.Count);
            // Create new thread
            new Thread(() =>
            {
                // Dim Dialog
                SetDimmer(Visibility.Visible);
                // Export queue
                Parallel.ForEach(assets, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, (asset, loop) =>
                {
                    // Attempt to export
                    try
                    {
                        // Pass to asset export function
                        if (!asset.ExportFunction(asset))
                        {
                            // Log it
                            ActiveLogger.Log(String.Format("The expected data @ 0x{0:X} has changed, {1} skipped.", asset.HeaderAddress, asset.Name), Logger.MessageType.ERROR);
                        }
                        else
                        {
                            // Log it
                            ActiveLogger.Log(String.Format("Exported {0}", asset.Name), Logger.MessageType.INFO);
                        }
                    }
                    catch(System.IO.IOException)
                    {
                    //    // IO Exceptions can be skipped, they're 99% of time due to duplicate assets and 
                    //    // 2 threads attempting to write them at the same time
                    }
                    catch(Exception e)
                    {
                        // Anything else we should log it
                        ActiveLogger.Log(String.Format("An unhandled exception while exporting {0}:\n\n{1}", asset.Name, e), Logger.MessageType.ERROR);
                    }
                    // Increment, and check for exit
                    if (progressWindow.IncrementProgress() || EndThread)
                        loop.Break();
                });
                // Flush Log
                ActiveLogger.Flush();
                // Flush GDTs
                Hydra.FlushGDTs();
                // Wait for export finished to close
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    // Close progress
                    progressWindow.Complete();
                    // Open Export Finished Dialog
                    new ExportFinishedDialog()
                    {
                        Owner = this
                    }.ShowDialog();
                    // Disable Dimmer
                    SetDimmer(Visibility.Hidden);
                }));
            }).Start();
        }

        private void AboutButtonClick(object sender, RoutedEventArgs e)
        {
            SetDimmer(Visibility.Visible);
            new AboutWindow()
            {
                Owner = this
            }.ShowDialog();
            SetDimmer(Visibility.Hidden);
        }

        private void SetDimmer(Visibility visibility)
        {
            Dispatcher.BeginInvoke(new Action(() => DimBox.Visibility = visibility));
        }

        private void ClearSearchClick(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
        }

        private void ClearAssetsClick(object sender, RoutedEventArgs e)
        {
            AssetListView.ItemsSource = null;
            AssetLoadedLabel.Content = "0 Assets Loaded";
            Hydra.Clear();
        }
    }
}
