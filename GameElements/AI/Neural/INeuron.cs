namespace PongML.GameElements.AI.Neural
{
    interface INeuron
    {
        INeuron[] PreviousNeurons { set; }
        float[] Weights { get; }
        float GetValue();
        void Update(int frame);
    }
}
