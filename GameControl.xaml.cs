using PongML.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PongML
{
    /// <summary>
    /// Interaction logic for GameControl.xaml
    /// </summary>
    public partial class GameControl : UserControl
    {
        private Game game;
        private readonly Timer updateTimer;

        public GameControl()
        {
            InitializeComponent();

            updateTimer = new Timer(16);
            updateTimer.Elapsed += update;
        }

        public void Init(Game game)
        {
            this.game = game;

            Canvas.SetTop(PaddleLeft, game.Players[0].PaddlePosition - game.PaddleSize / 2);
            Canvas.SetTop(PaddleRight, game.Players[1].PaddlePosition - game.PaddleSize / 2);
            Canvas.SetTop(Ball, game.BallPos.Y);
            Canvas.SetLeft(Ball, game.BallPos.X);
        }

        public void Start()
        {
            updateTimer.Start();
        }

        public void Stop()
        {
            updateTimer.Stop();
        }

        private void update(object sender, ElapsedEventArgs e)
        {
            game.Update();
            Dispatcher.Invoke(() =>
            {
                Canvas.SetTop(PaddleLeft, game.Players[0].PaddlePosition - game.PaddleSize/2);
                Canvas.SetTop(PaddleRight, game.Players[1].PaddlePosition - game.PaddleSize / 2);
                Canvas.SetTop(Ball, game.BallPos.Y - game.BallSize/2);
                Canvas.SetLeft(Ball, game.BallPos.X + 50 - game.BallSize/2);

                Score.Text = $"{game.Players[0].Score} - {game.Players[1].Score}";
            }, System.Windows.Threading.DispatcherPriority.Render);
        }
    }
}
