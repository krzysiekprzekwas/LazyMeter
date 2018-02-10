using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LazyMeter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public class WindowWrapper : IWin32Window
        {
            internal IntPtr handle;
            internal String title;

            public IntPtr Handle
            {
                get { return handle; }
            }

            public String Title
            {
                get { return title; }
            }
        }

        internal static class NativeMethods
        {
            public static readonly Int32 GWL_STYLE = -16;
            public static readonly UInt64 WS_VISIBLE = 0x10000000L;
            public static readonly UInt64 WS_BORDER = 0x00800000L;
            public static readonly UInt64 DESIRED_WS = WS_BORDER | WS_VISIBLE;

            public delegate Boolean EnumWindowsCallback(IntPtr hwnd, Int32 lParam);

            public static List<WindowWrapper> GetAllWindows()
            {
                List<WindowWrapper> windows = new List<WindowWrapper>();
                StringBuilder buffer = new StringBuilder(100);
                EnumWindows(delegate (IntPtr hwnd, Int32 lParam)
                {
                    if ((GetWindowLongA(hwnd, GWL_STYLE) & DESIRED_WS) == DESIRED_WS)
                    {
                        GetWindowText(hwnd, buffer, buffer.Capacity);
                        WindowWrapper wnd = new WindowWrapper();
                        wnd.handle = hwnd;
                        wnd.title = buffer.ToString();
                        windows.Add(wnd);
                    }
                    return true;
                }, 0);

                return windows;
            }

            [DllImport("user32.dll")]
            static extern Int32 EnumWindows(EnumWindowsCallback lpEnumFunc, Int32 lParam);

            [DllImport("user32.dll")]
            public static extern void GetWindowText(IntPtr hWnd, StringBuilder lpString, Int32 nMaxCount);

            [DllImport("user32.dll")]
            static extern UInt64 GetWindowLongA(IntPtr hWnd, Int32 nIndex);
        }
        
        public MainWindow()
        {
            InitializeComponent();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();

            
        }

        void timer_Tick(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            foreach (var wnd in NativeMethods.GetAllWindows())
            {
                if (wnd.Title != "")
                    listBox1.Items.Add("Process: " + wnd.Title);
            }
        }
    }
}
