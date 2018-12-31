using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace HydraX
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            VersionLabel.Content = String.Format("Version: {0}", Assembly.GetExecutingAssembly().GetName().Version);
        }

        private void DonateButtonClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.buymeacoffee.com/Scobalula");
        }

        private void HomePageButtonClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://philmaher.me/HydraX/");
        }
    }
}