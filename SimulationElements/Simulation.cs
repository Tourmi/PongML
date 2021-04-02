using PongML.GameElements.AI;
using PongML.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PongML.SimulationElements
{
    class Simulation
    {
        private class AiPlayed
        {
            public AiPlayed(Brain brain, int gamesPlayed)
            {
                Brain = brain;
                GamesPlayed = new bool[gamesPlayed];
            }

            public Brain Brain { get; set; }
            public bool[] GamesPlayed { get; set; }
            public int NetScore { get; set; }
            public int NetMatches { get; set; }
        }

        private bool stop;
        private readonly AiPlayed[] ais;
        private readonly Models.GameConfiguration gc;

        public Simulation(Models.GameConfiguration gc)
        {
            this.gc = gc;
            ais = new AiPlayed[gc.NumberOfAIs];
            for (int i = 0; i < ais.Length; i++)
            {
                ais[i] = new AiPlayed(new Brain(gc.NeuronCount, gc.LayerCount, gc.MemoryNeuronCount), ais.Length);
            }
        }

        public Simulation(Brain baseBrain, Models.GameConfiguration gc)
        {
            this.gc = gc;
            ais = new AiPlayed[gc.NumberOfAIs];
            ais[0] = new AiPlayed(baseBrain, ais.Length);
            for (int i = 1; i < ais.Length; i++)
            {
                ais[i] = new AiPlayed(baseBrain.GenerateChild(gc.BaseEvolutionFactor), ais.Length);
            }
        }


        public void Start()
        {
            stop = false;

            while (!stop)
            {
                //TODO : Select AI

                newRound();
            }
        }

        public void Stop()
        {
            stop = true;
        }

        private void fight(AiPlayed player1, AiPlayed player2)
        {
            Game game = new Game(gc, player1.Brain, player2.Brain);
            for (int i = 0; i < gc.GameLength; i++)
            {
                game.Update();
            }

            int finalScore = player1.Brain.Score - player2.Brain.Score;
            int match = finalScore == 0 ? 0 : finalScore > 0 ? 1 : -1;

            player1.NetScore += finalScore;
            player2.NetScore -= finalScore;
            player1.NetMatches += match;
            player2.NetMatches -= match;
        }

        private void newRound()
        {
            Brain[] winners = ais
                .OrderByDescending(ai => ai.NetMatches)
                .ThenByDescending(ai => ai.NetScore)
                .Take((int)gc.KeepBestAIs)
                .Select(ai => ai.Brain)
                .ToArray();

            foreach (Brain winner in winners)
            {
                winner.Score = 0;
                winner.ReverseHorizontal = false;
            }
            for (int i = 0; i < winners.Length; i++)
            {
                ais[i].Brain = winners[i];
            }
            for (int i = winners.Length; i < ais.Length; i++)
            {
                ais[i].Brain = winners[i % winners.Length].GenerateChild(gc.BaseEvolutionFactor / 100.0f);
            }

            foreach (AiPlayed ai in ais)
            {
                ai.NetScore = 0;
                ai.NetMatches = 0;
                for (int i = 0; i < ai.GamesPlayed.Length; i++)
                {
                    ai.GamesPlayed[i] = false;
                }
            }
        }
    }
}
