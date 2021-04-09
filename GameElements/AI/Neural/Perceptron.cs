namespace PongML.GameElements.AI.Neural
{
    class Perceptron : IPerceptron
    {
        private float value;

        /// <summary>
        /// Since the perceptron is on the first layer of the neural network, it does not have previous neurons
        /// </summary>
        public INeuron[] PreviousNeurons { set => throw new System.InvalidOperationException(); }

        /// <summary>
        /// Since the perceptron is on the first layer of the neural network, it does not have previous neurons
        /// </summary>
        public float[] Weights => throw new System.InvalidOperationException();

        public float GetValue() => value;

        public void SetValue(float value) => this.value = value;

        public void Update(int frame) { }
    }
}
