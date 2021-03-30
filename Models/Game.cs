using PongML.GameElements;
using PongML.GameElements.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PongML.Models
{
    class Game
    {
        public int CurrentFrame { get; set; }
        private List<IArtificialIntelligence> ais;
        private IPlayer[] players;

        public Game()
        {
            ais = new List<IArtificialIntelligence>();
            players = new IPlayer[2];

            players[0] = new HumanPlayer(Key.W, Key.S);
            IArtificialIntelligence ai = new RandomAI();
            players[1] = ai;
            ais.Add(ai);
        }

        public void Update()
        {
            foreach (var ai in ais)
            {
                ai.Update(this);
            }

            //process inputs

            //update ball

        }
    }
}
