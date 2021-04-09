using PongML.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PongML.GameElements.AI
{
    /// <summary>
    /// This is a test AI that simply picks random moves
    /// </summary>
    class RandomAI : IArtificialIntelligence
    {
        private readonly Random random;
        /// <summary>
        /// Time to hold the input for
        /// </summary>
        private int holdUntil;
        /// <summary>
        /// The current frame of the game
        /// </summary>
        private int currentFrame;
        /// <summary>
        /// The current input being held
        /// </summary>
        private Input currentInput;

        public RandomAI()
        {
            random = new Random();
            holdUntil = 0;
        }

        public float PaddlePosition { get; set; }
        public int Score { get; set; }

        /// <summary>
        /// Returns a random input, or the currently held input
        /// </summary>
        /// <returns></returns>
        public Input GetInput()
        {
            if (currentFrame >= holdUntil)
            {
                holdUntil += random.Next(6, 30);
                Input newInput = new Input();
                switch(random.Next(0,5))
                {
                    case 0:
                        newInput.Direction = Direction.None;
                        break;
                    case 1:
                    case 2:
                        newInput.Direction = Direction.Up;
                        break;
                    case 3:
                    case 4:
                        newInput.Direction = Direction.Down;
                        break;
                }

                newInput.Intensity = ((float)random.NextDouble()) + 0.1f;
                currentInput = newInput;
            }

            return currentInput;
        }

        /// <summary>
        /// Updates the current frame
        /// </summary>
        /// <param name="game"></param>
        public void Update(Game game)
        {
            currentFrame = game.CurrentFrame;
        }
    }
}
