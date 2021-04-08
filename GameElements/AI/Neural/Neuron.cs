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
        private int currUpdate = 0;

        public Neuron() { }

        public Neuron(INeuron[] previousNeurons, float[] previousWeights)
        {
            this.PreviousNeurons = previousNeurons;
            this.previousWeights = previousWeights;
        }

        [JsonIgnore]
        public float[] Weights => previousWeights;

        public float GetValue() => cachedValue;
        

        public void Update(int frame)
        {
            if (currUpdate != frame)
            {
                foreach (var neuron in PreviousNeurons)
                {
                    neuron.Update(frame);
                }

                float sum = 0;
                for (int i = 0; i < PreviousNeurons.Length; i++)
                {
                    sum += PreviousNeurons[i].GetValue() * previousWeights[i];
                }

                cachedValue = sum;
                currUpdate = frame;
            }
        }
    }
}
