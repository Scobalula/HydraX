using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydraLib.Assets
{
    /// <summary>
    /// Animation Selector Class
    /// </summary>
    public class AnimationSelectorObj
    {
        /// <summary>
        /// Animation Selector Column
        /// </summary>
        public class Column
        {
            /// <summary>
            /// Column Data Types (According to Linker's assembly, there is no 0x5/Other data types)
            /// </summary>
            public enum ColumnDataType : int
            {
                /// <summary>
                /// Enumerator (String Index)
                /// </summary>
                Enumerator = 0,

                /// <summary>
                /// Single Precision Float
                /// </summary>
                Float = 1,

                /// <summary>
                /// 32Bit Integer
                /// </summary>
                Int32 = 2,

                /// <summary>
                /// Minimum Single Precision Float
                /// </summary>
                FloatMin = 3,

                /// <summary>
                /// Maximum Single Precision Float
                /// </summary>
                FloatMax = 4,

                /// <summary>
                /// String Index
                /// </summary>
                String = 6,
            }

            /// <summary>
            /// Column Name
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Column Data Type
            /// </summary>
            public ColumnDataType DataType { get; set; }

            /// <summary>
            /// Creates an Animation Selector Header
            /// </summary>
            /// <param name="name">Header Name</param>
            /// <param name="dataType">Data Type</param>
            public Column(string name, ColumnDataType dataType)
            {
                Name = name;
                DataType = dataType;
            }
        }

        /// <summary>
        /// Animation Selector Row
        /// </summary>
        public class Row
        {
            /// <summary>
            /// Columns
            /// </summary>
            public string[] Columns { get; set; }

            /// <summary>
            /// Creates an Animation Selector Row
            /// </summary>
            /// <param name="columnCount">Column Count</param>
            public Row(int columnCount)
            {
                Columns = new string[columnCount];
            }
        }

        /// <summary>
        /// Animation Selector Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Animation Selector Columns (Each selector has its own headers)
        /// </summary>
        public Column[] Columns { get; set; }

        /// <summary>
        /// Animation Selector Row
        /// </summary>
        public Row[] Rows { get; set; }

        /// <summary>
        /// Creates an Animation Selector
        /// </summary>
        /// <param name="rowCount">Number of Rows</param>
        /// <param name="columnCount">Number of Columns/Headers</param>
        public AnimationSelectorObj(string name, int rowCount, int columnCount)
        {
            Name = name;
            Rows = new Row[rowCount];
            Columns = new Column[columnCount];
        }
    }
}
