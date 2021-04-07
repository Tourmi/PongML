using Newtonsoft.Json;
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
        [JsonIgnore]
        public int PlayerNumber { get; set; }
        [JsonIgnore]
        public bool ReverseHorizontal { private get; set; }
        private readonly Random random;

        private readonly IPerceptron xBall;
        private readonly IPerceptron yBall;
        private readonly IPerceptron myPaddle;
        private readonly IPerceptron theirPaddle;
        [JsonProperty]
        private Memory[] memories;

        [JsonProperty]
        private INeuron[][] hiddenLayers;
        [JsonProperty]
        private INeuron[] outputLayer;
        [JsonIgnore]
        public float PaddlePosition { get; set; }
        [JsonIgnore]
        public int Score { get; set; }

        private int updateCount;

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


        public Input GetInput()
        {
            Input input = new Input() { Direction = Direction.None, Intensity = 0 };

            float upValue = outputLayer[0].GetValue();
            float downValue = outputLayer[1].GetValue();
            if (upValue >= 0 && upValue > downValue)
            {
                input.Intensity = upValue - downValue;
                input.Direction = Direction.Up;
            }
            else if (downValue >= 0 && downValue > upValue)
            {
                input.Intensity = downValue - upValue;
                input.Direction = Direction.Down;
            }

            //Update the memory values
            for (int i = 2; i < outputLayer.Length; i++)
            {
                outputLayer[i].GetValue();
            }

            return input;
        }

        public void Update(Game game)
        {
            int xModifier = ReverseHorizontal ? -1 : 1;
            xBall.SetValue((game.BallPos.X - game.ArenaHeight / 2) * xModifier);
            yBall.SetValue(game.BallPos.Y - game.ArenaHeight / 2);
            myPaddle.SetValue(game.Players[PlayerNumber].PaddlePosition - game.ArenaHeight / 2);
            theirPaddle.SetValue(game.Players[(PlayerNumber + 1) % 2].PaddlePosition - game.ArenaHeight / 2);

            foreach (var output in outputLayer)
            {
                output.Update(updateCount);
            }

            updateCount++;
        }

        private float[] generateWeights(int count)
        {
            float[] weights = new float[count];
            for (int i = 0; i < count; i++)
            {
                weights[i] = (float)random.NextDouble() * 4 - 2;
            }
            return weights;
        }

        private float[] generateWeights(float[] weights, float evolutionFactor)
        {
            float[] newWeights = new float[weights.Length];

            for (int i = 0; i < weights.Length; i++)
            {
                float roll = (float)((random.NextDouble() - 0.5) * (random.NextDouble() - 0.5)) * 4;
                newWeights[i] = roll * evolutionFactor * weights[i] /*+ 0.01f * evolutionFactor * ((float)random.NextDouble() - 0.5f)*/;
            }

            return newWeights;
        }

        public string ToJson()
        {
            var jsonSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };

            return JsonConvert.SerializeObject(this, jsonSettings);
        }

        public static Brain FromJson(string json)
        {
            Brain brain = JsonConvert.DeserializeObject<Brain>(json, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });

            INeuron[] lastLayer = new INeuron[4 + brain.memories.Length];

            lastLayer[0] = brain.xBall;
            lastLayer[1] = brain.yBall;
            lastLayer[2] = brain.myPaddle;
            lastLayer[3] = brain.theirPaddle;
            for (int i = 0; i < brain.memories.Length; i++)
            {
                brain.memories[i].MemoryOutput = brain.outputLayer[2 + i];
                lastLayer[i + 4] = brain.memories[i];
            }

            for (int i = 0; i < brain.hiddenLayers.Length; i++)
            {
                for (int j = 0; j < brain.hiddenLayers[i].Length; j++)
                {
                    brain.hiddenLayers[i][j].PreviousNeurons = lastLayer;
                }

                lastLayer = brain.hiddenLayers[i];
            }

            for (int i = 0; i < brain.outputLayer.Length; i++)
            {
                brain.outputLayer[i].PreviousNeurons = lastLayer;
            }

            return brain;
        }

        public Brain GenerateChild(float evolutionFactor)
        {
            Brain newBrain = new Brain();
            INeuron[] lastLayer = new INeuron[4 + memories.Length];
            lastLayer[0] = newBrain.xBall;
            lastLayer[1] = newBrain.yBall;
            lastLayer[2] = newBrain.myPaddle;
            lastLayer[3] = newBrain.theirPaddle;

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
