using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PongML.Models
{
    public class GameConfiguration
    {

        /*
         * Neural Network
         */
        public int NeuronCount { get; set; }
        public int LayerCount { get; set; }
        public int MemoryNeuronCount { get; set; }
        public int BaseEvolutionFactor { get; set; }

        /*
         * Training
         */
        public uint NumberOfAIs { get; set; }
        public uint KeepBestAIs { get; set; }
        public uint MaximumChildrenPerAi { get; set; }
        //Game length in frames
        public int GameLength { get; set; }
        public bool SaveBestAIAfterEveryRound { get; set; }
        public bool SaveBestAIAfterStoppingSim { get; set; }

        /*
         * Game Settings
         */
        public float InitialBallSpeed { get; set; }
        public float BallSpeedIncrement { get; set; }
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
