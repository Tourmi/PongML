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
    /// Interaction logic for Training.xaml
    /// </summary>
    public partial class Training : Page
    {
        private Models.GameConfiguration gc;
        public Training(Models.GameConfiguration gc)
        {
            InitializeComponent();
            this.gc = gc;
        }

        private void Btn_TrainingOptions_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_Training_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_BackMainMenu_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.Content = new MainMenu(gc);
        }
    }
}
