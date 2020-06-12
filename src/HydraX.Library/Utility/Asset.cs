using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HydraX.Library
{
    /// <summary>
    /// A class to hold a generic asset
    /// </summary>
    public class Asset : Notifiable, IDisposable, IComparable
    {
        /// <summary>
        /// Gets or Sets the Asset Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the Asset Display Name
        /// </summary>
        public string DisplayName { get { return Path.GetFileNameWithoutExtension(Name); } }

        /// <summary>
        /// Gets or Sets the Asset Type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or Sets the Asset Zone Name
        /// </summary>
        public string Zone { get; set; }

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
        /// Gets or Sets the Asset Information
        /// </summary>
        public string Information { get; set; }

        public Brush ForegroundColor
        {
            get
            {
                switch (Status)
                {
                    case "Loaded":
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5EBEFF"));
                    case "Placeholder":
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFD83D"));
                    case "Exported":
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF63ff5e"));
                    case "Error":
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFff6666"));
                    default:
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
                }
            }
        }

        /// <summary>
        /// Gets or Sets the Game this Asset is from
        /// </summary>
        public string Game { get; set; }

        /// <summary>
        /// Gets or Sets the Asset Data
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Gets or Sets the method to load the Asset data
        /// </summary>
        public Action<Asset, HydraInstance> LoadMethod { get; set; }

        /// <summary>
        /// Saves the Asset
        /// </summary>
        public virtual void Save(string basePath, HydraInstance instance)
        {
            LoadMethod?.Invoke(this, instance);
        }

        /// <summary>
        /// Clears loaded asset data
        /// </summary>
        public virtual void ClearData()
        {
        }

        /// <summary>
        /// Disposes of the Asset
        /// </summary>
        public void Dispose()
        {
            ClearData();
        }

        /// <summary>
        /// Clones the Asset
        /// </summary>
        /// <returns>Cloned Asset</returns>
        public Asset Clone() => (Asset)MemberwiseClone();

        /// <summary>
        /// Che
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Asset asset)
            {
                return Name.Equals(asset.Name);
            }

            return base.Equals(obj);
        }

        /// <summary>
        /// Compares the Assets
        /// </summary>
        /// <param name="obj">Object to compare</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public virtual int CompareTo(object obj)
        {
            if (obj is Asset asset)
            {
                return Type.CompareTo(asset.Type);
            }

            return 1;
        }

        /// <summary>
        /// Compares the asset to the search query
        /// </summary>
        /// <param name="query">Query with data to check against the asset</param>
        /// <returns></returns>
        public virtual bool CompareToSearch(SearchQuery query)
        {
            var assetName = Name.ToLower();
            var assetType = Type.ToLower();
            var assetZone = Zone.ToLower();

            foreach (var item in query.Pass)
            {
                switch (item.Key)
                {
                    case "name":
                        foreach(var value in item.Value)
                            if (!assetName.Contains(value))
                                return false;
                        break;
                    case "zone":
                        foreach (var value in item.Value)
                            if (!assetZone.Contains(value))
                                return false;
                        break;
                    case "type":
                        foreach (var value in item.Value)
                            if (!assetType.Contains(value))
                                return false;
                        break;
                }
            }

            foreach (var item in query.Reject)
            {
                switch (item.Key)
                {
                    case "name":
                        foreach (var value in item.Value)
                            if (assetName.Contains(value))
                                return false;
                        break;
                    case "zone":
                        foreach (var value in item.Value)
                            if (assetZone.Contains(value))
                                return false;
                        break;
                    case "type":
                        foreach (var value in item.Value)
                            if (assetType.Contains(value))
                                return false;
                        break;
                }
            }

            foreach (var item in query.ExplicitPass)
            {
                switch (item.Key)
                {
                    case "name":
                        if (!item.Value.Contains(assetName))
                            return false;
                        break;
                    case "zone":
                        if (!item.Value.Contains(assetZone))
                            return false;
                        break;
                    case "type":
                        if (!item.Value.Contains(assetType))
                            return false;
                        break;
                }
            }

            foreach (var item in query.ExplicitReject)
            {
                switch (item.Key)
                {
                    case "name":
                        if (item.Value.Contains(assetName))
                            return false;
                        break;
                    case "zone":
                        if (item.Value.Contains(assetZone))
                            return false;
                        break;
                    case "type":
                        if (item.Value.Contains(assetType))
                            return false;
                        break;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns the Name of this Asset
        /// </summary>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}