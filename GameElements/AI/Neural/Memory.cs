using Newtonsoft.Json;

namespace PongML.GameElements.AI.Neural
{
    class Memory : IPerceptron
    {
        [JsonIgnore]
        public INeuron[] PreviousNeurons { set => throw new System.InvalidOperationException(); }
        [JsonIgnore]
        public INeuron MemoryOutput { get; set; }

        [JsonIgnore]
        public float[] Weights => throw new System.InvalidOperationException();

        private float currValue;

        public float GetValue() => currValue;

        public void SetValue(float value) => throw new System.InvalidOperationException();

        public void Update(int frame)
        {
            currValue = MemoryOutput.GetValue();
        }
    }
}
