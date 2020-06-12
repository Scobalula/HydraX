using HydraX.Library;
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
        /// Main Window Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            DataContext = ViewModel;
            ViewModel.DimmerVisibility = Visibility.Hidden;
            Instance.Settings.Load("Settings.json");
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
            if (((FrameworkElement)e.OriginalSource).DataContext is Asset asset)
            {
                ViewModel.DimmerVisibility = Visibility.Visible;
                new ProgressWindow(ExportAssets, null, ProgressComplete, "Exporting Assets...", new List<Asset>() { asset }, 1, this).ShowDialog();
                ViewModel.DimmerVisibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Handles cleaning up on close
        /// </summary>
        private void MainWindowClosing(object sender, CancelEventArgs e)
        {
            LogStream?.Flush();
            LogStream?.Dispose();
            Instance.Settings.Save("Settings.json");
        }

        public void ExportAssets(object sender, DoWorkEventArgs e)
        {
            Instance.Settings.Load("Settings.json");
            Instance.RefreshGDTDB();
            Instance.LoadGDTs();

            var window = e.Argument as ProgressWindow;

            Parallel.ForEach(window.Data as List<Asset>, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, (asset, loop) =>
            {
                window.Worker.ReportProgress(0);

                try
                {
                    asset.Save(System.IO.Path.Combine(Instance.ExportFolder, asset.Type), Instance);
                    Log(string.Format("Exported {0}", asset.Name), "INFO");
                    asset.Status = "Exported";
                }
                catch (Exception exception)
                {
                    // Anything else we should log it
                    Log(string.Format("An unhandled exception while exporting {0}:\n\n{1}", asset.Name, exception), "ERROR");
                    asset.Status = "Error";
                }

                if (window.Worker.CancellationPending)
                    loop.Break();
            });

            Instance.FlushGDTs();
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
        private void OpenGameClick(object sender, RoutedEventArgs e)
        {
            Title = "HydraX | Loading Game......";
            ViewModel.AssetButtonsEnabled = false;

            Instance.Clear();
            ViewModel.Assets.ClearAllItems();

            // Init Worker
            var worker = new BackgroundWorker();

            worker.DoWork += LoadGameWorker;
            worker.RunWorkerCompleted += ProgressComplete;

            worker.RunWorkerAsync();
        }

        private void LoadGameWorker(object sender, DoWorkEventArgs e)
        {
            Instance.LoadGame();
            ViewModel.Assets.AddRange(Instance.Assets);
        }

        /// <summary>
        /// Handles on progress complete
        /// </summary>
        private void ProgressComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            GC.Collect();
            // ViewModel.DimmerVisibility = Visibility.Hidden;
            ViewModel.AssetButtonsEnabled = true;

            if (e.Error == null)
            {
                ViewModel.Assets.SendNotify();
                Title = string.Format("HydraX | Loaded {0} assets from {1}", Instance.Assets.Count, Instance.Game.Name);
            }
            else if(e.Error is GameNotFoundException)
            {
                Log("HydraX failed to find a supported game, please ensure one of them is running.", "ERROR");
                MessageBox.Show("HydraX failed to find a supported game, please ensure one of them is running.", "HydraX | Oops", MessageBoxButton.OK, MessageBoxImage.Error);
                Instance.Clear();
            }
            else if (e.Error is GameNotSupportedException gnsException)
            {
                Log(string.Format("HydraX supports {0}, but not this version of it, if the game was recently updated, please wait for an update to HydraX.", gnsException.GameName), "ERROR");
                MessageBox.Show(string.Format("HydraX supports {0}, but not this version of it, if the game was recently updated, please wait for an update to HydraX.", gnsException.GameName), "HydraX | Oops", MessageBoxButton.OK, MessageBoxImage.Error);
                Instance.Clear();
            }
            else if(e.Error is Exception exception)
            {
                Log(string.Format("An unhandled exception has occurred, take this to my creator:\n\n{0}", exception), "ERROR");
                MessageBox.Show(string.Format("An unhandled exception has occurred, take this to my creator:\n\n{0}", exception), "HydraX | Oops", MessageBoxButton.OK, MessageBoxImage.Error);
                Instance.Clear();
            }
        }

        /// <summary>
        /// Exports all loaded assets
        /// </summary>
        private void ExportAllClick(object sender, RoutedEventArgs e)
        {
            var assets = AssetList.Items.Cast<Asset>().ToList();

            if (assets.Count == 0)
            {
                ViewModel.DimmerVisibility = Visibility.Visible;
                MessageBox.Show("There are no assets listed to export.", "Greyhound | Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ViewModel.DimmerVisibility = Visibility.Hidden;
            }
            else
            {
                ViewModel.DimmerVisibility = Visibility.Visible;
                new ProgressWindow(ExportAssets, null, ProgressComplete, "Exporting Assets...", assets, assets.Count, this).ShowDialog();
                ViewModel.DimmerVisibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Exports selected assets
        /// </summary>
        private void ExportSelectedClick(object sender, RoutedEventArgs e)
        {
            var assets = AssetList.SelectedItems.Cast<Asset>().ToList();

            if (assets.Count == 0)
            {
                ViewModel.DimmerVisibility = Visibility.Visible;
                MessageBox.Show("There are no assets listed to export.", "Greyhound | Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ViewModel.DimmerVisibility = Visibility.Hidden;
            }
            else
            {
                ViewModel.DimmerVisibility = Visibility.Visible;
                new ProgressWindow(ExportAssets, null, ProgressComplete, "Exporting Assets...", assets, assets.Count, this).ShowDialog();
                ViewModel.DimmerVisibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Opens the Donation Page
        /// </summary>
        private void DonateClick(object sender, RoutedEventArgs e) => System.Diagnostics.Process.Start("https://www.paypal.me/Scobalula");

        /// <summary>
        /// Opens the Discord Invite
        /// </summary>
        private void DiscordClick(object sender, RoutedEventArgs e) => System.Diagnostics.Process.Start("https://discord.gg/RyqyThu");
    }
}
