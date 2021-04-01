﻿using System;
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
            this.NeuronCount = 1;
            this.LayerCount = 1;
            this.MemoryNeuronCount = 0;
            this.BaseEvolutionFactor = 0;

            this.NumberOfAIs = 2;
            this.KeepBestAIs = 1;
            this.GameLength = 3600;
            this.SaveBestAIAfterEveryRound = false;
            this.SaveBestAIAfterStoppingSim = true;

            this.InitialBallSpeed = 3;
            this.BallSpeedIncrement = 5;
            this.PaddleSpeed = 5;
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
    }
}