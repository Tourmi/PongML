namespace PongML.GameElements.AI.Neural
{
    class Perceptron : IPerceptron
    {
        private float value;

        public float[] Weights => throw new System.InvalidOperationException();

        public float GetCachedValue() => value;

        public float GetValue() => value;

        public void SetValue(float value) => this.value = value;
    }
}
