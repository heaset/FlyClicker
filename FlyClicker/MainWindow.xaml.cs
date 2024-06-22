using System;
using System.Windows;
using System.Windows.Input;

namespace FlyClicker
{
    public partial class MainWindow : Window
    {
        private Clicker _clicker;
        private Key _startHotkey = Key.None;
        private Key _stopHotkey = Key.None;
        private bool _settingStartHotkey;
        private bool _settingStopHotkey;
        private SettingsManager _settingsManager;

        public MainWindow()
        {
            InitializeComponent();
            _clicker = new Clicker();
            _clicker.Jitter = 1;
            _clicker.Interval = 50;
            DataContext = _clicker;
            HookMouseEvents();
            HookKeyEvents();
            _settingsManager = new SettingsManager();
            LoadSettings();
        }
        
        private void LoadSettings()
        {
            _settingsManager.LoadSettings();
            IntervalSlider.Value = _settingsManager.Interval;
            JitterSlider.Value = _settingsManager.Jitter;
            _startHotkey = _settingsManager.StartHotkey;
            _stopHotkey = _settingsManager.StopHotkey;
            StartHotkeyBox.Text = _startHotkey.ToString();
            StopHotkeyBox.Text = _stopHotkey.ToString();
        }

        private void SaveSettings()
        {
            _settingsManager.StartHotkey = _startHotkey;
            _settingsManager.StopHotkey = _stopHotkey;
            _settingsManager.Interval = (int)IntervalSlider.Value;
            _settingsManager.Jitter = (int)JitterSlider.Value;
            _settingsManager.SaveSettings();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            _clicker.Interval = (int)IntervalSlider.Value;
            _clicker.Jitter = (int)JitterSlider.Value;
            _clicker.Start();
        }

        private async void StopButton_Click(object sender, RoutedEventArgs e)
        {
            await _clicker.StopAsync();
        }

        private void HookMouseEvents()
        {
            MouseHook.Start();
            MouseHook.MouseAction += MouseHook_MouseAction;
        }

        private void HookKeyEvents()
        {
            KeyHook.Start();
            KeyHook.KeyAction += KeyHook_KeyAction;
        }

        private void MouseHook_MouseAction(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButton.XButton1 && e.ButtonState == MouseButtonState.Pressed)
            {
                _clicker.Start();
            }
            else if (e.Button == MouseButton.XButton2 && e.ButtonState == MouseButtonState.Released)
            {
                _clicker.Stop();
            }
        }

        private async void KeyHook_KeyAction(object? sender, KeyEventArgs e)
        {
            if (e.RoutedEvent == Keyboard.KeyDownEvent)
            {
                if (_settingStartHotkey)
                {
                    _startHotkey = e.Key;
                    StartHotkeyBox.Text = _startHotkey.ToString();
                    _settingStartHotkey = false;
                }
                else if (_settingStopHotkey)
                {
                    _stopHotkey = e.Key;
                    StopHotkeyBox.Text = _stopHotkey.ToString();
                    _settingStopHotkey = false;
                }
                else
                {
                    if (e.Key == _startHotkey)
                    {
                        _clicker.Start();
                    }
                    else if (e.Key == _stopHotkey)
                    {
                        await _clicker.StopAsync();
                    }
                }
            }
        }

        private void SetStartHotkey_Click(object sender, RoutedEventArgs e)
        {
            _settingStartHotkey = true;
            StartHotkeyBox.Text = "Press any key...";
        }

        private void SetStopHotkey_Click(object sender, RoutedEventArgs e)
        {
            _settingStopHotkey = true;
            StopHotkeyBox.Text = "Press any key...";
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            SaveSettings();
            MouseHook.Stop();
            KeyHook.Stop();
        }
    }
}
