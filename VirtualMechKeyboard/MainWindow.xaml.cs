using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
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
using Path = System.IO.Path;

namespace VirtualMechKeyboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string relativePath = @"assets\mech1\down1.wav"; // Update the path if needed
            string fullPath = Path.Combine(baseDirectory, relativePath);
            
            var mediaPlayerBtnDown = new MediaPlayer();
            mediaPlayerBtnDown.Open(new Uri(fullPath));
            mediaPlayerBtnDown.Play();
        }

        private void UIElement_OnKeyUp(object sender, KeyEventArgs e)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string relativePath = @"assets\mech1\up1.wav"; // Update the path if needed
            string fullPath = Path.Combine(baseDirectory, relativePath);
            
            var mediaPlayerBtnDown = new MediaPlayer();
            mediaPlayerBtnDown.Open(new Uri(fullPath));
            mediaPlayerBtnDown.Play();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            //EXECUTE AUDIO SOUND ON PRESS
            //AND EXECUTE SECOND AUDIO SOUND ON RELEASE
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string relativePath = @"assets\nk-cream\a.wav"; // Update the path if needed
            string fullPath = Path.Combine(baseDirectory, relativePath);

            if (File.Exists(fullPath))
            {
                var mediaPlayerBtn = new MediaPlayer();
                mediaPlayerBtn.Open(new Uri(fullPath));
                mediaPlayerBtn.Play();
            }
            else
            {
                MessageBox.Show("Audio file does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}