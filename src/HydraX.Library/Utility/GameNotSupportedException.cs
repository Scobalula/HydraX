using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydraX.Library
{
    public class GameNotSupportedException : Exception
    {
        /// <summary>
        /// Gets the Name of the Game
        /// </summary>
        public string GameName { get; private set; }
        public GameNotSupportedException()
        {
        }

        public GameNotSupportedException(string game) : base()
        {
            GameName = game;
        }

        public GameNotSupportedException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
