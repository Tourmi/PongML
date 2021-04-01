namespace PongML.GameElements.AI.Neural
{
    interface INeuron
    {
        float[] Weights { get; }
        float GetValue();
        float GetCachedValue();
    }
}
