using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PongML.GameElements
{
    public interface IPlayer
    {
        float PaddlePosition { get; set; }
        int Score { get; set; }

        Input GetInput();
    }
}
