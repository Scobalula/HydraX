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
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Media;
using HydraX.Library;

namespace HydraX
{
    /// <summary>
    /// Main View Model Class
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        #region BackingVariables
        /// <summary>
        /// Gets or Sets the Filter String
        /// </summary>
        private string BackingFilterString { get; set; }

        /// <summary>
        /// Gets or Sets the Filter Strings
        /// </summary>
        private string[] FilterStrings { get; set; }
        #endregion

        /// <summary>
        /// Gets or Sets the filter string
        /// </summary>
        public string FilterString
        {
            get
            {
                return BackingFilterString;
            }
            set
            {
                if (value != BackingFilterString)
                {
                    BackingFilterString = value;
                    FilterStrings = string.IsNullOrWhiteSpace(BackingFilterString) ? null : BackingFilterString.Split(' ');
                    AssetsView.Refresh();
                    OnPropertyChanged("FilterString");
                }
            }
        }

        /// <summary>
        /// Gets or Sets the Collection View for the Assets
        /// </summary>
        private ICollectionView AssetsView { get; set; }

        /// <summary>
        /// Gets the observable collection of assets
        /// </summary>
        public AssetList Assets { get; } = new AssetList();

        /// <summary>
        /// Property Changed Event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public SolidColorBrush High = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));

        /// <summary>
        /// Creates a new Viewmodel
        /// </summary>
        public MainViewModel()
        {
            AssetsView = CollectionViewSource.GetDefaultView(Assets);
            AssetsView.Filter = delegate (object obj)
            {
                // TODO: Optimize this for Greyhound, parse search string into an object we can utilize
                if (FilterStrings != null && FilterStrings.Length > 0 && obj is GameAsset asset)
                {
                    var assetName = asset.Name.ToLower();
                    var assetType = asset.Type.ToLower();
                    var result    = true;

                    foreach (var filterString in FilterStrings)
                    {
                        if (filterString.StartsWith("type:"))
                        {
                            result = assetType.Contains(filterString.Replace("type:", "").ToLower());
                        }

                        if(result)
                        {
                            if (!string.IsNullOrWhiteSpace(filterString) && !filterString.StartsWith("type:"))
                                result = assetName.Contains(filterString.ToLower());
                        }

                        if (result)
                            break;
                    }

                    return result;
                }

                return true;
            };
        }

        /// <summary>
        /// Updates the Property on Change
        /// </summary>
        /// <param name="name">Property Name</param>
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
