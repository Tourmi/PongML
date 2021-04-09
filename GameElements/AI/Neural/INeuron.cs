namespace PongML.GameElements.AI.Neural
{
    /// <summary>
    /// Interface used by all neurons in the neural system
    /// </summary>
    interface INeuron
    {
        /// <summary>
        /// The neurons that come before this one.
        /// </summary>
        INeuron[] PreviousNeurons { set; }
        /// <summary>
        /// The weight of the connection of the neurons that come before this one.
        /// </summary>
        float[] Weights { get; }
        /// <summary>
        /// Gets the value that is updated via <see cref="Update(int)"/>
        /// </summary>
        /// <returns></returns>
        float GetValue();
        /// <summary>
        /// Updates the stored value of this neuron
        /// </summary>
        /// <param name="frame">The current frame of the update. Avoids duplicate updates.</param>
        void Update(int frame);
    }
}
