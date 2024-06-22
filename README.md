# AutoClicker

🖱️ AutoClicker is a simple application that automates mouse clicks at specified intervals with jitter and settings saving. It also allows you to set hotkeys to start and stop clicking.

## Features

⏱️ Adjustable clicking interval with random jitter to simulate human-like clicks.

🔥 Set custom hotkeys to start and stop clicking.

## Requirements

- Windows OS
- .NET Framework

## Usage

1. Run `FlyClicker.exe`.
2. Adjust the Interval and Jitter sliders to your desired values.
3. Set Start and Stop hotkeys by clicking the "Set" button next to each textbox and pressing the desired keys.
4. Click the "Start" button or use the hotkey to start clicking.
5. Click the "Stop" button or use the hotkey to stop clicking.

## Known Issues

- **Cannot assign hotkeys using mouse buttons:** Due to limitations in the current implementation, assigning hotkeys to mouse buttons (e.g., XButton1, XButton2) is not supported. (FIXED)

## Build

If you want to build from source:

1. Clone this repository.
2. Open the solution file (`FlyClicker.sln`) in JetBrains Rider for example.
3. Build the solution (`Ctrl + Shift + F9`).

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Mouse and Keyboard hook implementations: [MouseHook.cs](FlyClicker/MouseHook.cs), [KeyHook.cs](FlyClicker/KeyHook.cs) based on examples from MSDN.
- Input simulation: [Clicker.cs](FlyClicker/Clicker.cs) using WindowsInput library.

