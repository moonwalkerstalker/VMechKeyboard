using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Media;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using KeyboardHookLite;

namespace VirtualMechKeyboard
{
    public partial class MainWindow : Window
    {
        private string selectedSoundPackFolder = "assets/mech1"; // Default sound pack folder
        private bool isMinimizedToTray = false;
        private NotifyIcon notifyIcon;
        private BackgroundWorker backgroundWorker;
        private Thread backgroundThread;

        private Dictionary<Key, DateTime> lastPlayTimes = new Dictionary<Key, DateTime>();
        private Random random = new Random();

        public MainWindow()
        {
            InitializeComponent();
            
       //     notifyIcon = new NotifyIcon();
       //     notifyIcon.Icon = new System.Drawing.Icon("assets/mouse-keyboard-hook-logo.png"); // Provide the path to your app's icon
       //     notifyIcon.Text = "Keyboard Sound Simulator";
       //     notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
       
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerAsync();

            KeyboardHook kbh = new KeyboardHook();

            kbh.KeyboardPressed += onKeyPress;
            
        }

        private void onKeyPress(object? sender, KeyboardHookEventArgs e)
        {
            if (e.KeyPressType == KeyboardHook.KeyPressType.KeyDown)
            {
                PlaySound(e.InputEvent.Key, "_down.wav");
            }
            else
            {
                PlaySound(e.InputEvent.Key, "_up.wav");
            }
        }
        
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.IsRepeat)
                return;

            PlaySound(e.Key, "_up.wav");
        }

        private void PlaySound(Key key, string action)
        {
            // Check if the key is a special key
            if (key == Key.Space || key == Key.Enter || key == Key.LeftShift || key == Key.RightShift ||
                key == Key.LeftCtrl || key == Key.RightCtrl || key == Key.LeftAlt || key == Key.RightAlt ||
                key == Key.Escape || key == Key.Back)
            {
                string soundPath = Path.Combine(selectedSoundPackFolder, key.ToString().ToLower() + action);
                PlaySoundFile(soundPath);
            }
            else
            {
                int randomSoundIndex = random.Next(1, 7);
                string soundPath = Path.Combine(selectedSoundPackFolder, "random_" + randomSoundIndex + action);
                PlaySoundFile(soundPath);
            }

            lastPlayTimes[key] = DateTime.Now;
        }

        private void PlaySoundFile(string soundPath)
        {
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, soundPath);

            if (File.Exists(fullPath))
            {
                using (SoundPlayer player = new SoundPlayer(fullPath))
                {
                    player.Play();
                }
            }
        }

        private void SoundPackComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)SoundPackComboBox.SelectedItem;
            selectedSoundPackFolder = selectedItem.Tag.ToString();
        }
        
        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                notifyIcon.Visible = true;
                isMinimizedToTray = true;
            }
            else
            {
                isMinimizedToTray = false;
            }
        }
        
        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                // Implement your background processing logic here
                Thread.Sleep(100); // Add a small delay to avoid excessive CPU usage
            }
        }




    }

}
