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
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Page
    {
        private Models.GameConfiguration gc;

        public MainMenu(Models.GameConfiguration gc)
        {
            InitializeComponent();
            this.gc = gc;
        }

        private void Btn_Quit_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Do you really want to quit the application?", "Quit", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void Btn_Configuration_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.Content = new GameConfiguration(gc);
        }

        private void Btn_Training_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.Content = new Training(gc);
        }

        private void Btn_PlayGame_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.Content = new PlayGame(gc);
        }
    }
}
