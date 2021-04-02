using Newtonsoft.Json;

namespace PongML.GameElements.AI.Neural
{
    class Neuron : INeuron
    {
        [JsonIgnore]
        public INeuron[] PreviousNeurons { private get; set; }
        [JsonProperty]
        private readonly float[] previousWeights;
        private float cachedValue;

        public Neuron() { }

        public Neuron(INeuron[] previousNeurons, float[] previousWeights)
        {
            this.PreviousNeurons = previousNeurons;
            this.previousWeights = previousWeights;
        }

        [JsonIgnore]
        public float[] Weights => previousWeights;

        public float GetCachedValue() => cachedValue;

        public float GetValue()
        {
            float sum = 0;
            for (int i = 0; i < PreviousNeurons.Length; i++)
            {
                sum += PreviousNeurons[i].GetValue() * previousWeights[i];
            }

            cachedValue = sum;

            return sum;
        }
    }
}
