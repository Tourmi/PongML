namespace PongML.GameElements.AI.Neural
{
    class Perceptron : IPerceptron
    {
        private float value;

        public INeuron[] PreviousNeurons { set => throw new System.InvalidOperationException(); }

        public float[] Weights => throw new System.InvalidOperationException();

        public float GetValue() => value;

        public void SetValue(float value) => this.value = value;

        public void Update(int frame) { }
    }
}
