using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Threading;


namespace PlayAroundwithImages2
{
    /// <summary>
    /// クリップボードを監視する
    /// </summary>
    public class ClipboardMonitor
    {
        [DllImport("user32.dll", SetLastError = true)]
        private extern static void AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        private extern static void RemoveClipboardFormatListener(IntPtr hwnd);

        IntPtr handle;

        private const int WM_CLIPBOARD = 0x31D;

        HwndSource hwndSource = null;
        public ClipboardMonitor(IntPtr handle)
        {
            hwndSource = HwndSource.FromHwnd(handle);
            hwndSource.AddHook(WndProc);
            this.handle = handle;
        }

        //なぜかイベントが2回呼び出されることがあるのでタイマーで対処
        Timer timer = new Timer(timerCallback, null, 0, 50);
        static int count = 0;
        // TimerCallbackをラムダ式で定義
        static TimerCallback timerCallback = state =>
        {
            if (count < 10)
                count++;
        };

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_CLIPBOARD && count != 0)
            {
                        this.Raise();
            }
            count = 0;
            return IntPtr.Zero;
        }

        public event EventHandler ChangeClipboard;

        private void Raise()
        {
            ChangeClipboard?.Invoke(this, EventArgs.Empty);
        }
        public void Start()
        {
            AddClipboardFormatListener(handle);
        }

        public void Stop()
        {
            RemoveClipboardFormatListener(handle);
        }


        [System.Runtime.InteropServices.ComVisible(true)]
        public interface IDisposable
        {
            void Dispose();
        }

        public void Dispose()
        {
            RemoveClipboardFormatListener(handle);
        }

    }
}
