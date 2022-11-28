using Microsoft.Web.WebView2.WinForms;
using System.Diagnostics;

namespace DragDropWinFormsApp2
{
    public partial class Form1 : Form
    {
        private DragService? _dragService;
        private readonly WebView2 _wv;

        public Form1()
        {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.None;

            var mainfilePath =
                Path.Combine(
                    Path.GetDirectoryName(typeof(Form1).Assembly.Location),
                    "mainfile.html");

            _wv = new WebView2()
            {
                Dock = DockStyle.Fill,
                Source = new Uri(mainfilePath),
            };
            _wv.CoreWebView2InitializationCompleted += Wv_CoreWebView2InitializationCompleted;
            Controls.Add(_wv);

        }

        private void Wv_CoreWebView2InitializationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            _wv.CoreWebView2.Settings.IsWebMessageEnabled = true;
            _wv.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
        }

        private void CoreWebView2_WebMessageReceived(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            Debug.WriteLine("CoreWebView2_WebMessageReceived: " + e.TryGetWebMessageAsString());

            _dragService.BeginDrag();
        }

        //override 
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            Task.Run(() => {
                Invoke(() =>
                {
                    Thread.Sleep(2000);
                    _dragService = new DragService();
                });

            });
        }
    }

    public class DragService
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int WM_NCLBUTTONUP = 0xA2;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private IntPtr MainHandle { get; init; }

        public DragService()
        {
            MainHandle = Application.OpenForms.OfType<Form1>().FirstOrDefault().Handle;
            Debug.WriteLine($"{nameof(DragService)} ctor");
        }

        public void BeginDrag()
        {
            ReleaseCapture();
            var hr = SendMessage(MainHandle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            Debug.WriteLine($"{nameof(BeginDrag)} - HR = {hr}");
        }

        public void EndDrag()
        {
            ////ReleaseCapture();
            //SendMessage(MainHandle, WM_NCLBUTTONUP, HT_CAPTION, 0);
        }
    }
}