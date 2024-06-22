using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;

public static class MouseHook
{
    public static event EventHandler<MouseEventArgs> MouseAction = delegate { };

    private static LowLevelMouseProc _proc = HookCallback;
    private static IntPtr _hookID = IntPtr.Zero;

    public static void Start()
    {
        _hookID = SetHook(_proc);
    }

    public static void Stop()
    {
        UnhookWindowsHookEx(_hookID);
    }

    private static IntPtr SetHook(LowLevelMouseProc proc)
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule)
        {
            return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && (MouseMessages.WM_XBUTTONDOWN == (MouseMessages)wParam || MouseMessages.WM_XBUTTONUP == (MouseMessages)wParam))
        {
            int mouseButton = (int)(((MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT))).mouseData >> 16);
            if (mouseButton == 1) // XButton1
            {
                MouseAction(null, new MouseEventArgs(MouseButton.XButton1, MouseMessages.WM_XBUTTONDOWN == (MouseMessages)wParam ? MouseButtonState.Pressed : MouseButtonState.Released));
            }
            else if (mouseButton == 2) // XButton2
            {
                MouseAction(null, new MouseEventArgs(MouseButton.XButton2, MouseMessages.WM_XBUTTONDOWN == (MouseMessages)wParam ? MouseButtonState.Pressed : MouseButtonState.Released));
            }
        }

        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    private enum MouseMessages
    {
        WM_XBUTTONDOWN = 0x020B,
        WM_XBUTTONUP = 0x020C
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int x;
        public int y;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    private const int WH_MOUSE_LL = 14;
}

public class MouseEventArgs : EventArgs
{
    public MouseButton Button { get; }
    public MouseButtonState ButtonState { get; }

    public MouseEventArgs(MouseButton button, MouseButtonState buttonState)
    {
        Button = button;
        ButtonState = buttonState;
    }
}
