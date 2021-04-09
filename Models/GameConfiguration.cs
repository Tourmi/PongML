using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PongML.Models
{
    /// <summary>
    /// Configuration class used by the program
    /// </summary>
    public class GameConfiguration
    {

        /*
         * Neural Network
         */

        /// <summary>
        /// Amount of neurons in each hidden layers
        /// </summary>
        public int NeuronCount { get; set; }
        /// <summary>
        /// Amount of hidden layers in the neural network
        /// </summary>
        public int LayerCount { get; set; }
        /// <summary>
        /// Amount of memory neurons in the neural network
        /// </summary>
        public int MemoryNeuronCount { get; set; }
        /// <summary>
        /// The starting evolution factor of the training. Should be a number between 0 and 100
        /// </summary>
        public int BaseEvolutionFactor { get; set; }

        /*
         * Training
         */

        /// <summary>
        /// Number of AIs to train per round.
        /// Do note that the time to train is N^2, since every AI will fight every other AI.
        /// </summary>
        public uint NumberOfAIs { get; set; }
        /// <summary>
        /// The amount of AIs to keep every round.
        /// </summary>
        public uint KeepBestAIs { get; set; }
        /// <summary>
        /// The amount of children to generate per kept AI.
        /// </summary>
        public uint MaximumChildrenPerAi { get; set; }
        /// <summary>
        /// The game length in frames (1/60th of a second)
        /// </summary>
        public int GameLength { get; set; }
        /// <summary>
        /// Whether or not we should save the best AIs of a round to a file
        /// </summary>
        public bool SaveBestAIAfterEveryRound { get; set; }
        /// <summary>
        /// Whether or not we should save the best AIs of the whole training to a file
        /// </summary>
        public bool SaveBestAIAfterStoppingSim { get; set; }

        /*
         * Game Settings
         */

        /// <summary>
        /// The starting ball speed
        /// </summary>
        public float InitialBallSpeed { get; set; }
        /// <summary>
        /// Amount by which the ball's speed is increased every bounce
        /// </summary>
        public float BallSpeedIncrement { get; set; }
        /// <summary>
        /// Maximum speed of the players' paddle
        /// </summary>
        public float PaddleSpeed { get; set; }

        public GameConfiguration()
        {
            this.NeuronCount = 8;
            this.LayerCount = 1;
            this.MemoryNeuronCount = 0;
            this.BaseEvolutionFactor = 100;

            this.NumberOfAIs = 64;
            this.KeepBestAIs = 8;
            this.MaximumChildrenPerAi = 6;
            this.GameLength = 10800;
            this.SaveBestAIAfterEveryRound = false;
            this.SaveBestAIAfterStoppingSim = true;

            this.InitialBallSpeed = 5;
            this.BallSpeedIncrement = 1;
            this.PaddleSpeed = 10;
        }

        public GameConfiguration(int neuronCount, int layerCount, int memoryNeuronCount, int baseEvolutionFactor,
            uint numberOfAIs, uint keepBestAIs, int gameLength, bool saveBestAIAfterEveryRound, bool saveBestAIAfterStoppingSim,
            float initialBallSpeed, float ballSpeedIncrement, float paddleSpeed)
        {
            this.NeuronCount = neuronCount;
            this.LayerCount = layerCount;
            this.MemoryNeuronCount = memoryNeuronCount;
            this.BaseEvolutionFactor = baseEvolutionFactor;

            this.NumberOfAIs = numberOfAIs;
            this.KeepBestAIs = keepBestAIs;
            this.GameLength = gameLength;
            this.SaveBestAIAfterEveryRound = saveBestAIAfterEveryRound;
            this.SaveBestAIAfterStoppingSim = saveBestAIAfterStoppingSim;

            this.InitialBallSpeed = initialBallSpeed;
            this.BallSpeedIncrement = ballSpeedIncrement;
            this.PaddleSpeed = paddleSpeed;
        }
        public static GameConfiguration FromJson(string json) => JsonConvert.DeserializeObject<GameConfiguration>(json);

        public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
