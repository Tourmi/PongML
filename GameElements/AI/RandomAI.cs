using PongML.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PongML.GameElements.AI
{
    class RandomAI : IArtificialIntelligence
    {
        private readonly Random random;
        private int holdUntil;
        private int currentFrame;
        private Input currentInput;

        public RandomAI()
        {
            random = new Random();
            holdUntil = 0;
        }

        public float PaddlePosition { get; set; }
        public int Score { get; set; }

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

        public void Update(Game game)
        {
            currentFrame = game.CurrentFrame;
        }
    }
}
