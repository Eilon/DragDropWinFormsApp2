using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Web.WebView2.WinForms;
using System.Diagnostics;

namespace BlazorDragDropWinFormsApp1
{
    public partial class Form1 : Form
    {
        private DragService? _dragService;
        private readonly BlazorWebView _wv;

        public Form1()
        {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.None;

            var mainfilePath =
                Path.Combine(
                    Path.GetDirectoryName(typeof(Form1).Assembly.Location),
                    "mainfile.html");

            var services = new ServiceCollection();
            services.AddWindowsFormsBlazorWebView();
            services.AddSingleton<DragService>();

            _wv = new BlazorWebView()
            {
                Dock = DockStyle.Fill,
                HostPage = "mainfile.html",
                //Source = new Uri(mainfilePath),
            };
            _wv.RootComponents.Add<MainPage>("#app");
            _wv.Services = services.BuildServiceProvider();

            Controls.Add(_wv);

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
