// ------------------------------------------------------------------------
// HydraX - Black Ops III Asset Decompiler
// Copyright (C) 2019 Philip/Scobalula
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// ------------------------------------------------------------------------
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
        /// <summary>
        /// Creates the About Window
        /// </summary>
        public AboutWindow()
        {
            InitializeComponent();
            VersionLabel.Content = string.Format("Version: {0}", Assembly.GetExecutingAssembly().GetName().Version);
        }

        /// <summary>
        /// Opens the donate page
        /// </summary>
        private void DonateButtonClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.paypal.me/Scobalula");
        }

        /// <summary>
        /// Opens the home page
        /// </summary>
        private void HomePageButtonClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/Scobalula/HydraX");
        }
    }
}
