using Microsoft.Win32;
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
    /// Interaction logic for GameConfiguration.xaml
    /// </summary>
    public partial class GameConfiguration : Page
    {
        public GameConfiguration(Models.GameConfiguration gc)
        {
            InitializeComponent();
            SetConfiguration(gc);
        }

        public void SetConfiguration(Models.GameConfiguration gc)
        {
            NeuronCount.Value = gc.NeuronCount;
            LayerCount.Value = gc.LayerCount;
            MemoryNeuronCount.Value = gc.MemoryNeuronCount;
            BaseEvolutionFactor.Value = gc.BaseEvolutionFactor;

            NumberOfAIs.Value = gc.NumberOfAIs;
            KeepBestAIs.Value = gc.KeepBestAIs;
            MaximumChildren.Value = gc.MaximumChildrenPerAi;
            GameLength.Value = gc.GameLength;
            SaveBestAIAfterEveryRound.IsChecked = gc.SaveBestAIAfterEveryRound;
            SaveBestAIAfterStoppingSim.IsChecked = gc.SaveBestAIAfterStoppingSim;

            InitialBallSpeed.Value = gc.InitialBallSpeed;
            BallSpeedIncrement.Value = gc.BallSpeedIncrement;
            PaddleSpeed.Value = gc.PaddleSpeed;
        }

        public Models.GameConfiguration GetGameConfiguration()
        {
            Models.GameConfiguration gc = new Models.GameConfiguration()
            {
                NeuronCount = (int)NeuronCount.Value,
                LayerCount = (int)LayerCount.Value,
                MemoryNeuronCount = (int)MemoryNeuronCount.Value,
                BaseEvolutionFactor = (int)BaseEvolutionFactor.Value,
                NumberOfAIs = (uint)NumberOfAIs.Value,
                KeepBestAIs = (uint)KeepBestAIs.Value,
                MaximumChildrenPerAi = (uint)MaximumChildren.Value,
                GameLength = (int)GameLength.Value,
                SaveBestAIAfterEveryRound = (bool)SaveBestAIAfterEveryRound.IsChecked,
                SaveBestAIAfterStoppingSim = (bool)SaveBestAIAfterStoppingSim.IsChecked,
                InitialBallSpeed = (float)InitialBallSpeed.Value,
                BallSpeedIncrement = (float)BallSpeedIncrement.Value,
                PaddleSpeed = (float)PaddleSpeed.Value
            };

            return gc;
        }

        private void Btn_BackMainMenu_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.Content = new MainMenu(GetGameConfiguration());
        }

        private void Btn_LoadConfig_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Json file (*.json)|*.json";
            if (openFileDialog.ShowDialog() == true)
            {
                Models.GameConfiguration gc = Models.GameConfiguration.FromJson(File.ReadAllText(openFileDialog.FileName));
                SetConfiguration(gc);
            }
                
        }

        private void Btn_SaveConfig_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Json file (*.json)|*.json";
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, GetGameConfiguration().ToJson());
            }
        }
    }
}
