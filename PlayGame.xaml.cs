using Microsoft.Win32;
using PongML.GameElements;
using PongML.GameElements.AI;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for PlayGame.xaml
    /// </summary>
    public partial class PlayGame : Page
    {
        private Models.GameConfiguration gc;
        public PlayGame(Models.GameConfiguration gc)
        {
            InitializeComponent();
            this.gc = gc;
        }

        private void Btn_HumanVsHuman_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.Content = new GamePage(gc, new Models.Game(gc, new HumanPlayer(Key.W, Key.S),new HumanPlayer(Key.Up, Key.Down)));
        }

        private void Btn_HumanVsAI_Click(object sender, RoutedEventArgs e)
        {
            Brain brain = null;

            var openFileDialog = new OpenFileDialog
            {
                Filter = "Json file (*.json)|*.json"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                brain = Brain.FromJson(File.ReadAllText(openFileDialog.FileName));
            }

            if (brain == null)
            {
                MessageBox.Show("Invalid AI file");
                return;
            }

            var window = Window.GetWindow(this);
            window.Content = new GamePage(gc, new Models.Game(gc, new HumanPlayer(Key.W, Key.S), brain));
        }

        private void Btn_AIvsAI_Click(object sender, RoutedEventArgs e)
        {
            Brain brain1 = null;
            Brain brain2 = null;

            var openFileDialog = new OpenFileDialog
            {
                Filter = "Json file (*.json)|*.json"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                brain1 = Brain.FromJson(File.ReadAllText(openFileDialog.FileName));
            }

            if (brain1 == null)
            {
                MessageBox.Show("Invalid AI file");
                return;
            }

            openFileDialog = new OpenFileDialog
            {
                Filter = "Json file (*.json)|*.json"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                brain2 = Brain.FromJson(File.ReadAllText(openFileDialog.FileName));
            }

            if (brain2 == null)
            {
                MessageBox.Show("Invalid AI file");
                return;
            }

            var window = Window.GetWindow(this);
            window.Content = new GamePage(gc, new Models.Game(gc, brain1, brain2));
        }

        private void Btn_BackMainMenu_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.Content = new MainMenu(gc);
        }
    }
}
