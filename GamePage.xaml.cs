using PongML.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : Page
    {
        private readonly Game game;
        private readonly Models.GameConfiguration gc;

        public GamePage(Models.GameConfiguration gc, Game game)
        {
            InitializeComponent();

            this.game = game;
            this.gc = gc;
        }


        private void Btn_Start_Click(object sender, RoutedEventArgs e)
        {
            GameScreen.Init(game);
            GameScreen.Start();
            Btn_Start.Visibility = Visibility.Collapsed;
        }

        private void Btn_BackMainMenu_Click(object sender, RoutedEventArgs e)
        {
            GameScreen.Stop();

            var window = Window.GetWindow(this);
            window.Content = new MainMenu(gc);
        }
    }
}
