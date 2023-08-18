using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace VirtualMechKeyboard
{
    public partial class MainWindow : Window
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private IntPtr hookId = IntPtr.Zero;
        private Dispatcher dispatcher;
        
        public class KeyInfo
        {
            public MediaPlayer Sound { get; set; }
            public bool IsPressed { get; set; }
            public bool IsKeyDown { get; set; } // New property to track key press state
        }

        
        private Dictionary<Key, KeyInfo> keyInfoMap = new Dictionary<Key, KeyInfo>();

        public MainWindow()
        {
            InitializeComponent();
            dispatcher = Dispatcher.CurrentDispatcher;

            foreach (Key key in Enum.GetValues(typeof(Key)))
            {
                keyInfoMap[key] = new KeyInfo
                {
                    Sound = null, // No sound initially
                    IsPressed = false
                };
            }

            hookId = SetHook(KeyboardHookCallback);
            SetupSystemTray();
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                return NativeMethods.SetWindowsHookEx(WH_KEYBOARD_LL, proc, NativeMethods.GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_KEYUP))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Key key = (Key)KeyInterop.KeyFromVirtualKey(vkCode); // Convert to WPF Key enum

                if (keyInfoMap.ContainsKey(key))
                {
                    KeyInfo keyInfo = keyInfoMap[key];

                    if (wParam == (IntPtr)WM_KEYDOWN)
                    {
                        if (!keyInfo.IsKeyDown) // Check if the key is not already down
                        {
                            dispatcher.Invoke(() =>
                            {
                                keyInfo.Sound = CreateMediaPlayer(key, isKeyDown: true);
                                keyInfo.Sound?.Play();
                                keyInfo.IsPressed = true;
                                keyInfo.IsKeyDown = true;
                            });
                        }
                    }
                    else if (wParam == (IntPtr)WM_KEYUP)
                    {
                        dispatcher.Invoke(() =>
                        {
                            keyInfo.Sound?.Stop();
                            keyInfo.Sound = CreateMediaPlayer(key, isKeyDown: false);
                            keyInfo.Sound?.Play();
                            keyInfo.IsPressed = false;
                            keyInfo.IsKeyDown = false; // Reset the key press state
                        });
                    }
                }
            }

            return NativeMethods.CallNextHookEx(hookId, nCode, wParam, lParam);
        }

        //TODO KEYBOARD AS STREAM OVERLAY
        //TODO SHOW KEYBOARD ON SCREEN, ANIMATE KEYS PRESSED AND RELEASED

        private MediaPlayer CreateMediaPlayer(Key key, bool isKeyDown)
        {
            string soundFileName;

            if (key == Key.Enter) soundFileName = isKeyDown ? "enter_down" : "enter_up";
            else if (key == Key.Space) soundFileName = isKeyDown ? "space_down" : "space_up";
            // else if (key == Key.Escape) soundFileName = isKeyDown ? "esc_down" : "esc_up"; //Todo add ESC sound up and down
            else if (key == Key.LeftShift || key == Key.RightShift) soundFileName = isKeyDown ? "shift_down" : "shift_up";
            else if (key == Key.LeftCtrl || key == Key.RightCtrl) soundFileName = isKeyDown ? "ctrl_down" : "ctrl_up";
            else if (key == Key.Back) soundFileName = isKeyDown ? "backspace_down" : "backspace_up";
            else
            {
                // For other keys, generate a random sound file name
                Random random = new Random();
                int randomIndex = random.Next(1, 7); // Assuming you have 6 sounds
                soundFileName = isKeyDown ? $"random_{randomIndex}_down" : $"random_{randomIndex}_up";
            }

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string relativePath = $"assets/mech1/{soundFileName}.wav"; // Update the path if needed
            string fullPath = Path.Combine(baseDirectory, relativePath);

            if (File.Exists(fullPath))
            {
                var mediaPlayer = new MediaPlayer();
                mediaPlayer.Open(new Uri(fullPath));
                return mediaPlayer;
            }
            else
            {
                System.Windows.MessageBox.Show("Audio file does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }


        private void SetupSystemTray()
        {
            // Set up your system tray icon and event handling here
        }

     //  protected override void OnClosed(EventArgs e)
     //  {
     //      NativeMethods.UnhookWindowsHookEx(hookId);

     //      foreach (var sound in keySounds.Values)
     //      {
     //          sound?.Stop();
     //          sound?.Close();
     //      }

     //      base.OnClosed(e);
     //  }
    }
    public static class NativeMethods
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
    }

    public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
}
