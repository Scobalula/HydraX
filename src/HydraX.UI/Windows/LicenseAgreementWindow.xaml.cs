using System.IO;
using System.Text;
using System.Windows;

namespace HydraX
{
    /// <summary>
    /// Interaction logic for LicenseAgreementWindow.xaml
    /// </summary>
    public partial class LicenseAgreementWindow : Window
    {
        /// <summary>
        /// Whether or not the user has accepted the EULA
        /// </summary>
        public bool HasAccepted = false;

        public LicenseAgreementWindow()
        {
            InitializeComponent();
            
            using (var stream = new MemoryStream(Encoding.ASCII.GetBytes(Properties.Resources.LICENSE)))
            {
                LicenseBox.Selection.Load(stream, DataFormats.Rtf);
            }
        }

        private void AcceptClick(object sender, RoutedEventArgs e)
        {
            HasAccepted = true;
            Close();
        }

        private void DoNotAcceptClick(object sender, RoutedEventArgs e)
        {
            HasAccepted = false;
            Close();
        }
    }
}
