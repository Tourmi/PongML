using Microsoft.Win32;
using PongML.SimulationElements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
        private Simulation sim;
        public Training(Models.GameConfiguration gc)
        {
            InitializeComponent();
            this.gc = gc;
            Btn_StopTraining.IsEnabled = false;
        }

        private void Btn_Training_Click(object sender, RoutedEventArgs e)
        {
            if (sim != null && !sim.Ready)
            {
                return;
            }

            sim = new Simulation(gc);

            sim.NewGeneration += OnNewRound;

            ThreadPool.QueueUserWorkItem(p => sim.Start());

            Btn_StopTraining.IsEnabled = true;
            Btn_StartTraining.IsEnabled = false;
            Btn_StartTrainingFromFile.IsEnabled = false;
        }

        private void Btn_StopTraining_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sim.Stop();
                sim.NewGeneration -= OnNewRound;
            }
            catch (Exception ex)
            {
                throw new Exception("Failure when stopping the simulation", ex);
            }

            Btn_StopTraining.IsEnabled = false;
            Btn_StartTraining.IsEnabled = true;
            Btn_StartTrainingFromFile.IsEnabled = true;
        }

        private void Btn_TrainingFromFile_Click(object sender, RoutedEventArgs e)
        {
            if (sim != null && !sim.Ready)
            {
                return;
            }

            GameElements.AI.Brain brain = null;

            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Json file (*.json)|*.json";
            if (openFileDialog.ShowDialog() == true)
            {
                brain = GameElements.AI.Brain.FromJson(File.ReadAllText(openFileDialog.FileName));
            }

            if (brain == null)
            {
                MessageBox.Show("Failed to load the AI file.");
                return;
            }

            sim = new Simulation(brain, gc);

            sim.NewGeneration += OnNewRound;

            ThreadPool.QueueUserWorkItem(p => sim.Start());

            Btn_StopTraining.IsEnabled = true;
            Btn_StartTraining.IsEnabled = false;
            Btn_StartTrainingFromFile.IsEnabled = false;
        }

        private void Btn_BackMainMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sim?.Stop();
                sim.NewGeneration -= OnNewRound;
            }
            catch (Exception ex)
            {
                throw new Exception("Failure when stopping the simulation", ex);
            }

            var window = Window.GetWindow(this);
            window.Content = new MainMenu(gc);
        }

        private void OnNewRound()
        {
            Dispatcher.Invoke(() =>
            {
                TextBlock_Round.Text = $"Generation: {sim.Round}";
            }, System.Windows.Threading.DispatcherPriority.Render);
        }
    }
}
