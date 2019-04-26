using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydraX.Library
{
    /// <summary>
    /// Status Enum for various operations
    /// </summary>
    public enum HydraStatus
    {
        Success,
        UnsupportedBinary,
        FailedToFindGame,
        Exception,
        MemoryChanged,
        GameClosed,
    }
}
