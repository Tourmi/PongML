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
    /// <summary>
    /// This class represents a single Neural network
    /// </summary>
    class Brain : IArtificialIntelligence
    {
        /// <summary>
        /// The player index of this AI
        /// </summary>
        [JsonIgnore]
        public int PlayerNumber { get; set; }
        /// <summary>
        /// Whether or not to mirror the horizontal axis when updating perceptrons
        /// </summary>
        [JsonIgnore]
        public bool ReverseHorizontal { private get; set; }
        private readonly Random random;

        /// <summary>
        /// X position of the ball input
        /// </summary>
        private readonly IPerceptron xBall;
        /// <summary>
        /// Y position of the ball input
        /// </summary>
        private readonly IPerceptron yBall;
        /// <summary>
        /// This network's paddle input
        /// </summary>
        private readonly IPerceptron myPaddle;
        /// <summary>
        /// The enemy's paddle input
        /// </summary>
        private readonly IPerceptron theirPaddle;
        /// <summary>
        /// The memory neurons that's used as extra inputs
        /// </summary>
        [JsonProperty]
        private Memory[] memories;

        /// <summary>
        /// The hidden layers of the neural network
        /// </summary>
        [JsonProperty]
        private INeuron[][] hiddenLayers;
        /// <summary>
        /// The output layer of the neural network.
        /// Has two outputs, plus one more per memory neuron
        /// </summary>
        [JsonProperty]
        private INeuron[] outputLayer;
        /// <summary>
        /// Position of the paddle
        /// </summary>
        [JsonIgnore]
        public float PaddlePosition { get; set; }
        /// <summary>
        /// The current score of the AI
        /// </summary>
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

        /// <summary>
        /// Returns the current input the AI is picking
        /// </summary>
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

            return input;
        }

        /// <summary>
        /// Updates the value of the perceptrons, then updates the neural network's neurons recursively
        /// </summary>
        /// <param name="game">The current game of the AI</param>
        public void Update(Game game)
        {
            int xModifier = ReverseHorizontal ? -1 : 1;
            xBall.SetValue((game.BallPos.X - game.ArenaWidth / 2) * xModifier);
            yBall.SetValue(game.BallPos.Y - game.ArenaHeight / 2);
            myPaddle.SetValue(game.Players[PlayerNumber].PaddlePosition - game.ArenaHeight / 2);
            theirPaddle.SetValue(game.Players[(PlayerNumber + 1) % 2].PaddlePosition - game.ArenaHeight / 2);

            foreach (var output in outputLayer)
            {
                output.Update(updateCount);
            }

            updateCount++;
        }

        /// <summary>
        /// Generates an array of weights of size <paramref name="count"/>
        /// </summary>
        /// <param name="count">The size of the weights array to return</param>
        /// <returns>The randomly generated weights</returns>
        private float[] generateWeights(int count)
        {
            float[] weights = new float[count];
            for (int i = 0; i < count; i++)
            {
                weights[i] = (float)random.NextDouble() * 4 - 2;
            }
            return weights;
        }

        /// <summary>
        /// Generates a new array of weights based on the given <paramref name="weights"/>.
        /// The <paramref name="evolutionFactor"/> should be a number superior to zero, which determines how much the weights may change
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="evolutionFactor"></param>
        /// <returns></returns>
        private float[] generateWeights(float[] weights, float evolutionFactor)
        {
            float[] newWeights = new float[weights.Length];

            for (int i = 0; i < weights.Length; i++)
            {
                //The roll is zero-centered, and has a value between -1 and 1.
                float roll = (float)((random.NextDouble() - 0.5) * (random.NextDouble() - 0.5)) * 4;
                newWeights[i] += roll * evolutionFactor * weights[i];
            }

            return newWeights;
        }

        /// <summary>
        /// Saves the weights, and memory neurons to a Json string
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            var jsonSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };

            return JsonConvert.SerializeObject(this, jsonSettings);
        }

        /// <summary>
        /// Returns a new <see cref="Brain"/> object from the given Json string
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Generates a "child" of this Neural network, by copying this Brain's weights, and slightly modifying them in the child.
        /// </summary>
        /// <param name="evolutionFactor"></param>
        /// <returns></returns>
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
