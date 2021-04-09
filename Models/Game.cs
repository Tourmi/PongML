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
    /// <summary>
    /// Class representing a single game of Pong
    /// </summary>
    public class Game
    {
        /// <summary>
        /// The current frame of the game
        /// </summary>
        public int CurrentFrame { get; set; }
        /// <summary>
        /// Position of the ball
        /// </summary>
        public Vector2 BallPos { get; set; }
        /// <summary>
        /// Direction the ball is going in
        /// </summary>
        public Vector2 BallDirection { get; set; }
        /// <summary>
        /// Current speed of the ball
        /// </summary>
        public float BallSpeed { get; set; }
        /// <summary>
        /// The speed at which the ball spawns at
        /// </summary>
        public float InitialBallSpeed { get; set; }
        /// <summary>
        /// The speed that the ball gains whenever it is bounced back
        /// </summary>
        public float BallSpeedIncrement { get; set; }
        /// <summary>
        /// The size of the ball
        /// </summary>
        public float BallSize { get; set; }
        /// <summary>
        /// The width of the arena
        /// </summary>
        public int ArenaWidth { get; set; }
        /// <summary>
        /// The height of the arena
        /// </summary>
        public int ArenaHeight { get; set; }
        /// <summary>
        /// Size of a single player's paddle
        /// </summary>
        public int PaddleSize { get; set; }
        /// <summary>
        /// Maximum speed at which the paddle may go
        /// </summary>
        public float PaddleSpeed { get; set; }
        /// <summary>
        /// This game's players. Will always be an array with two elements
        /// </summary>
        public readonly IPlayer[] Players;

        /// <summary>
        /// This game's AIs. Can have between 0 to 2 of them
        /// </summary>
        private readonly List<IArtificialIntelligence> ais;
        private readonly Random random;

        /// <summary>
        /// Default game of a human player versus a random AI
        /// </summary>
        public Game() : this(new HumanPlayer(Key.W, Key.S), new RandomAI()) { }

        /// <summary>
        /// New game with the two given players and a default configuration
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
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
                    brain.PlayerNumber = 0;
                }
            }
            if (player2 is IArtificialIntelligence ai2)
            {
                ais.Add(ai2);
                if (ai2 is Brain brain)
                {
                    brain.ReverseHorizontal = false;
                    brain.PlayerNumber = 1;
                }
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
            BallDirection = Vector2.Normalize(new Vector2(random.Next(2) * 2 - 1, (float)random.NextDouble() - 0.5f));
            BallSpeedIncrement = 1;
        }

        /// <summary>
        /// New game with the given configuration and players
        /// </summary>
        /// <param name="gc"></param>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        public Game(GameConfiguration gc, IPlayer player1, IPlayer player2) : this(player1, player2)
        {
            BallSpeed = gc.InitialBallSpeed;
            BallSpeedIncrement = gc.BallSpeedIncrement;
            PaddleSpeed = gc.PaddleSpeed;
            InitialBallSpeed = gc.InitialBallSpeed;
        }

        /// <summary>
        /// Updates the whole game by one frame
        /// </summary>
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
                //If we moved out of the lower bounds, cap the position on the lower bound
                if (player.PaddlePosition + (PaddleSize / 2) > ArenaHeight)
                {
                    player.PaddlePosition = ArenaHeight - PaddleSize / 2;
                } //If we moved out of the upper bounds, cap the position on the upper bound
                else if (player.PaddlePosition - (PaddleSize / 2) < 0)
                {
                    player.PaddlePosition = 0 + PaddleSize / 2;
                }
            }

            BallPos += BallDirection * BallSpeed;
            //If the ball moved out the lower bound, cap it
            if (BallPos.Y + (BallSize / 2) > ArenaHeight)
            {
                BallPos = new Vector2(BallPos.X, ArenaHeight - BallSize / 2);
                BallDirection = new Vector2(BallDirection.X, -BallDirection.Y);
            } //If the ball moved out the upper bound, cap it
            else if (BallPos.Y - (BallSize / 2) < 0)
            {
                BallPos = new Vector2(BallPos.X, 0 + BallSize / 2);
                BallDirection = new Vector2(BallDirection.X, -BallDirection.Y);
            }

            //If the ball moved out the right bounds, cap it and process if it should bounce or respawn
            if (BallPos.X + (BallSize / 2) > ArenaWidth)
            {
                BallPos = new Vector2(ArenaWidth - (BallSize / 2), BallPos.Y);
                ballExitedSide(1);
            } //If the ball moved out the left bounds, cap it and process if it should bounce or respawn
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
            //Has the ball hit the player's paddle?
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
