using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PongML.GameElements
{
    public enum Direction
    {
        None,
        Up,
        Down
    }

    /// <summary>
    /// Class that represents a single input, up, down or none. As well as an intensity between 0 and 1.
    /// </summary>
    public class Input
    {
        /// <summary>
        /// The direction of the input
        /// </summary>
        public Direction Direction { get; set; }
        private float intensity;
        /// <summary>
        /// Between 0 and 1
        /// </summary>
        public float Intensity
        {
            get => intensity; set
            {
                intensity = value;
                if (intensity > 1)
                {
                    intensity = 1;
                }
                if (intensity < 0)
                {
                    intensity = 0;
                }
            }
        }
    }
}
