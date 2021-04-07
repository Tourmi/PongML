using PongML.GameElements.AI;
using PongML.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
            public Semaphore Semaphore { get; } = new Semaphore(1, 1);
            public bool[] GamesPlayed { get; set; }
            public int NetScore { get; set; }
            public int NetMatches { get; set; }

        }

        private bool stop;
        private readonly AiPlayed[] ais;
        private int doneCount;
        private readonly object doneCountLock = new object();
        private readonly Models.GameConfiguration gc;
        private Brain[] lastWinners;

        public bool Ready { get; private set; }
        public event Action NewGeneration;
        public int Round { get; private set; }
        public int BestScore { get; private set; }

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
            Ready = false;

            while (!stop)
            {
                for (int i = 0; i < ais.Length; i++)
                {
                    int processNumber = i;
                    ThreadPool.QueueUserWorkItem(state => process(processNumber));
                }
                lock (doneCountLock)
                {
                    while (doneCount < ais.Length)
                    {
                        Monitor.Wait(doneCountLock);
                    }
                }

                newRound();
                //TODO: Save best AI to file
                if (gc.SaveBestAIAfterEveryRound)
                {
                    Directory.CreateDirectory("AISaves");
                    for (int i = 0; i < lastWinners.Length; i++)
                    {
                        File.WriteAllText("AISaves/" + Round + "_" + i + ".json", lastWinners[i].ToJson());
                    }
                }
            }
            //TODO : Save best AI to file
            if (gc.SaveBestAIAfterStoppingSim)
            {
                Directory.CreateDirectory("AISaves");
                for (int i = 0; i < lastWinners.Length; i++)
                {
                    File.WriteAllText("AISaves/_" + i + ".json", lastWinners[i].ToJson());
                }
            }
            Ready = true;
        }

        public void Stop()
        {
            stop = true;
        }

        private void process(int self)
        {
            Random random = new Random(self);
            //Randomizing the order of the opponents avoids too much interblocking
            int[] opponents = Enumerable.Range(1, ais.Length - 1).OrderBy(x => random.Next()).ToArray();

            foreach (int opponent in opponents)
            {
                int other = (self + opponent) % ais.Length;
                AiPlayed selfAi = ais[self];
                AiPlayed otherAi = ais[other];

                //To avoid deadlocks, we need to determine the order depending on the indexes
                if (self < other)
                {
                    selfAi.Semaphore.WaitOne();
                    otherAi.Semaphore.WaitOne();
                }
                else
                {
                    otherAi.Semaphore.WaitOne();
                    selfAi.Semaphore.WaitOne();
                }

                if (selfAi.GamesPlayed[other])
                {
                    otherAi.Semaphore.Release();
                    selfAi.Semaphore.Release();

                    continue;
                }
                fight(selfAi, otherAi);

                selfAi.GamesPlayed[other] = true;
                otherAi.GamesPlayed[self] = true;

                otherAi.Semaphore.Release();
                selfAi.Semaphore.Release();
            }

            lock (doneCountLock)
            {
                doneCount++;
                Monitor.Pulse(doneCountLock);
            }
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

            player1.NetScore += finalScore - player2.Brain.Score; // Penalty for letting the other score
            player2.NetScore -= finalScore + player1.Brain.Score; // Penalty for letting the other score
            player1.NetMatches += match;
            player2.NetMatches -= match;
            player1.Brain.Score = 0;
            player2.Brain.Score = 0;
        }

        private void newRound()
        {
            lastWinners = ais
                .OrderByDescending(ai => ai.NetScore)
                .ThenByDescending(ai => ai.NetMatches)
                .Take((int)gc.KeepBestAIs)
                .Select(ai => ai.Brain)
                .ToArray();

            BestScore = ais.OrderByDescending(ai => ai.NetScore).First().NetScore;

            foreach (Brain winner in lastWinners)
            {
                winner.ReverseHorizontal = false;
            }
            for (int i = 0; i < lastWinners.Length; i++)
            {
                ais[i].Brain = lastWinners[i];
            }
            int currIndex;
            for (currIndex = lastWinners.Length; currIndex < ais.Length && currIndex < gc.MaximumChildrenPerAi * gc.KeepBestAIs; currIndex++)
            {
                const int slowDownFactor = 1000;
                float currEvolutionFactor = ((gc.BaseEvolutionFactor / 100.0f) * slowDownFactor) / (slowDownFactor + Round);
                ais[currIndex].Brain = lastWinners[currIndex % lastWinners.Length].GenerateChild(currEvolutionFactor);
            }
            for (int i = currIndex; i < ais.Length; i++)
            {
                ais[i].Brain = new Brain(gc.NeuronCount, gc.LayerCount, gc.MemoryNeuronCount);
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

            doneCount = 0;
            Round++;
            NewGeneration?.Invoke();
        }
    }
}
