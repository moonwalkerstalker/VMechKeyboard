using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Media;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using KeyboardHookLite;

namespace VirtualMechKeyboard
{
    public partial class MainWindow : Window
    {
        private string selectedSoundPackFolder = "assets/mech1"; // Default sound pack folder
        private bool isMinimizedToTray = false;
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
        
        private void PlaySound(Key key, string action)
        {
            string keyName;
            
            bool isKeyDown = action == "_down";

            if (key == Key.Enter) keyName = "enter";
            else if (key == Key.Space) keyName = "space";
            else if (key==Key.Escape) keyName = "escape";
            else if (key is Key.LeftShift or Key.RightShift) keyName = "shift";
            else if (key is Key.LeftCtrl or Key.RightCtrl) keyName = "ctrl";
            else if (key==Key.Back) keyName = "backspace";
            else if (key==Key.Escape) keyName = "escape";
            else if (key is Key.LeftAlt or Key.RightAlt) keyName = "alt";
            else if (key is Key.LeftShift or Key.RightShift) keyName = "shift";
            else
            {
                int randomSoundIndex = random.Next(1, 7);
                keyName = "random_" + randomSoundIndex;
            }
            
            string soundPath = Path.Combine(selectedSoundPackFolder, keyName + action);
            PlaySoundFile(soundPath);
            
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
