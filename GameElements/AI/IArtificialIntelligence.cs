using PongML.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PongML.GameElements
{
    interface IArtificialIntelligence
    {
        void Update(Game game);
        Input GetInput();
    }
}
