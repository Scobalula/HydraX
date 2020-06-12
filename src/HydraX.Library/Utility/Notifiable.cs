using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HydraX.Library
{
    public class Notifiable : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the Backing Variables Dictionary
        /// </summary>
        private Dictionary<string, object> BackingVariables { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Property Changed Event Handler
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the value of the given property
        /// </summary>
        protected T GetValue<T>([CallerMemberName] string propertyName = "")
        {
            if (BackingVariables.TryGetValue(propertyName, out var value))
                return (T)value;
            else
                return default;
        }

        /// <summary>
        /// Gets the value of the given property
        /// </summary>
        protected T GetValue<T>(T defaultVal, [CallerMemberName] string propertyName = "")
        {
            if (BackingVariables.TryGetValue(propertyName, out var value))
                return (T)value;
            else
                return defaultVal;
        }

        /// <summary>
        /// Sets the value of the given property
        /// </summary>
        protected void SetValue<T>(T newValue, [CallerMemberName] string propertyName = "")
        {
            BackingVariables[propertyName] = newValue;
            NotifyPropertyChanged(propertyName);
        }

        /// <summary>
        /// Notifies that the property has changed
        /// </summary>
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
