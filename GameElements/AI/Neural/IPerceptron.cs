namespace PongML.GameElements.AI.Neural
{
    interface IPerceptron : INeuron
    {
        /// <summary>
        /// Sets the value of this perceptron
        /// </summary>
        /// <param name="value"></param>
        void SetValue(float value);
    }
}
