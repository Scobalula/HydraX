using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HydraLib;

namespace HydraX
{
    /// <summary>
    /// Interaction logic for ExportFinishedDialog.xaml
    /// </summary>
    public partial class ExportFinishedDialog : Window
    {
        public ExportFinishedDialog()
        {
            InitializeComponent();
        }

        private void CloseWindowClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OpenExportFolderClick(object sender, RoutedEventArgs e)
        {
            Process.Start(Path.Combine("exported_files", Hydra.ActiveGameName));
            Close();
        }
    }
}
