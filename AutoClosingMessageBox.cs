using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Threading.Timer;

namespace RestServicePlugin
{
    public class AutoClosingMessageBox
    {
        private const int WM_CLOSE = 0x0010;
        private readonly string _caption;
        private readonly Timer _timeoutTimer;

        private AutoClosingMessageBox(string text, string caption, int timeout)
            : this(text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, timeout)
        {
        }

        private AutoClosingMessageBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, int timeout)
        {
            _caption = caption;
            _timeoutTimer = new Timer(OnTimerElapsed,
                null, timeout, Timeout.Infinite);
            MessageBox.Show(text, caption, buttons, icon);
        }

        public static void Show(string text, string caption, int timeout)
        {
            new AutoClosingMessageBox(text, caption, timeout);
        }

        public static void Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, int timeout)
        {
            new AutoClosingMessageBox(text, caption, buttons, icon, timeout);
        }

        private void OnTimerElapsed(object state)
        {
            IntPtr mbWnd = FindWindow(null, _caption);
            if (mbWnd != IntPtr.Zero)
                SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            _timeoutTimer.Dispose();
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
    }
}