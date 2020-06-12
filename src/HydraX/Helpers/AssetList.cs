// ------------------------------------------------------------------------
// HydraX - Black Ops III Asset Decompiler
// Copyright (C) 2020 Philip/Scobalula
// ------------------------------------------------------------------------
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// ------------------------------------------------------------------------
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// ------------------------------------------------------------------------
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// ------------------------------------------------------------------------
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;

namespace HydraX
{
    /// <summary>
    /// A class to hold a UI Item List
    /// </summary>
    public class UIItemList<T> : ObservableCollection<T>
    {
        /// <summary>
        /// Gets or Sets if we should notify
        /// </summary>
        public bool Notify { get; set; }

        /// <summary>
        /// Initializes a new instance of the UI Item List
        /// </summary>
        public UIItemList()
        {
            Notify = true;
        }

        /// <summary>
        /// Adds items to the list
        /// </summary>
        /// <param name="items">List of Items</param>
        public void AddRange(IEnumerable<T> items)
        {
            lock (((List<T>)Items))
            {
                ((List<T>)Items).AddRange(items);
            }
        }

        public void Sort()
        {
            ((List<T>)Items).Sort();
        }

        /// <summary>
        /// Clears all loaded items
        /// </summary>
        public void ClearAllItems()
        {
            Clear();
        }

        public void SendNotify()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Overrides On Collection Changed to disable notifying each time the list is updated
        /// </summary>
        /// <param name="e">Args</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (Notify)
                base.OnCollectionChanged(e);
        }
    }
}