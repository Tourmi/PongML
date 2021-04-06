using PongML.GameElements;
using PongML.GameElements.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PongML.Models
{
    public class Game
    {
        public int CurrentFrame { get; set; }
        public Vector2 BallPos { get; set; }
        public Vector2 BallDirection { get; set; }
        public float BallSpeed { get; set; }
        public float InitialBallSpeed { get; set; }
        public float BallSpeedIncrement { get; set; }
        public float BallSize { get; set; }
        public int ArenaWidth { get; set; }
        public int ArenaHeight { get; set; }
        public int PaddleSize { get; set; }
        public float PaddleSpeed { get; set; }
        public readonly IPlayer[] Players;

        private readonly List<IArtificialIntelligence> ais;
        private readonly Random random;

        public Game() : this(new HumanPlayer(Key.W, Key.S), new RandomAI()) { }

        public Game(IPlayer player1, IPlayer player2)
        {
            ais = new List<IArtificialIntelligence>();
            Players = new IPlayer[] { player1, player2 };
            random = new Random();

            if (player1 is IArtificialIntelligence ai1)
            {
                ais.Add(ai1);
                if (ai1 is Brain brain)
                {
                    brain.ReverseHorizontal = true;
                }
            }
            if (player2 is IArtificialIntelligence ai2)
            {
                ais.Add(ai2);
            }

            BallSize = 16;
            ArenaWidth = 1000;
            ArenaHeight = 500;
            PaddleSize = 75;
            PaddleSpeed = 10;

            player1.PaddlePosition = ArenaHeight / 2;
            player2.PaddlePosition = ArenaHeight / 2;

            InitialBallSpeed = 5;
            BallSpeed = InitialBallSpeed;
            BallPos = new Vector2(ArenaWidth / 2, ArenaHeight / 2);
            BallDirection = new Vector2(-1, 0);
            BallSpeedIncrement = 1;
        }

        public Game(GameConfiguration gc, IPlayer player1, IPlayer player2) : this(player1, player2)
        {
            BallSpeed = gc.InitialBallSpeed;
            BallSpeedIncrement = gc.BallSpeedIncrement;
            PaddleSpeed = gc.PaddleSpeed;
            InitialBallSpeed = gc.InitialBallSpeed;
        }

        public void Update()
        {
            foreach (var ai in ais)
            {
                ai.Update(this);
            }

            foreach (IPlayer player in Players)
            {
                var input = player.GetInput();

                float move = PaddleSpeed * input.Intensity;

                switch (input.Direction)
                {
                    case Direction.None:
                        move = 0;
                        break;
                    case Direction.Up:
                        move *= -1;
                        break;
                    default:
                        break;
                }

                player.PaddlePosition += move;
                if (player.PaddlePosition + (PaddleSize / 2) > ArenaHeight)
                {
                    player.PaddlePosition = ArenaHeight - PaddleSize / 2;
                }
                else if (player.PaddlePosition - (PaddleSize / 2) < 0)
                {
                    player.PaddlePosition = 0 + PaddleSize / 2;
                }
            }

            BallPos += BallDirection * BallSpeed;
            if (BallPos.Y + (BallSize / 2) > ArenaHeight)
            {
                BallPos = new Vector2(BallPos.X, ArenaHeight - BallSize / 2);
                BallDirection = new Vector2(BallDirection.X, -BallDirection.Y);
            }
            else if (BallPos.Y - (BallSize / 2) < 0)
            {
                BallPos = new Vector2(BallPos.X, 0 + BallSize / 2);
                BallDirection = new Vector2(BallDirection.X, -BallDirection.Y);
            }

            if (BallPos.X + (BallSize / 2) > ArenaWidth)
            {
                BallPos = new Vector2(ArenaWidth - (BallSize / 2), BallPos.Y);
                ballExitedSide(1);
            }
            else if (BallPos.X - (BallSize / 2) < 0)
            {
                BallPos = new Vector2(0 + (BallSize / 2), BallPos.Y);
                ballExitedSide(0);
            }

            CurrentFrame++;
        }

        private void ballExitedSide(int playerIndex)
        {
            int oppositePlayer = (playerIndex + 1) % 2;

            float minBallPos = BallPos.Y - BallSize / 2;
            float maxBallPos = BallPos.Y + BallSize / 2;
            float minPaddlePos = Players[playerIndex].PaddlePosition - PaddleSize / 2;
            float maxPaddlePos = Players[playerIndex].PaddlePosition + PaddleSize / 2;
            if (maxBallPos >= minPaddlePos && minBallPos <= maxPaddlePos)
            {
                //The ball bounces back
                float yAngle = (BallPos.Y - Players[playerIndex].PaddlePosition) / PaddleSize * 4;

                BallDirection = Vector2.Normalize(new Vector2(oppositePlayer * 2 - 1, yAngle));
                BallSpeed += BallSpeedIncrement;
            }
            else
            {
                //The ball passed through
                Players[oppositePlayer].Score++;
                BallPos = new Vector2(ArenaWidth / 2, ArenaHeight / 2);
                BallSpeed = InitialBallSpeed;
                BallDirection = Vector2.Normalize(new Vector2((oppositePlayer * 2) - 1, (float)((random.NextDouble() - 0.5) * 4)));
            }
        }
    }
}
