namespace PongML.GameElements.AI.Neural
{
    class Memory : IPerceptron
    {
        public INeuron MemoryOutput { get; set; }

        public float[] Weights => throw new System.InvalidOperationException();

        private float currValue;

        public float GetValue() => currValue;

        public void SetValue(float value) => currValue = value;

        public float GetCachedValue() => currValue;
    }
}
