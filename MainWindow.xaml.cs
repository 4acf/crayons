using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace crayons
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {

        private bool _isDrawing;
        private int _currentColor;
        private bool _currentUtensil; //true = pencil, false = eraser
        private Point _mousePosition = new();

        private struct ColorPreset
        {
            public string Hex { get; set; }
            public string ImageUri { get; set; }
        }

        private List<Line> _lines = new();
        private List<ColorPreset> _colorPresets;
        private Dictionary<string, ImageSource> _hoverImages;
        private Dictionary<string, ImageSource> _clickImages;

        public MainWindow()
        {
            InitializeComponent();
            _isDrawing = false;
            _currentColor = 0;
            _currentUtensil = true;
            _mousePosition.X = -1;
            _mousePosition.Y = -1;

            _colorPresets = new List<ColorPreset>
            {
                new ColorPreset { Hex = "#000000", ImageUri = "/res/color_000000.png" },
                new ColorPreset { Hex = "#f03535", ImageUri = "/res/color_f03535.png" },
                new ColorPreset { Hex = "#fabd2f", ImageUri = "/res/color_fabd2f.png" },
                new ColorPreset { Hex = "#7ecf61", ImageUri = "/res/color_7ecf61.png" },
                new ColorPreset { Hex = "#296be4", ImageUri = "/res/color_296be4.png" },
                new ColorPreset { Hex = "#b14ddf", ImageUri = "/res/color_b14ddf.png" },
                new ColorPreset { Hex = "#fdfdfd", ImageUri = "/res/color_fdfdfd.png" }
            };

            _hoverImages = new Dictionary<string, ImageSource>
            {
                {"pack://application:,,,/res/pencil.png",       new BitmapImage (new Uri(@"/res/pencil_hover.png", UriKind.Relative))},
                {"pack://application:,,,/res/pencil_hover.png", new BitmapImage (new Uri(@"/res/pencil.png",       UriKind.Relative))},
                {"pack://application:,,,/res/eraser.png",       new BitmapImage (new Uri(@"/res/eraser_hover.png", UriKind.Relative))},
                {"pack://application:,,,/res/eraser_hover.png", new BitmapImage (new Uri(@"/res/eraser.png",       UriKind.Relative))},
                {"pack://application:,,,/res/trash.png",        new BitmapImage (new Uri(@"/res/trash_hover.png",  UriKind.Relative))},
                {"pack://application:,,,/res/trash_hover.png",  new BitmapImage (new Uri(@"/res/trash.png",        UriKind.Relative))},
            };

            _clickImages = new Dictionary<string, ImageSource>
            {
                {"pack://application:,,,/res/pencil_hover.png", new BitmapImage (new Uri(@"/res/eraser_hover.png", UriKind.Relative))},
                {"pack://application:,,,/res/eraser_hover.png", new BitmapImage (new Uri(@"/res/pencil_hover.png", UriKind.Relative))},
            };

            var bounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            canvas.Width = bounds.Width;
            canvas.Height = bounds.Height;

        }

    private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDrawing = true;
            _mousePosition = e.GetPosition(this);

            if (_currentUtensil)
            {
                //draw dot
                Line line = new Line();
                Color brushColor = (Color)ColorConverter.ConvertFromString(_colorPresets[_currentColor].Hex);
                line.Stroke = new SolidColorBrush(brushColor);
                line.StrokeThickness = 5;
                line.StrokeDashCap = PenLineCap.Round;
                line.StrokeStartLineCap = PenLineCap.Round;
                line.StrokeEndLineCap = PenLineCap.Round;
                line.X1 = _mousePosition.X;
                line.Y1 = _mousePosition.Y;
                line.X2 = _mousePosition.X;
                line.Y2 = _mousePosition.Y;
                _lines.Add(line);
                canvas.Children.Add(line);
            }
            else
            {

                Canvas.SetLeft(eraserEllipse, _mousePosition.X - 50);
                Canvas.SetTop(eraserEllipse, _mousePosition.Y - 50);

                foreach (var line in _lines)
                {
                    if (Math.Abs(line.X1 - _mousePosition.X) < 50 && Math.Abs(line.Y1 - _mousePosition.Y) < 50)
                        canvas.Children.Remove(line);
                }
            }

        }

        private void canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDrawing = false;
            _mousePosition.X = -1;
            _mousePosition.Y = -1;

            if (!_currentUtensil)
            {
                Canvas.SetLeft(eraserEllipse, -100);
                Canvas.SetTop(eraserEllipse, 0);
            }

        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDrawing && _mousePosition.X != -1 && _mousePosition.Y != -1)
            {
                if (_currentUtensil)
                {
                    Line line = new Line();
                    Color brushColor = (Color)ColorConverter.ConvertFromString(_colorPresets[_currentColor].Hex);
                    line.Stroke = new SolidColorBrush(brushColor);
                    line.StrokeThickness = 5;
                    line.StrokeDashCap = PenLineCap.Round;
                    line.StrokeStartLineCap = PenLineCap.Round;
                    line.StrokeEndLineCap = PenLineCap.Round;
                    line.X1 = _mousePosition.X;
                    line.Y1 = _mousePosition.Y;
                    line.X2 = e.GetPosition(this).X;
                    line.Y2 = e.GetPosition(this).Y;

                    _lines.Add(line);
                    canvas.Children.Add(line);
                }
                else
                {
                    Canvas.SetLeft(eraserEllipse, _mousePosition.X - 50);
                    Canvas.SetTop(eraserEllipse, _mousePosition.Y - 50);

                    foreach (var line in _lines)
                    {
                        if (Math.Abs(line.X1 - _mousePosition.X) < 50 && Math.Abs(line.Y1 - _mousePosition.Y) < 50)
                            canvas.Children.Remove(line);
                    }                    
                }

                _mousePosition = e.GetPosition(this);

            }
            else
            {
                Canvas.SetLeft(eraserEllipse, -100);
                Canvas.SetTop(eraserEllipse, 0);
            }

        }

        private void buttonUtensil_MouseEnter(object sender, MouseEventArgs e)
        {
            var img = buttonUtensil.FindName("imageUtensil") as Image;
            if (img != null)
            {
                var currentSource = img.Source.ToString();
                img.Source = _hoverImages[currentSource];
            }
        }

        private void buttonUtensil_MouseLeave(object sender, MouseEventArgs e)
        {
            var img = buttonUtensil.FindName("imageUtensil") as Image;
            if (img != null)
            {
                var currentSource = img.Source.ToString();
                img.Source = _hoverImages[currentSource];
            }
        }

        private void buttonClear_MouseEnter(object sender, MouseEventArgs e)
        {
            var img = buttonClear.FindName("imageClear") as Image;
            if (img != null)
            {
                var currentSource = img.Source.ToString();
                img.Source = _hoverImages[currentSource];
            }
        }

        private void buttonClear_MouseLeave(object sender, MouseEventArgs e)
        {
            var img = buttonClear.FindName("imageClear") as Image;
            if (img != null)
            {
                var currentSource = img.Source.ToString();
                img.Source = _hoverImages[currentSource];
            }
        }

        private void buttonColor_Click(object sender, RoutedEventArgs e)
        {

            _currentColor = (_currentColor + 1) % _colorPresets.Count;
            var img = buttonColor.FindName("imageColor") as Image;
            if(img != null)
            {
                img.Source = new BitmapImage(new Uri(_colorPresets[_currentColor].ImageUri, UriKind.Relative));
            }
        }

        private void buttonUtensil_Click(object sender, RoutedEventArgs e)
        {
            _currentUtensil = !_currentUtensil;

            var img = buttonColor.FindName("imageUtensil") as Image;
            if (img != null)
            {
                var currentSource = img.Source.ToString();
                img.Source = _clickImages[currentSource];
            }
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            foreach(var line in _lines)
            {
                canvas.Children.Remove(line);
            }
        }
    }
}
