namespace PongML.GameElements.AI.Neural
{
    class Neuron : INeuron
    {
        private readonly INeuron[] previousNeurons;
        private readonly float[] previousWeights;
        private float cachedValue;

        public Neuron(INeuron[] previousNeurons, float[] previousWeights)
        {
            this.previousNeurons = previousNeurons;
            this.previousWeights = previousWeights;
        }

        public float[] Weights => previousWeights;

        public float GetCachedValue() => cachedValue;

        public float GetValue()
        {
            float sum = 0;
            for (int i = 0; i < previousNeurons.Length; i++)
            {
                sum += previousNeurons[i].GetValue() * previousWeights[i];
            }

            cachedValue = sum;

            return sum;
        }
    }
}
