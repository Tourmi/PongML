using PongML.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PongML.GameElements.AI
{
    class Brain : IArtificialIntelligence
    {
        private bool reverseHorizontal;

        public Brain(bool reverseHorizontal)
        {
            this.reverseHorizontal = reverseHorizontal;
        }

        public float PaddlePosition { get; set; }
        public int Score { get; set; }

        public Input GetInput()
        {
            throw new NotImplementedException();
        }

        public void Update(Game game)
        {
            throw new NotImplementedException();
        }
    }
}
