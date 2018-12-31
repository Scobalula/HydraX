using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydraLib
{
    /// <summary>
    /// Holds Asset Information
    /// </summary>
    public class Asset
    {
        public string Name { get; set; }

        public string DisplayName { get { return Path.GetFileName(Name); } }

        public string Type { get; set; }

        public string Information { get; set; }

        public long NameLocation { get; set; }

        public long HeaderAddress { get; set; }

        public int Size { get; set; }

        public Func<Asset, bool> ExportFunction { get; set; }

        public object Data { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
