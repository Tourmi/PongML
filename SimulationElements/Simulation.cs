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
    /// <summary>
    /// This class takes care of the training of neural networks.
    /// </summary>
    class Simulation
    {
        /// <summary>
        /// Class used for additional information for each AI
        /// </summary>
        private class AiPlayed
        {
            public AiPlayed(Brain brain, int gamesPlayed)
            {
                Brain = brain;
                GamesPlayed = new bool[gamesPlayed];
            }
            /// <summary>
            /// The neural network itself
            /// </summary>
            public Brain Brain { get; set; }
            public Semaphore Semaphore { get; } = new Semaphore(1, 1);
            /// <summary>
            /// Whether or not this AI has already played vs the ai at the i index
            /// </summary>
            public bool[] GamesPlayed { get; set; }
            /// <summary>
            /// Score of this AI over all of its games
            /// </summary>
            public int NetScore { get; set; }
            /// <summary>
            /// Amount of matches won - matches lost.
            /// </summary>
            public int NetMatches { get; set; }

        }

        /// <summary>
        /// Whether or not to stop the simulation
        /// </summary>
        private bool stop;
        /// <summary>
        /// The ais to train
        /// </summary>
        private readonly AiPlayed[] ais;
        /// <summary>
        /// Amount of AIs that finished fighting
        /// </summary>
        private int doneCount;
        /// <summary>
        /// Object used as a lock to synchronize accesses to the <see cref="doneCount"/> variable
        /// </summary>
        private readonly object doneCountLock = new object();
        /// <summary>
        /// The program's configuration
        /// </summary>
        private readonly Models.GameConfiguration gc;
        /// <summary>
        /// The latest kept AIs of the simulation
        /// </summary>
        private Brain[] lastWinners;

        /// <summary>
        /// Whether or not the simulation is ready to be exited
        /// </summary>
        public bool Ready { get; private set; }
        /// <summary>
        /// Event called at every new generation/round
        /// </summary>
        public event Action NewGeneration;
        /// <summary>
        /// The latest round ID
        /// </summary>
        public int Round { get; private set; }
        /// <summary>
        /// The current best score of the latest round
        /// </summary>
        public int CurrBestScore { get; private set; }
        /// <summary>
        /// The best score achieved since the beginning of the training
        /// </summary>
        public int BestScore { get; private set; }
        /// <summary>
        /// The round at which the best score was achieved
        /// </summary>
        public int BestScoreRound { get; private set; }
        /// <summary>
        /// The latest round's average score
        /// </summary>
        public int AverageScore { get; private set; }

        /// <summary>
        /// The training will be started from randomly generated AIs
        /// </summary>
        /// <param name="gc"></param>
        public Simulation(Models.GameConfiguration gc)
        {
            this.gc = gc;
            ais = new AiPlayed[gc.NumberOfAIs];
            for (int i = 0; i < ais.Length; i++)
            {
                ais[i] = new AiPlayed(new Brain(gc.NeuronCount, gc.LayerCount, gc.MemoryNeuronCount), ais.Length);
            }
        }

        /// <summary>
        /// The training will be based on the given <paramref name="baseBrain"/>, with the first AI being the one given, and the rest being its children
        /// </summary>
        /// <param name="baseBrain"></param>
        /// <param name="gc"></param>
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

        /// <summary>
        /// Starts the training. Shouldn't be called more than once
        /// </summary>
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

                if (gc.SaveBestAIAfterEveryRound)
                {
                    Directory.CreateDirectory("AISaves");
                    for (int i = 0; i < lastWinners.Length; i++)
                    {
                        File.WriteAllText("AISaves/" + Round + "_" + i + ".json", lastWinners[i].ToJson());
                    }
                }
            }

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

        /// <summary>
        /// Stops the training.
        /// </summary>
        public void Stop()
        {
            stop = true;
        }

        /// <summary>
        /// Processes a single AI's fights.
        /// </summary>
        /// <param name="self">The AI to process</param>
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
                    //We already played against this opponent, go to the next one
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

        /// <summary>
        /// Processes a fight between the two players
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
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

        /// <summary>
        /// Prepares the next round of generation
        /// <para/> - Picks the winners
        /// <para/> - Reproduces new children
        /// <para/> - Generates new AIs if needed
        /// <para/> - Resets the AIs
        /// </summary>
        private void newRound()
        {
            Round++;

            //Pick the ais to keep based on their score, and tie-breaked by their match results
            lastWinners = ais
                .OrderByDescending(ai => ai.NetScore)
                .ThenByDescending(ai => ai.NetMatches)
                .Take((int)gc.KeepBestAIs)
                .Select(ai => ai.Brain)
                .ToArray();

            //Update stats
            CurrBestScore = ais.OrderByDescending(ai => ai.NetScore).First().NetScore;
            AverageScore = (int)ais.Average(ai => ai.NetScore);
            if (CurrBestScore > BestScore || BestScoreRound == 0)
            {
                BestScoreRound = Round;
                BestScore = CurrBestScore;
            }

            //Set the first AIs as the kept winners
            for (int i = 0; i < lastWinners.Length && i < ais.Length; i++)
            {
                ais[i].Brain = lastWinners[i];
            }

            //Generates children for the kept AIs
            int currIndex;
            for (currIndex = lastWinners.Length; currIndex < ais.Length && currIndex < gc.MaximumChildrenPerAi * gc.KeepBestAIs; currIndex++)
            {
                const int slowDownFactor = 100;
                float currEvolutionFactor = ((gc.BaseEvolutionFactor / 100.0f) * slowDownFactor) / (slowDownFactor + Round);
                ais[currIndex].Brain = lastWinners[currIndex % lastWinners.Length].GenerateChild(currEvolutionFactor);
            }

            //If we don't have enough AIs, generate new random ones
            for (int i = currIndex; i < ais.Length; i++)
            {
                ais[i].Brain = new Brain(gc.NeuronCount, gc.LayerCount, gc.MemoryNeuronCount);
            }

            //Reset scores
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
            NewGeneration?.Invoke();
        }
    }
}
