using System.Diagnostics;
using System.Windows;

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
            Process.Start("exported_files");
            Close();
        }
    }
}
