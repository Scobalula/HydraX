using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydraX.Library
{
    /// <summary>
    /// Setting data type
    /// </summary>
    public enum SettingDataType
    {
        Invalid,
        Color,
        UInt,
        UInt1,
        UInt2,
        UInt3,
        UInt4,
        Float,
        Float1,
        Float2,
        Float3,
        Float4,
        Boolean,
    }

    /// <summary>
    /// Post Process Flags
    /// </summary>
    public enum SettingPostProcess
    {
        None,
        LinearToRGB,
        Radians,
        SqRt,
    }

    /// <summary>
    /// A class t hold a technique setting
    /// </summary>
    public class MaterialTechniqueSetting
    {
        /// <summary>
        /// Gets or Sets the name in the HLSL file
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets the data type
        /// </summary>
        public SettingDataType DataType { get; set; }

        /// <summary>
        /// Gets or Sets the Post Processor
        /// </summary>
        public SettingPostProcess PostProcessor { get; set; }

        /// <summary>
        /// Gets or Sets the buffer offset
        /// </summary>
        public int BufferOffset { get; set; }

        /// <summary>
        /// Gets or Sets the buffer index
        /// </summary>
        public int BufferIndex { get; set; }

        /// <summary>
        /// Gets or Sets the slot names
        /// </summary>
        public string[] GDTSlotNames { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="slots"></param>
        public MaterialTechniqueSetting(string name, SettingDataType type, int slots)
        {
            Name = name;
            DataType = type;
            BufferOffset = -1;
            BufferIndex = -1;
            GDTSlotNames = new string[slots];
        }
    }

    /// <summary>
    /// A class to hold a techset
    /// </summary>
    public class MaterialTechniqueSet
    {
        /// <summary>
        /// Gets or Sets the Category
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or Sets the Display Name in APE
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or Sets the Technique Settings
        /// </summary>
        public Dictionary<string, MaterialTechniqueSetting> Settings { get; set; }

        /// <summary>
        /// Gets or Sets the Image Slots by hash with GDT name
        /// </summary>
        public Dictionary<uint, string> ImageSlots { get; set; }

        public MaterialTechniqueSet()
        {
            Settings = new Dictionary<string, MaterialTechniqueSetting>();
            ImageSlots = new Dictionary<uint, string>();
        }
    }
}
