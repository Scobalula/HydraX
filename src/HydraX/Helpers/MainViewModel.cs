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
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using HydraX.Library;

namespace HydraX
{
    /// <summary>
    /// Main View Model Class
    /// </summary>
    public class MainViewModel : Notifiable
    {
        /// <summary>
        /// Gets the Search Query
        /// </summary>
        public SearchQuery Query { get; } = new SearchQuery();

        /// <summary>
        /// Gets or Sets the filter string
        /// </summary>
        public string FilterString
        {
            get
            {
                return GetValue<string>("FilterString");
            }
            set
            {
                if (value != GetValue<string>("FilterString"))
                {
                    SetValue(value, "FilterString");
                    Query.Update(value);
                    AssetsView.Refresh();
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
        public UIItemList<Asset> Assets { get; } = new UIItemList<Asset>();

        /// <summary>
        /// Gets or Sets the Dimmer Visibility
        /// </summary>
        public Visibility DimmerVisibility
        {
            get
            {
                return GetValue<Visibility>("DimmerVisibility");
            }
            set
            {
                SetValue(value, "DimmerVisibility");
            }
        }

        /// <summary>
        /// Gets or Sets whether or not something is being loaded
        /// </summary>
        public bool AssetButtonsEnabled
        {
            get
            {
                return GetValue<bool>(true, "AssetButtonsEnabled");
            }
            set
            {
                SetValue(value, "AssetButtonsEnabled");
            }
        }

        public string Title { get; } = "HydraX";


        public SolidColorBrush High = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));

        /// <summary>
        /// Creates a new Viewmodel
        /// </summary>
        public MainViewModel()
        {
            AssetsView = CollectionViewSource.GetDefaultView(Assets);

            AssetsView.Filter = delegate (object obj)
            {
                if (obj is Asset asset)
                    return asset.CompareToSearch(Query);

                return true;
            };
        }
    }
}
