using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Hardcodet.Wpf.TaskbarNotification;
using NonInvasiveKeyboardHookLibrary;

namespace crayons
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private TaskbarIcon _notifyIcon;
        private KeyboardHookManager _keyboardHookManager = new();
        private MainWindow _mainWindow = new();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");

            //capture and override global scroll lock keypresses
            _keyboardHookManager.Start();

            _keyboardHookManager.RegisterHotkey(0x91, () =>
            {
                ToggleDrawMode();
            });

        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon.Dispose();
            base.OnExit(e);
        }

        private void ToggleDrawMode()
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                if (_mainWindow.IsVisible) 
                {
                    _mainWindow.Visibility = Visibility.Hidden;
                }
                else
                {

                    //set background to be a capture of the screen
                    Canvas canvas = _mainWindow.FindName("canvas") as Canvas;

                    var bounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
                    var bitmap = new System.Drawing.Bitmap(bounds.Width, bounds.Height);

                    var graphics = System.Drawing.Graphics.FromImage(bitmap);
                    graphics.CopyFromScreen(bounds.Location, System.Drawing.Point.Empty, bounds.Size);

                    //bitmap to bitmapimage implementation courtesy of https://stackoverflow.com/questions/6484357/converting-bitmapimage-to-bitmap-and-vice-versa
                    using (var memoryStream = new MemoryStream())
                    {
                        bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                        memoryStream.Position = 0;

                        var bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = memoryStream;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                        bitmapImage.Freeze();

                        canvas.Background = new ImageBrush(bitmapImage);
                    }

                    _mainWindow.Visibility = Visibility.Visible;
                    
                }

            });
            
        }

 
    }

}
