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

    public class Input
    {
        public Direction Direction { get; set; }
        private double intensity;
        public double Intensity
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
