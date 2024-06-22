using System.Windows;
using System.Windows.Input;

namespace FlyClicker
{
    public partial class MainWindow : Window
    {
        private Clicker _clicker;
        private String _startHotkey = Key.None.ToString();
        private String _stopHotkey = Key.None.ToString();
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
            StartHotkeyBox.Text = _startHotkey;
            StopHotkeyBox.Text = _stopHotkey;
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
        
        private async void MouseHook_MouseAction(object? sender, MouseEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                CheckForIdenticalKeys(e.Button);
                if (_settingStartHotkey)
                {
                    _startHotkey = e.Button.ToString();
                    StartHotkeyBox.Text = _startHotkey;
                    _settingStartHotkey = false;
                }
                else if (_settingStopHotkey)
                {
                    _stopHotkey = e.Button.ToString();
                    StopHotkeyBox.Text = _stopHotkey;
                    _settingStopHotkey = false;
                }
                else
                {
                    if (e.Button.ToString() == _startHotkey)
                    {
                        _clicker.Start();
                    }
                    else if (e.Button.ToString() == _stopHotkey)
                    {
                        await _clicker.StopAsync();
                    }
                }
            }
        }

        private async void KeyHook_KeyAction(object? sender, KeyEventArgs e)
        {
            if (e.RoutedEvent == Keyboard.KeyDownEvent)
            {
                CheckForIdenticalKeys(e.Key);
                if (_settingStartHotkey)
                {
                    _startHotkey = e.Key.ToString();
                    StartHotkeyBox.Text = _startHotkey;
                    _settingStartHotkey = false;
                }
                else if (_settingStopHotkey)
                {
                    _stopHotkey = e.Key.ToString();
                    StopHotkeyBox.Text = _stopHotkey;
                    _settingStopHotkey = false;
                }
                else
                {
                    if (e.Key.ToString() == _startHotkey)
                    {
                        _clicker.Start();
                    }
                    else if (e.Key.ToString() == _stopHotkey)
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

        private async void CheckForIdenticalKeys(object key)
        {
            if (_settingStartHotkey || _settingStopHotkey)
            {
                return;
            }
            String stringKey = key.ToString();
            if (stringKey == _startHotkey && stringKey == _stopHotkey)
            {
                if (_clicker.IsRunning())
                {
                    await _clicker.StopAsync();
                }
                else
                {
                    _clicker.Start();
                }
            }
        }
    }
}
