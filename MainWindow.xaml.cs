using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using System.Windows.Controls;
using MagnetField.Properties;

namespace MagnetField
{
    public partial class MainWindow : Window
    {
        private delegate void SensorChangedDelegate(byte sensor, Vector3D data);
        private delegate void CalibrationDelegate();

        private Size _boardImageSize;
        public static Size BoardSize = new Size(720, 720);
        private Point _boardOffset;
        //private Point _magnetLocation;

        private Sensor[] _sensors = new Sensor[5];
        private CompassWorker _worker;

        public MainWindow()
        {
            InitializeComponent();

            Canvas.SetZIndex(TheMagnet, 32);

            this.WindowState = System.Windows.WindowState.Maximized;

            var tmpImage = new BitmapImage(new Uri("pack://application:,,,/Board.png", UriKind.Absolute));
            _boardImageSize = new Size(tmpImage.PixelWidth, tmpImage.PixelHeight);
            _boardOffset = new Point((_boardImageSize.Width - BoardSize.Width) / 2d, (_boardImageSize.Height - BoardSize.Height) / 2d);
            TheCanvas.Width = _boardImageSize.Width;
            TheCanvas.Height = _boardImageSize.Height;

            _sensors[1] = new Sensor(TheCanvas, new Point(_boardOffset.X + BoardSize.Width - 40, _boardOffset.Y + BoardSize.Height - 40));
            _sensors[2] = new Sensor(TheCanvas, new Point(_boardOffset.X + 40, _boardOffset.Y + BoardSize.Height - 40));
            _sensors[3] = new Sensor(TheCanvas, new Point(_boardOffset.X + 40, _boardOffset.Y + 40));
            _sensors[4] = new Sensor(TheCanvas, new Point(_boardOffset.X + BoardSize.Width - 40, _boardOffset.Y + 40));

            _sensors[1].UOrientation = new Vector3D(-100, 200, 250);
            _sensors[2].UOrientation = new Vector3D(300, 400, -250);
            _sensors[3].UOrientation = new Vector3D(500, -600, 250);
            _sensors[4].UOrientation = new Vector3D(-700, -800, 250);

            var tmpTextBox = new TextBox();
            tmpTextBox.Opacity = 0.5;
            tmpTextBox.Width = 80;
            tmpTextBox.Height = 20;
            Canvas.SetLeft(tmpTextBox, _boardOffset.X + Settings.Default.InformationOffset);
            Canvas.SetTop(tmpTextBox, _boardOffset.Y + BoardSize.Height + Settings.Default.InformationOffset);
            Canvas.SetZIndex(tmpTextBox, 8);
            tmpTextBox.Text = Settings.Default.UnitsPerMeter.ToString();
            tmpTextBox.GotFocus += TextBoxGotFocus;
            tmpTextBox.LostFocus += TextBoxLostFocus;
            tmpTextBox.KeyDown += TextBoxKeyUp;
            TheCanvas.Children.Add(tmpTextBox);

            tmpTextBox = new TextBox();
            tmpTextBox.Width = 0;
            tmpTextBox.Height = 0;
            Canvas.SetLeft(tmpTextBox, _boardOffset.X);
            Canvas.SetTop(tmpTextBox, _boardOffset.Y + BoardSize.Height);
            TheCanvas.Children.Add(tmpTextBox);

            _worker = new CompassWorker();
            _worker.Changed += new EventHandler<CompassEventArgs>(SensorChanged);
            (new Thread(_worker.Work) { IsBackground = true }).Start();
        }

        private void TextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).Opacity = 1;
        }

        private void TextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            var tmpTextBox = (sender as TextBox);
            tmpTextBox.Opacity = 0.5;

            try
            {
                Settings.Default.UnitsPerMeter = double.Parse(tmpTextBox.Text);
                Settings.Default.Save();
                UpdateSensors();
            }
            catch (Exception)
            {
            }

            tmpTextBox.Text = Settings.Default.UnitsPerMeter.ToString();
        }

        private void TextBoxKeyUp(object sender, KeyEventArgs e)
        {
            var tmpTextBox = (sender as TextBox);
            var tmpDelta = 0d;

            switch (e.Key)
            {
                case Key.Enter:
                    tmpTextBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    return;

                case Key.OemPlus:
                    tmpDelta = 10d;
                    break;

                case Key.OemMinus:
                    tmpDelta = -10d;
                    break;

                default:
                    return;
            }

            e.Handled = true;

            try
            {
                tmpDelta += double.Parse(tmpTextBox.Text);

                if (tmpDelta < 10d)
                    tmpDelta = 10d;

                tmpTextBox.Text = tmpDelta.ToString();
            }
            catch (Exception)
            {
                tmpTextBox.Text = Settings.Default.UnitsPerMeter.ToString();
            }

            tmpTextBox.Select(tmpTextBox.Text.Length, 0);
        }

        private void SensorChanged(object sender, CompassEventArgs e)
        {
            Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new SensorChangedDelegate(SensorChangedSync),
                e.Sensor, e.Data);
        }

        private void SensorChangedSync(byte sensor, Vector3D data)
        {
            _sensors[sensor].UOrientation = data;

            double tmpX = 0, tmpY = 0;

            tmpX = _sensors[3].PLocation.X + (Math.Pow(_sensors[3].ZEllipseR, 2) - Math.Pow(_sensors[4].ZEllipseR, 2) + Math.Pow(640, 2)) / 1280d;
            tmpX += _sensors[2].PLocation.X + (Math.Pow(_sensors[2].ZEllipseR, 2) - Math.Pow(_sensors[1].ZEllipseR, 2) + Math.Pow(640, 2)) / 1280d;
            tmpX /= 2d;

            tmpY = _sensors[3].PLocation.Y + (Math.Pow(_sensors[3].ZEllipseR, 2) - Math.Pow(_sensors[2].ZEllipseR, 2) + Math.Pow(640, 2)) / 1280d;
            tmpY += _sensors[4].PLocation.Y + (Math.Pow(_sensors[4].ZEllipseR, 2) - Math.Pow(_sensors[1].ZEllipseR, 2) + Math.Pow(640, 2)) / 1280d;
            tmpY /= 2d;

            /*var tmpWorstR = _sensors[1].ZEllipseR;
            var tmpWorstSensor = 1;

            for (int i = 2; i <= 4; ++i)
            {
                if (_sensors[i].ZEllipseR < tmpWorstR)
                {
                    tmpWorstR = _sensors[i].ZEllipseR;
                    tmpWorstSensor = i;
                }
            }

            switch (tmpWorstSensor)
            {
                case 1:
                    tmpWorstSensor = 3;
                    break;
                case 2:
                    tmpWorstSensor = 4;
                    break;
                case 3:
                    tmpWorstSensor = 1;
                    break;
                case 4:
                    tmpWorstSensor = 2;
                    break;
            }

            switch (tmpWorstSensor)
            {
                case 1:
                    tmpX = _sensors[3].PLocation.X + (Math.Pow(_sensors[3].ZEllipseR, 2) - Math.Pow(_sensors[4].ZEllipseR, 2) + Math.Pow(640, 2)) / 1280d;
                    tmpY = _sensors[3].PLocation.Y + (Math.Pow(_sensors[3].ZEllipseR, 2) - Math.Pow(_sensors[2].ZEllipseR, 2) + Math.Pow(640, 2)) / 1280d;
                    break;
                case 2:
                    tmpX = _sensors[3].PLocation.X + (Math.Pow(_sensors[3].ZEllipseR, 2) - Math.Pow(_sensors[4].ZEllipseR, 2) + Math.Pow(640, 2)) / 1280d;
                    tmpY = _sensors[4].PLocation.Y + (Math.Pow(_sensors[4].ZEllipseR, 2) - Math.Pow(_sensors[1].ZEllipseR, 2) + Math.Pow(640, 2)) / 1280d;
                    break;
                case 3:
                    tmpX = _sensors[2].PLocation.X + (Math.Pow(_sensors[2].ZEllipseR, 2) - Math.Pow(_sensors[1].ZEllipseR, 2) + Math.Pow(640, 2)) / 1280d;
                    tmpY = _sensors[4].PLocation.Y + (Math.Pow(_sensors[4].ZEllipseR, 2) - Math.Pow(_sensors[1].ZEllipseR, 2) + Math.Pow(640, 2)) / 1280d;
                    break;
                case 4:
                    tmpX = _sensors[2].PLocation.X + (Math.Pow(_sensors[2].ZEllipseR, 2) - Math.Pow(_sensors[1].ZEllipseR, 2) + Math.Pow(640, 2)) / 1280d;
                    tmpY = _sensors[3].PLocation.Y + (Math.Pow(_sensors[3].ZEllipseR, 2) - Math.Pow(_sensors[2].ZEllipseR, 2) + Math.Pow(640, 2)) / 1280d;
                    break;
            }*/

            Canvas.SetLeft(TheMagnet, tmpX - (TheMagnet.ActualWidth / 2d));
            Canvas.SetTop(TheMagnet, tmpY - (TheMagnet.ActualHeight / 2d));
        }

        private void UpdateSensors()
        {
            for (int i = 1; i <= 4; ++i)
                _sensors[i].Update(true);
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_worker != null)
                _worker.Stop();
        }

        private void ScrollViewerMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
                ScrollToCenter();
        }

        private void ScrollViewerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScrollToCenter(e.NewSize);
        }

        private void ScrollToCenter(Size? newSize = null)
        {
            TheScrollViewer.ScrollToHorizontalOffset((_boardImageSize.Width - (newSize.HasValue ? newSize.Value.Width : TheScrollViewer.ViewportWidth)) / 2);
            TheScrollViewer.ScrollToVerticalOffset((_boardImageSize.Height - (newSize.HasValue ? newSize.Value.Height : TheScrollViewer.ViewportHeight)) / 2);
        }

        private void TheCanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                Dispatcher.BeginInvoke(
                    DispatcherPriority.Normal,
                    new CalibrationDelegate(Calibrate));
        }

        private void Calibrate()
        {
            var tmpPoint = Mouse.GetPosition(TheCanvas);
            var tmpHalf = TheCanvas.Width / 2;

            if (tmpPoint.X < tmpHalf)
            {
                if (tmpPoint.Y < tmpHalf)
                    _sensors[3].Calibrate();
                else
                    _sensors[2].Calibrate();
            }
            else
            {
                if (tmpPoint.Y < tmpHalf)
                    _sensors[4].Calibrate();
                else
                    _sensors[1].Calibrate();
            }
        }

        /*private void SolveMagnet(double A, double B, double C, double L)
        {
            A *= A;
            B *= B;
            C *= C;

            var R1 = (A != 0) ? 1 / A : double.MaxValue;
            var R2 = (B != 0) ? 1 / B : double.MaxValue;
            var R3 = (C != 0) ? 1 / C : double.MaxValue;

            var R12 = new Point(0, R2 + (L - R1 - R2) / 2);
            var R23 = new Point(R2 + (L - R3 - R2) / 2, 0);
            var sqrt2 = Math.Sqrt(2);
            var R31c = (R1 + (sqrt2 * L - R1 - R3) / 2) / sqrt2;
            var R31 = new Point(R31c, L - R31c);

            var result = new Point((R12.X + R23.X + R31.X) / 3, (R12.Y + R23.Y + R31.Y) / 3);

            //var X = R2 + ((L - R3 - R2) / 2);
            //var Y = R2 + ((L - R1 - R2) / 2);

            _magnetLocation = new Point(_boardOffset.X + 40 + result.X * Settings.Default.PixelsPerMeter, _boardOffset.Y + _boardImageSize.Height - 40 - result.Y * Settings.Default.PixelsPerMeter);
            UpdateMagnet();
        }

        */

        /*public static Point? DoLinesIntersect(Line L1, Line L2)
        {
            Point ptIntersection = new Point();

            // Denominator for ua and ub are the same, so store this calculation
            double d =
               (L2.Y2 - L2.Y1) * (L1.X2 - L1.X1)
               -
               (L2.X2 - L2.X1) * (L1.Y2 - L1.Y1);

            //n_a and n_b are calculated as seperate values for readability
            double n_a =
               (L2.X2 - L2.X1) * (L1.Y1 - L2.Y1)
               -
               (L2.Y2 - L2.Y1) * (L1.X1 - L2.X1);

            double n_b =
               (L1.X2 - L1.X1) * (L1.Y1 - L2.Y1)
               -
               (L1.Y2 - L1.Y1) * (L1.X1 - L2.X1);

            // Make sure there is not a division by zero - this also indicates that
            // the lines are parallel.  
            // If n_a and n_b were both equal to zero the lines would be on top of each 
            // other (coincidental).  This check is not done because it is not 
            // necessary for this implementation (the parallel check accounts for this).
            if (d == 0)
                return null;

            // Calculate the intermediate fractional point that the lines potentially intersect.
            double ua = n_a / d;
            double ub = n_b / d;

            // The fractional point will be between 0 and 1 inclusive if the lines
            // intersect.  If the fractional calculation is larger than 1 or smaller
            // than 0 the lines would need to be longer to intersect.
            if (ua >= 0d && ua <= 1d && ub >= 0d && ub <= 1d)
            {
                ptIntersection.X = L1.X1 + (ua * (L1.X2 - L1.X1));
                ptIntersection.Y = L1.Y1 + (ua * (L1.Y2 - L1.Y1));
                return ptIntersection;
            }
            return null;
        }*/
    }
}
