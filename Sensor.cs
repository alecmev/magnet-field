using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using MagnetField.Properties;
using System.Windows.Threading;

namespace MagnetField
{
    public class Sensor
    {
        private static int _sensorNumber = 1;
        private Canvas _canvas;

        private Point _pLocation; // In pixels
        public Point PLocation { get { return _pLocation; } } // In pixels // Location of the sensor

        private Vector3D _pOrientation = new Vector3D(0, 0, 0); // In units
        public Vector3D UOrientation // In units // Sensor data
        {
            get { return _pOrientation; }
            set { _pOrientation = value; Update(); }
        }
        public double UX // In units
        {
            get { return _pOrientation.X; }
            set { UOrientation = new Vector3D(value, UY, UZ); }
        }
        public double UY // In units
        {
            get { return _pOrientation.Y; }
            set { UOrientation = new Vector3D(UX, value, UZ); }
        }
        public double UZ // In units
        {
            get { return _pOrientation.Z; }
            set { UOrientation = new Vector3D(UX, UY, value); }
        }
        public double ULengthXY { get { return Math.Sqrt(Math.Pow(UX, 2) + Math.Pow(UY, 2)); } } // In units

        private double MX { get { return UX / Settings.Default.UnitsPerMeter; } } // In meters
        private double MY { get { return UY / Settings.Default.UnitsPerMeter; } } // In meters
        private double MZ { get { return UZ / Settings.Default.UnitsPerMeter; } } // In meters
        private double MLengthXY { get { return Math.Sqrt(Math.Pow(MX, 2) + Math.Pow(MY, 2)); } } // In meters

        private double _pixelsPerUnit = Settings.Default.PixelsPerMeter / Settings.Default.UnitsPerMeter; // To save processing time
        public double PX { get { return UX * _pixelsPerUnit; } } // In pixels
        public double PY { get { return UY * _pixelsPerUnit * -1; } } // In pixels
        public double PZ { get { return UZ * _pixelsPerUnit; } } // In pixels
        public double PLengthXY { get { return Math.Sqrt(Math.Pow(PX, 2) + Math.Pow(PY, 2)); } } // In pixels
        public double PLengthXYZ { get { return Math.Sqrt(Math.Pow(PX, 2) + Math.Pow(PY, 2) + Math.Pow(PZ, 2)); } } // In pixels

        public double RAngleXY { get { return Math.Atan2(UY, UX) * (UY >= 0 ? 1 : -1); } } // In radians, (-PI; +PI]
        public double DAngleXY { get { return (RAngleXY / Math.PI) * 180d; } } // In degrees, (-180; +180]

        private Ellipse _zEllipse = new Ellipse();
        public double ZEllipseR { get { return _zEllipse.Width / 2d; } }
        private Line _laserLine = new Line();
        private Path _anglePath = new Path();
        private Line _xLine = new Line();
        private Line _yLine = new Line();
        private Line _pointerLine = new Line();

        private TextBlock _lengthText = new TextBlock();
        private TextBlock _angleText = new TextBlock();
        private TextBlock _xText = new TextBlock();
        private TextBlock _yText = new TextBlock();
        private TextBlock _zText = new TextBlock();

        public Sensor(Canvas canvas, Point pLocation)
        {
            _canvas = canvas;
            _pLocation = pLocation;

            _zEllipse.Style = (Style)App.Current.Resources["ZEllipseVector"];
            //_zEllipse.Style = (Style)App.Current.Resources["ZEllipsePositive"];
            Canvas.SetZIndex(_zEllipse, 1);
            _canvas.Children.Add(_zEllipse);

            _laserLine.X1 = (canvas.Width + canvas.Height) * -1;
            _laserLine.Y1 = PLocation.Y;
            _laserLine.X2 = _laserLine.X1 * -2;
            _laserLine.Y2 = PLocation.Y;
            _laserLine.Style = (Style)App.Current.Resources["LaserLine" + (_sensorNumber++)]; // WARNING
            Canvas.SetZIndex(_laserLine, 2);
            _canvas.Children.Add(_laserLine);

            _anglePath.Style = (Style)App.Current.Resources["AnglePath"];
            Canvas.SetZIndex(_anglePath, 3);
            _canvas.Children.Add(_anglePath);

            _xLine.Y1 = PLocation.Y;
            _xLine.Style = (Style)App.Current.Resources["SizeLine"];
            Canvas.SetZIndex(_xLine, 4);
            _canvas.Children.Add(_xLine);

            _yLine.X1 = PLocation.X;
            _yLine.Style = (Style)App.Current.Resources["SizeLine"];
            Canvas.SetZIndex(_yLine, 5);
            _canvas.Children.Add(_yLine);

            _pointerLine.X1 = PLocation.X;
            _pointerLine.Y1 = PLocation.Y;
            _pointerLine.Style = (Style)App.Current.Resources["PointerLine"];
            Canvas.SetZIndex(_pointerLine, 6);
            _canvas.Children.Add(_pointerLine);

            Border tmpBorder;

            _lengthText.Style = (Style)App.Current.Resources["InformationText"];
            tmpBorder = new Border();
            tmpBorder.Style = (Style)App.Current.Resources["InformationBorder"];
            tmpBorder.Child = _lengthText;
            Canvas.SetZIndex(tmpBorder, 7);
            _canvas.Children.Add(tmpBorder);

            _angleText.Style = (Style)App.Current.Resources["InformationText"];
            tmpBorder = new Border();
            tmpBorder.Style = (Style)App.Current.Resources["InformationBorder"];
            tmpBorder.Child = _angleText;
            Canvas.SetZIndex(tmpBorder, 7);
            _canvas.Children.Add(tmpBorder);

            _xText.Style = (Style)App.Current.Resources["InformationText"];
            tmpBorder = new Border();
            tmpBorder.Style = (Style)App.Current.Resources["InformationBorder"];
            tmpBorder.Child = _xText;
            Canvas.SetZIndex(tmpBorder, 7);
            _canvas.Children.Add(tmpBorder);

            _yText.Style = (Style)App.Current.Resources["InformationText"];
            tmpBorder = new Border();
            tmpBorder.Style = (Style)App.Current.Resources["InformationBorder"];
            tmpBorder.Child = _yText;
            Canvas.SetZIndex(tmpBorder, 7);
            _canvas.Children.Add(tmpBorder);

            _zText.Style = (Style)App.Current.Resources["InformationText"];
            tmpBorder = new Border();
            tmpBorder.Style = (Style)App.Current.Resources["InformationBorder"];
            tmpBorder.Child = _zText;
            Canvas.SetZIndex(tmpBorder, 7);
            _canvas.Children.Add(tmpBorder);
        }

        public double CalibrationRatio = 1;

        public void Update(bool settingsChanged = false)
        {
            if (settingsChanged)
                _pixelsPerUnit = Settings.Default.PixelsPerMeter / Settings.Default.UnitsPerMeter;

            _zEllipse.Width = (PLengthXYZ > 1 ? Math.Pow((CalibrationRatio / PLengthXYZ), 1d/3d) : 1);
            _zEllipse.Height = _zEllipse.Width;
            Canvas.SetLeft(_zEllipse, PLocation.X - (_zEllipse.Width / 2));
            Canvas.SetTop(_zEllipse, PLocation.Y - (_zEllipse.Height / 2));

            /*_zEllipse.Style = (Style)App.Current.Resources[(PZ >= 0 ? "ZEllipsePositive" : "ZEllipseNegative")];
            _zEllipse.Width = Math.Abs(PZ) * 2;
            _zEllipse.Height = _zEllipse.Width;
            Canvas.SetLeft(_zEllipse, PLocation.X - Math.Abs(PZ));
            Canvas.SetTop(_zEllipse, PLocation.Y - Math.Abs(PZ));*/

            _laserLine.RenderTransform = new RotateTransform((PY >= 0 ? 1 : -1) * (DAngleXY + 90), PLocation.X, PLocation.Y);

            _anglePath.Data = Geometry.Parse("M" + (PLocation.X + (PLengthXY * (PX >= 0 ? 1 : -1))).ToString("G") + "," + PLocation.Y.ToString("G") + " A" + PLengthXY.ToString("G") + "," + PLengthXY.ToString("G") + " 0 0 " + (PY >= 0 ? (PX >= 0 ? 1 : 0) : (PX >= 0 ? 0 : 1)) + " " + (PLocation.X + PX).ToString("G") + "," + (PLocation.Y + PY).ToString("G"));

            _xLine.X1 = PLocation.X + PX;
            _xLine.X2 = _xLine.X1;
            _xLine.Y2 = PLocation.Y + PY;

            _yLine.Y1 = _xLine.Y2;
            _yLine.X2 = _xLine.X2;
            _yLine.Y2 = _xLine.Y2;

            _pointerLine.X2 = _xLine.X2;
            _pointerLine.Y2 = _xLine.Y2;

            Border tmpBorder;
            Size tmpSize = new Size(double.PositiveInfinity, double.PositiveInfinity);

            _lengthText.Text = ULengthXY.ToString("F0");
            tmpBorder = (Border)_lengthText.Parent;
            tmpBorder.Measure(tmpSize);
            if (PX >= 0) Canvas.SetLeft(tmpBorder, PLocation.X + PX - (tmpBorder.DesiredSize.Width / 2d));
            else Canvas.SetRight(tmpBorder, (_canvas.Width - PLocation.X) - PX - (tmpBorder.DesiredSize.Width / 2d));
            if (PY >= 0) Canvas.SetTop(tmpBorder, PLocation.Y + PY + (Settings.Default.InformationOffset * 2d));
            else Canvas.SetBottom(tmpBorder, (_canvas.Height - PLocation.Y) - PY + (Settings.Default.InformationOffset * 2d));

            _angleText.Text = (DAngleXY > 90 ? 180d - DAngleXY : DAngleXY).ToString("F0") + "°"; // NEW CHANGE
            tmpBorder = (Border)_angleText.Parent;
            tmpBorder.Measure(tmpSize);
            if (PX >= 0) Canvas.SetLeft(tmpBorder, PLocation.X + PLengthXY - Settings.Default.InformationOffset);
            else Canvas.SetRight(tmpBorder, (_canvas.Width - PLocation.X) + PLengthXY - Settings.Default.InformationOffset);
            if (PY >= 0) Canvas.SetBottom(tmpBorder, (_canvas.Height - PLocation.Y) + Settings.Default.InformationOffset);
            else Canvas.SetTop(tmpBorder, PLocation.Y + Settings.Default.InformationOffset);

            _xText.Text = UX.ToString("F0");
            tmpBorder = (Border)_xText.Parent;
            tmpBorder.Measure(tmpSize);
            if (PX >= 0) Canvas.SetRight(tmpBorder, (_canvas.Width - PLocation.X) - PX - Settings.Default.InformationOffset);
            else Canvas.SetLeft(tmpBorder, PLocation.X + PX - Settings.Default.InformationOffset);
            if (PY >= 0) Canvas.SetBottom(tmpBorder, (_canvas.Height - PLocation.Y) + Settings.Default.InformationOffset);
            else Canvas.SetTop(tmpBorder, PLocation.Y + Settings.Default.InformationOffset);

            _yText.Text = UY.ToString("F0");
            tmpBorder = (Border)_yText.Parent;
            tmpBorder.Measure(tmpSize);
            if (PX >= 0) Canvas.SetRight(tmpBorder, (_canvas.Width - PLocation.X) + Settings.Default.InformationOffset);
            else Canvas.SetLeft(tmpBorder, PLocation.X + Settings.Default.InformationOffset);
            if (PY >= 0) Canvas.SetBottom(tmpBorder, (_canvas.Height - PLocation.Y) - PY - Settings.Default.InformationOffset);
            else Canvas.SetTop(tmpBorder, PLocation.Y + PY - Settings.Default.InformationOffset);
            
            _zText.Text = UZ.ToString("F0");
            tmpBorder = (Border)_zText.Parent;
            tmpBorder.Measure(tmpSize);
            if (PX >= 0) Canvas.SetLeft(tmpBorder, PLocation.X + Settings.Default.InformationOffset);
            else Canvas.SetRight(tmpBorder, (_canvas.Width - PLocation.X) + Settings.Default.InformationOffset);
            if (PY >= 0) Canvas.SetTop(tmpBorder, PLocation.Y + Settings.Default.InformationOffset);
            else Canvas.SetBottom(tmpBorder, (_canvas.Height - PLocation.Y) + Settings.Default.InformationOffset);
        }

        public void Calibrate()
        {
            CalibrationRatio = Math.Pow(640, 3) * PLengthXYZ;
            Update();
        }
    }
}
