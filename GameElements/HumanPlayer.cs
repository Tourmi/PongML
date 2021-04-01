using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PongML.GameElements
{
    class HumanPlayer : IPlayer
    {
        private readonly Key upKey;
        private readonly Key downKey;

        public HumanPlayer(Key upKey, Key downKey)
        {
            this.upKey = upKey;
            this.downKey = downKey;
        }

        public float PaddlePosition { get; set; }
        public int Score { get; set; }

        public Input GetInput()
        {
            Input input = new Input() { Direction = Direction.None, Intensity = 1 };
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (Keyboard.IsKeyDown(upKey) && !Keyboard.IsKeyDown(downKey))
                {
                    input.Direction = Direction.Up;
                }
                if (Keyboard.IsKeyDown(downKey) && !Keyboard.IsKeyDown(upKey))
                {
                    input.Direction = Direction.Down;
                }
            });

            return input;
        }
    }
}
