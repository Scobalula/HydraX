using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using HydraX.Library;

namespace HydraX
{
    /// <summary>
    /// Progress Window View Model
    /// </summary>
    internal class ProgressWindowViewModel : Notifiable
    {
        /// <summary>
        /// Gets or Sets the number of items
        /// </summary>
        public double Count
        {
            get
            {
                return GetValue<double>("Count");
            }
            set
            {
                SetValue(value, "Count");
            }
        }

        /// <summary>
        /// Gets or Sets the current progress value
        /// </summary>
        public double Value
        {
            get
            {
                return GetValue<double>("Value");
            }
            set
            {
                SetValue(value, "Value");
            }
        }

        /// <summary>
        /// Gets or Sets if the progress bar is Indeterminate
        /// </summary>
        public bool Indeterminate
        {
            get
            {
                return GetValue<bool>("Indeterminate");
            }
            set
            {
                SetValue(value, "Indeterminate");
            }
        }

        /// <summary>
        /// Gets or Sets the Display Text
        /// </summary>
        public string Text
        {
            get
            {
                return GetValue<string>("Text");
            }
            set
            {
                SetValue(value, "Text");
            }
        }

        /// <summary>
        /// Gets or Sets the button visibility
        /// </summary>
        public Visibility OpenExportFolderVisibility
        {
            get
            {
                return GetValue<Visibility>("OpenExportFolderVisibility");
            }
            set
            {
                SetValue(value, "OpenExportFolderVisibility");
            }
        }

        /// <summary>
        /// Gets or Sets the button text
        /// </summary>
        public string RightButtonText
        {
            get
            {
                return GetValue<string>("RightButtonText");
            }
            set
            {
                SetValue(value, "RightButtonText");
            }
        }
    }
}
