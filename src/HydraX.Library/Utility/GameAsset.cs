using System.IO;
using System.Windows.Media;

namespace HydraX.Library
{
    /// <summary>
    /// A generic class to hold a CoD Asset
    /// </summary>
    public class GameAsset : Notifiable
    {
        /// <summary>
        /// Gets or Sets the Name of this Asset
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the Display Name for this Asset
        /// </summary>
        public string DisplayName { get { return Path.GetFileName(Name); } }

        /// <summary>
        /// Gets or Sets this Asset's Type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or Sets the Asset Status
        /// </summary>
        public string Status
        {
            get
            {
                return GetValue<string>("Status");
            }
            set
            {
                SetValue(value, "Status");
                // Update Foreground
                NotifyPropertyChanged("ForegroundColor");
            }
        }

        /// <summary>
        /// Creates a new Game Asset
        /// </summary>
        public GameAsset()
        {
            Status = "Loaded";
        }

        /// <summary>
        /// Gets the Foreground Color
        /// </summary>
        public Brush ForegroundColor
        {
            get
            {
                switch (Status)
                {
                    case "Exported":
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF63FF5E"));
                    case "Error":
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF6666"));
                    default:
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
                }
            }
        }

        /// <summary>
        /// Gets or Sets Information for this Asset
        /// </summary>
        public string Information { get; set; }

        /// <summary>
        /// Gets or Sets the Pointer to the Asset's name
        /// </summary>
        public long NameLocation { get; set; }

        /// <summary>
        /// Gets or Sets the Header Address for this Asset
        /// </summary>
        public long HeaderAddress { get; set; }

        /// <summary>
        /// Gets or Sets the size of this asset
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Gets or Sets the Pool for this Asset
        /// </summary>
        public IAssetPool AssetPool { get; set; }

        /// <summary>
        /// Returns the Name of this Asset
        /// </summary>
        public override string ToString()
        {
            return Name;
        }
    }
}
