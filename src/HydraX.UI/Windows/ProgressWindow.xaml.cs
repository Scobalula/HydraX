using System;
using System.Windows;
using System.Windows.Controls;

namespace HydraX
{
    /// <summary>
    /// Interaction logic for ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : Window
    {
        /// <summary>
        /// Whether we cancelled or not
        /// </summary>
        private bool HasCancelled = false;

        /// <summary>
        /// Initializes Progress Window
        /// </summary>
        public ProgressWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets Cancelled to true to update current task
        /// </summary>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !HasCancelled;
        }

        /// <summary>
        /// Closes Window on Cancel click
        /// </summary>
        private void CancelClick(object sender, RoutedEventArgs e)
        {
            HasCancelled = true;
        }

        /// <summary>
        /// Closes Progress Window on Complete
        /// </summary>
        public void Complete()
        {
            ProgressBar.Value = ProgressBar.Maximum;
            HasCancelled = true;
            Close();
        }

        public void SetProgressCount(double value)
        {
            // Invoke dispatcher to update UI
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ProgressBar.Maximum = value;
                ProgressBar.Value = 0;
            }));
        }

        public void SetProgressMessage(string value)
        {
            // Invoke dispatcher to update UI
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Message.Content = value;
            }));
        }

        /// <summary>
        /// Update Progress and checks for cancel
        /// </summary>
        public bool IncrementProgress()
        {
            // Invoke dispatcher to update UI
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ProgressBar.Value++;
            }));

            // Return whether we've cancelled or not
            return HasCancelled;
        }
    }
}
