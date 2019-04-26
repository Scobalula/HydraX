using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HydraX.Library;

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
        /// Active Instance
        /// </summary>
        private HydraInstance Instance = new HydraInstance();

        /// <summary>
        /// Gets or Sets the Active Settings
        /// </summary>
        private Settings ActiveSettings = new Settings("Settings.hcfg");

        /// <summary>
        /// Active Log
        /// </summary>
        private HydraLogger ActiveLogger = new HydraLogger("HydraX", "HydraLog.txt");

        /// <summary>
        /// List View (For Filtering)
        /// </summary>
        public CollectionView View { get; set; }

        /// <summary>
        /// Initializes the Main Window
        /// </summary>
        public MainWindow()
        {
            if(ActiveSettings["EulaWindowShown", "false"] == "false")
            {
                var eulaWindow = new LicenseAgreementWindow();
                eulaWindow.ShowDialog();

                if (eulaWindow.HasAccepted)
                {
                    ActiveSettings["EulaWindowShown"] = "true";
                }
                else
                {
                    Close();
                }

                ActiveSettings.Save("Settings.hcfg");
            }

            if(ActiveSettings["AutoUpdates", "false"] == "true")
            {
                new Thread(() =>
                {
                    if (HydraUpdater.CheckForUpdates(Assembly.GetExecutingAssembly().GetName().Version))
                    {
                        var result = MessageBox.Show("A new version of HydraX is available, do you want to download it now?", "Update Available", MessageBoxButton.YesNo, MessageBoxImage.Information);
                        if (result == MessageBoxResult.Yes)
                            System.Diagnostics.Process.Start("https://github.com/Scobalula/HydraX/releases");
                    }
                }).Start();
            }

            InitializeComponent();
        }

        /// <summary>
        /// Filters Sounds by text in Search Box
        /// </summary>
        private bool ViewFilter(object obj)
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
            Instance.Clear();

            // Attempt to load game
            try
            {
                // Sent to Hydra Lib
                var status = Instance.LoadGame();

                switch (status)
                {
                    case HydraStatus.Success:
                        {
                            // Set it
                            AssetListView.ItemsSource = Instance.Assets;
                            AssetLoadedLabel.Content = string.Format("{0} Assets Loaded", Instance.Assets.Count);
                            // Log it
                            ActiveLogger.Log(string.Format("Loaded {0} Assets from {1} Successfully", Instance.Assets.Count, Instance.Game.Name), HydraLogger.MessageType.INFO);
                            View = CollectionViewSource.GetDefaultView(AssetListView.ItemsSource) as CollectionView;
                            View.Filter = ViewFilter;
                            break;
                        }
                    // Unsupported game
                    case HydraStatus.UnsupportedBinary:
                        {
                            ActiveLogger.Log("HydraX supports this game, but not this executable, if the game was recently updated, please wait for an update to HydraX.", HydraLogger.MessageType.ERROR);
                            MessageBox.Show("HydraX supports this game, but not this executable, if the game was recently updated, please wait for an update to HydraX.", "HydraX", MessageBoxButton.OK, MessageBoxImage.Error);
                            Instance.Clear();
                            break;
                        }
                    // Couldn't even find the madness
                    case HydraStatus.FailedToFindGame:
                        {
                            ActiveLogger.Log("HydraX failed to find a supported game, please ensure one of them is running.", HydraLogger.MessageType.ERROR);
                            MessageBox.Show("HydraX failed to find a supported game, please ensure one of them is running.", "HydraX", MessageBoxButton.OK, MessageBoxImage.Error);
                            Instance.Clear();
                            break;
                        }
                }
            }
            catch (Exception exception)
            {
                AssetListView.ItemsSource = null;
                AssetLoadedLabel.Content = "0 Assets Loaded";
                ActiveLogger.Log(string.Format("An unhandled exception occured while loading the game:\n\n{0}", exception), HydraLogger.MessageType.ERROR);
                MessageBox.Show(string.Format("An unhandled exception occured while loading the game:\n\n{0}", exception), "HydraX", MessageBoxButton.OK, MessageBoxImage.Error);
                Instance.Clear();
            }
            // Write the log
            ActiveLogger.Flush();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ActiveSettings.Save("Settings.hcfg");
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
            Instance.Clear();
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

        private void ExportSelectedClick(object sender, RoutedEventArgs e)
        {

            var assets = AssetListView.SelectedItems.Cast<GameAsset>().ToList();

            if (assets.Count == 0)
            {
                MessageBox.Show("No assets selected to export.", "HydraX", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            ExportAssets(assets);
        }

        private void ExportAllClick(object sender, RoutedEventArgs e)
        {
            var assets = AssetListView.Items.Cast<GameAsset>().ToList();
            if (assets.Count == 0)
            {
                MessageBox.Show("No assets listed to export.", "HydraX", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            ExportAssets(assets);
        }

        public void ExportAssets(List<GameAsset> assets)
        {
            var result = Instance.ValidateGame();

            switch(result)
            {
                case HydraStatus.MemoryChanged:
                    {
                        ActiveLogger.Log("The game's Process ID has changed, click Load Game to refresh the asset list.", HydraLogger.MessageType.ERROR);
                        MessageBox.Show("The game's Process ID has changed, click Load Game to refresh the asset list.", "HydraX", MessageBoxButton.OK, MessageBoxImage.Warning);
                        ActiveLogger.Flush();
                        return;
                    }
                case HydraStatus.GameClosed:
                    {
                        ActiveLogger.Log("HydraX failed to find the game's process, this is an indication that the game has been closed.", HydraLogger.MessageType.ERROR);
                        MessageBox.Show("HydraX failed to find the game's process, this is an indication that the game has been closed.", "HydraX", MessageBoxButton.OK, MessageBoxImage.Warning);
                        ActiveLogger.Flush();
                        return;
                    }
            }

            var progressWindow = new ProgressWindow()
            {
                Owner = this,
                Title = "Working"
            };

            Dispatcher.BeginInvoke(new Action(() => progressWindow.ShowDialog()));
            progressWindow.SetProgressCount(assets.Count);

            new Thread(() =>
            {

                SetDimmer(Visibility.Visible);

                Parallel.ForEach(assets, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, (asset, loop) =>
                {
                    try
                    {
                        if (asset.AssetPool.Export(asset, Instance) == HydraStatus.MemoryChanged)
                        {
                            ActiveLogger.Log(String.Format("The expected data @ 0x{0:X} has changed, {1} skipped.", asset.HeaderAddress, asset.Name), HydraLogger.MessageType.ERROR);
                        }
                        else
                        {
                            ActiveLogger.Log(String.Format("Exported {0}", asset.Name), HydraLogger.MessageType.INFO);
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
                        ActiveLogger.Log(String.Format("An unhandled exception while exporting {0}:\n\n{1}", asset.Name, e), HydraLogger.MessageType.ERROR);
                    }

                    if (progressWindow.IncrementProgress() || EndThread)
                        loop.Break();
                });

                ActiveLogger.Flush();
                Instance.FlushGDTs();
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    GC.Collect();
                    progressWindow.Complete();
                    new ExportFinishedDialog()
                    {
                        Owner = this
                    }.ShowDialog();
                    SetDimmer(Visibility.Hidden);
                }));
            }).Start();
        }
    }
}
