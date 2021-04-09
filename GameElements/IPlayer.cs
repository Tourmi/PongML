using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PongML.GameElements
{
    /// <summary>
    /// Interface used by all players
    /// </summary>
    public interface IPlayer
    {
        /// <summary>
        /// The player's paddle position in the game
        /// </summary>
        float PaddlePosition { get; set; }
        /// <summary>
        /// The player's current score in the game
        /// </summary>
        int Score { get; set; }

        /// <summary>
        /// Returns this player's wanted <see cref="Input"/>
        /// </summary>
        /// <returns></returns>
        Input GetInput();
    }
}
