using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydraLib
{
    /// <summary>
    /// Class to hold Game Database Info
    /// </summary>
    public class GameDBInfo
    {
        /// <summary>
        /// Asset Database/Array Address
        /// </summary>
        public long AssetDBAddress { get; set; }

        /// <summary>
        /// String Array Address
        /// </summary>
        public long StringDBAddress { get; set; }

        /// <summary>
        /// Initializes Game DB Info with Asset and String Database Address
        /// </summary>
        /// <param name="assetDBAddress"></param>
        /// <param name="stringArrayAddress"></param>
        public GameDBInfo(long assetDBAddress, long stringDBAddress)
        {
            AssetDBAddress = assetDBAddress;
            StringDBAddress = stringDBAddress;
        }
    }
}
