using HydraX.Library;
using PhilLibX.Compression;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
        private bool EndThread = false;

        /// <summary>
        /// Active Instance
        /// </summary>
        private HydraInstance Instance = new HydraInstance();

        /// <summary>
        /// Gets or Sets the Log
        /// </summary>
        private StreamWriter LogStream { get; set; }

        /// <summary>
        /// Gets the ViewModel
        /// </summary>
        public MainViewModel ViewModel { get; } = new MainViewModel();

        /// <summary>
        /// Sets the dimmer
        /// </summary>
        private void SetDimmer(Visibility visibility) => Dispatcher.BeginInvoke(new Action(() => Dimmer.Visibility = visibility));

        /// <summary>
        /// Main Window Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            DataContext = ViewModel;
            Instance.Settings.Load("Settings.hcfg");
            Log("HydraX - Log Session Begin", "BEGIN");

            try
            {
                LogStream = new StreamWriter("Log.txt", true);
            }
            catch
            {
                LogStream = null;
            }

            if (Instance.Settings["AutoUpdates", "Yes"] == "Yes")
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
        }

        /// <summary>
        /// Exports the asset on double click
        /// </summary>
        private void AssetListMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (((FrameworkElement)e.OriginalSource).DataContext is GameAsset asset)
            {
                ExportAssets(new List<GameAsset>()
                {
                    asset
                });
            }
        }

        /// <summary>
        /// Handles cleaning up on close
        /// </summary>
        private void MainWindowClosing(object sender, CancelEventArgs e)
        {
            LogStream?.Flush();
            LogStream?.Dispose();
            Instance.Settings.Save("Settings.hcfg");
        }

        public void ExportAssets(List<GameAsset> assets)
        {
            var result = Instance.ValidateGame();

            switch (result)
            {
                case HydraStatus.MemoryChanged:
                    {
                        Log("The game's Process ID has changed, click Load Game to refresh the asset list.", "ERROR");
                        MessageBox.Show("The game's Process ID has changed, click Load Game to refresh the asset list.", "HydraX | Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                case HydraStatus.GameClosed:
                    {
                        Log("HydraX failed to find the game's process, this is an indication that the game has been closed.", "ERROR");
                        MessageBox.Show("HydraX failed to find the game's process, this is an indication that the game has been closed.", "HydraX | Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
            }

            var progressWindow = new ProgressWindow()
            {
                Owner = this,
                Title = "HydraX | Working"
            };

            Dispatcher.BeginInvoke(new Action(() => progressWindow.ShowDialog()));
            progressWindow.SetProgressCount(assets.Count);

            new Thread(() =>
            {

                SetDimmer(Visibility.Visible);

                Instance.LoadGDTs();
                Instance.RefreshGDTDB();

                Parallel.ForEach(assets, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, (asset, loop) =>
                {
                    try
                    {
                        if (asset.AssetPool.Export(asset, Instance) == HydraStatus.MemoryChanged)
                        {
                            Log(string.Format("The expected data @ 0x{0:X} has changed, {1} skipped.", asset.HeaderAddress, asset.Name), "ERROR");
                            asset.Status = "Error";
                        }
                        else
                        {
                            Log(string.Format("Exported {0}", asset.Name), "INFO");
                            asset.Status = "Exported";
                        }
                    }
                    catch (Exception e)
                    {
                        // Anything else we should log it
                        Log(string.Format("An unhandled exception while exporting {0}:\n\n{1}", asset.Name, e), "ERROR");
                        asset.Status = "Error";
                    }

                    if (progressWindow.IncrementProgress() || EndThread)
                        loop.Break();
                });

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

        /// <summary>
        /// Writes to the log in debug
        /// </summary>
        private void Log(string value, string messageType)
        {
            if (LogStream != null)
            {
                lock (LogStream)
                {
                    LogStream.WriteLine("{0} [ {1} ] {2}", DateTime.Now.ToString("dd-MM-yyyy - HH:mm:ss"), messageType.PadRight(12), value);
                    LogStream.Flush();
                }
            }
        }

        /// <summary>
        /// Opens file dialog to open a package
        /// </summary>
        private void OpenFileClick(object sender, RoutedEventArgs e)
        {
            Instance.Clear();
            ViewModel.Assets.ClearAssets();

            // Attempt to load game
            try
            {
                // Sent to Hydra Lib
                var status = Instance.LoadGame();

                switch (status)
                {
                    case HydraStatus.Success:
                        {
                            ViewModel.Assets.AddAssets(Instance.Assets);
                            Title = string.Format("HydraX | Loaded {0} assets from {1}", Instance.Assets.Count, Instance.Game.Name);
                            Log(string.Format("Loaded {0} Assets from {1} Successfully", Instance.Assets.Count, Instance.Game.Name), "INFO");
                            break;
                        }
                    case HydraStatus.UnsupportedBinary:
                        {
                            Log("HydraX supports this game, but not this executable, if the game was recently updated, please wait for an update to HydraX.", "ERROR");
                            MessageBox.Show("HydraX supports this game, but not this executable, if the game was recently updated, please wait for an update to HydraX.", "HydraX | Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            Instance.Clear();
                            break;
                        }
                    case HydraStatus.FailedToFindGame:
                        {
                            Log("HydraX failed to find a supported game, please ensure one of them is running.", "ERROR");
                            MessageBox.Show("HydraX failed to find a supported game, please ensure one of them is running.", "HydraX | Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            Instance.Clear();
                            break;
                        }
                }
            }
            catch (Exception exception)
            {
                Log(string.Format("An unhandled exception occured while loading the game:\n\n{0}", exception), "ERROR");
                MessageBox.Show(string.Format("An unhandled exception occured while loading the game:\n\n{0}", exception), "HydraX | Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Instance.Clear();
            }
            //ViewModel.Assets.ClearAssets();
            //ActivePackage?.Dispose();
            //ActivePackage = null;
            //GC.Collect();

            //var dialog = new OpenFileDialog()
            //{
            //    Title = "HydraX | Open File",
            //    Filter = "Package files (*.pak)|*.pak|All files (*.*)|*.*"
            //};

            //if (dialog.ShowDialog() == true)
            //{
            //    if (Path.GetExtension(dialog.FileName).ToLower() == ".pak")
            //    {
            //        LoadPackage(dialog.FileName);
            //    }
            //}
        }

        /// <summary>
        /// Exports all loaded assets
        /// </summary>
        private void ExportAllClick(object sender, RoutedEventArgs e)
        {
            var assets = AssetList.Items.Cast<GameAsset>().ToList();

            if (assets.Count == 0)
            {
                SetDimmer(Visibility.Visible);
                MessageBox.Show("There are no assets listed to export. Load some assets first.", "HydraX | Error", MessageBoxButton.OK, MessageBoxImage.Error);
                SetDimmer(Visibility.Hidden);
                return;
            }

            ExportAssets(assets);
        }

        /// <summary>
        /// Exports selected assets
        /// </summary>
        private void ExportSelectedClick(object sender, RoutedEventArgs e)
        {
            var assets = AssetList.SelectedItems.Cast<GameAsset>().ToList();

            if (assets.Count == 0)
            {
                SetDimmer(Visibility.Visible);
                MessageBox.Show("There are no assets selected to export. Select assets in the list first.", "HydraX | Error", MessageBoxButton.OK, MessageBoxImage.Error);
                SetDimmer(Visibility.Hidden);
                return;
            }

            ExportAssets(assets);
        }

        /// <summary>
        /// Opens settings window
        /// </summary>
        private void SettingsClick(object sender, RoutedEventArgs e)
        {
            var settings = new SettingsWindow()
            {
                Owner = this
            };

            Dimmer.Visibility = Visibility.Visible;

            settings.SetUIFromSettings(Instance.Settings);
            settings.ShowDialog();
            settings.SetSettingsFromUI(Instance.Settings);

            Dimmer.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Opens about window
        /// </summary>
        private void AboutClick(object sender, RoutedEventArgs e)
        {
            SetDimmer(Visibility.Visible);
            new AboutWindow()
            {
                Owner = this
            }.ShowDialog();
            SetDimmer(Visibility.Hidden);
        }

        private void DonateClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.paypal.me/Scobalula");
        }

        private void DiscordClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://discord.gg/fGVpV39");
        }
    }
}
