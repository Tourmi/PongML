using PongML.GameElements.AI.Neural;
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
        public bool ReverseHorizontal { get; set; }
        private readonly Random random;

        private IPerceptron xBall;
        private IPerceptron yBall;
        private IPerceptron myPaddle;
        private IPerceptron theirPaddle;
        private Memory[] memories;

        private INeuron[][] hiddenLayers;
        private INeuron[] outputLayer;

        private Brain()
        {
            random = new Random();

            xBall = new Perceptron();
            yBall = new Perceptron();
            myPaddle = new Perceptron();
            theirPaddle = new Perceptron();
        }

        public Brain(int neuronCount, int hiddenLayerCount, int memoryNeuronCount) : this()
        {
            INeuron[] lastLayer = new INeuron[4 + memoryNeuronCount];

            lastLayer[0] = xBall;
            lastLayer[1] = yBall;
            lastLayer[2] = myPaddle;
            lastLayer[3] = theirPaddle;

            memories = new Memory[memoryNeuronCount];
            for (int i = 0; i < memoryNeuronCount; i++)
            {
                Memory memory = new Memory();
                memories[i] = memory;
                lastLayer[i + 4] = memory;
            }

            hiddenLayers = new INeuron[hiddenLayerCount][];

            for (int i = 0; i < hiddenLayerCount; i++)
            {
                hiddenLayers[i] = new INeuron[neuronCount];

                for (int j = 0; j < neuronCount; j++)
                {
                    hiddenLayers[i][j] = new Neuron(lastLayer, generateWeights(lastLayer.Length));
                }

                lastLayer = hiddenLayers[i];
            }

            outputLayer = new INeuron[2 + memoryNeuronCount];

            for (int i = 0; i < 2 + memoryNeuronCount; i++)
            {
                outputLayer[i] = new Neuron(lastLayer, generateWeights(lastLayer.Length));
            }

            for (int i = 0; i < memoryNeuronCount; i++)
            {
                memories[i].MemoryOutput = outputLayer[i + 2];
            }
        }

        public float PaddlePosition { get; set; }
        public int Score { get; set; }

        public Input GetInput()
        {
            Input input = new Input() { Direction = Direction.None, Intensity = 0 };

            float upValue = outputLayer[0].GetValue();
            float downValue = outputLayer[1].GetValue();
            if (upValue > 0 && downValue < 0)
            {
                input.Intensity = upValue;
                input.Direction = Direction.Up;
            }
            else if (downValue > 0 && upValue < 0)
            {
                input.Intensity = downValue;
                input.Direction = Direction.Down;
            }

            return input;
        }

        public void Update(Game game)
        {
            int xModifier = ReverseHorizontal ? -1 : 1;
            xBall.SetValue((game.BallPos.X / game.ArenaWidth - 0.5f) * xModifier);
            yBall.SetValue(game.BallPos.Y / game.ArenaHeight - 0.5f);
            //TODO: myPaddle
            //TODO: theirPaddle

            foreach (Memory memory in memories)
            {
                memory.SetValue(memory.MemoryOutput.GetCachedValue());
            }
        }

        private float[] generateWeights(int count)
        {
            float[] weights = new float[count];
            for (int i = 0; i < count; i++)
            {
                weights[i] = random.Next() * 2 - 1;
            }
            return weights;
        }

        private float[] generateWeights(float[] weights, float evolutionFactor)
        {
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] += ((float)random.NextDouble() * 2 - 1) * evolutionFactor * weights[i];
            }

            return weights;
        }

        public Brain GenerateChild(float evolutionFactor)
        {
            Brain newBrain = new Brain();
            INeuron[] lastLayer = new INeuron[4 + memories.Length];

            newBrain.memories = new Memory[memories.Length];
            for (int i = 0; i < memories.Length; i++)
            {
                Memory memory = new Memory();
                newBrain.memories[i] = memory;
                lastLayer[i + 4] = memory;
            }

            newBrain.hiddenLayers = new INeuron[hiddenLayers.Length][];

            for (int i = 0; i < hiddenLayers.Length; i++)
            {
                newBrain.hiddenLayers[i] = new INeuron[hiddenLayers[i].Length];

                for (int j = 0; j < hiddenLayers[i].Length; j++)
                {
                    newBrain.hiddenLayers[i][j] = new Neuron(lastLayer, generateWeights(hiddenLayers[i][j].Weights, evolutionFactor));
                }

                lastLayer = newBrain.hiddenLayers[i];
            }

            newBrain.outputLayer = new INeuron[2 + memories.Length];

            for (int i = 0; i < 2 + memories.Length; i++)
            {
                newBrain.outputLayer[i] = new Neuron(lastLayer, generateWeights(outputLayer[i].Weights, evolutionFactor));
            }

            for (int i = 0; i < memories.Length; i++)
            {
                newBrain.memories[i].MemoryOutput = outputLayer[i + 2];
            }

            return newBrain;
        }
    }
}
