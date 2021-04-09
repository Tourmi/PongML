using PongML.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PongML.GameElements
{
    /// <summary>
    /// Interface used by all AIs
    /// </summary>
    interface IArtificialIntelligence : IPlayer
    {
        /// <summary>
        /// Updates the AI's current state
        /// </summary>
        /// <param name="game">The ongoing game</param>
        void Update(Game game);
    }
}
