using System;
using System.IO.Ports;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Threading;
using Compass.Properties;

namespace Compass
{
    public partial class MainWindow : Window
    {
        private delegate void Point3DDelegate(Point3D data);
        private delegate void CalibrationPhaseDelegate(CalibrationPhase data, bool isFirst);

        private CompassWorker _compassWorker;

        public MainWindow()
        {
            InitializeComponent();

            object tmpRemembered = null;

            foreach (var tmpSerialPort in SerialPort.GetPortNames())
            {
                SerialPortComboBox.Items.Add(new ComboBoxItem { Content = tmpSerialPort });
                if (tmpSerialPort == Settings.Default.SerialPort)
                    tmpRemembered = SerialPortComboBox.Items[SerialPortComboBox.Items.Count - 1];
            }

            SerialPortComboBox.SelectionChanged += SerialPortComboBoxSelectionChanged;

            if (tmpRemembered != null)
                SerialPortComboBox.SelectedValue = tmpRemembered;
        }

        private void SerialPortComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_compassWorker != null)
                _compassWorker.Stop();

            if (SerialPortComboBox.SelectedIndex == 0)
                return;

            var tmpPortName = (string)((ComboBoxItem)SerialPortComboBox.SelectedItem).Content;
            Settings.Default.SerialPort = tmpPortName;
            Settings.Default.Save();

            TheCalibrateButton.IsEnabled = true;
            _compassWorker = new CompassWorker(tmpPortName, Settings.Default.IsCalibrated ? Settings.Default.CalibrationOffset : (Point?) null);
            _compassWorker.Changed += CompassWorkerChanged;
            _compassWorker.CalibrationPhaseChanged += CompassWorkerCalibrationPhaseChanged;
            UpdateCalibrationOffsetText();

            (new Thread(_compassWorker.Work)).Start();
        }

        private void CompassWorkerChanged(object sender, CompassEventArgs e)
        {
            Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Point3DDelegate(UpdateCoordinates),
                e.Data
                );
        }

        private void UpdateCoordinates(Point3D data)
        {
            data.X = -data.X;

            /*var tmpDot = new Ellipse()
            {
                Width = 4,
                Height = 4,
                Fill = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#1DACD6"))
            };
            Canvas.SetLeft(tmpDot, (data.X) * 320 / 500);
            Canvas.SetBottom(tmpDot, (data.Y) * 320 / 500);
            TheScatter.Children.Add(tmpDot);*/

            TheX.Text = "X = " + (Math.Sign(data.X) == 1 ? "+" : "") + data.X;
            TheY.Text = "Y = " + (Math.Sign(data.Y) == 1 ? "+" : "") + data.Y;
            TheZ.Text = "Z = " + (Math.Sign(data.Z) == 1 ? "+" : "") + data.Z;

            data.X += _compassWorker.CalibrationOffset.X;
            data.Y -= _compassWorker.CalibrationOffset.Y;

            TheXReal.Text = (Math.Sign(data.X) == 1 ? "+" : "") + data.X.ToString("F1");
            TheYReal.Text = (Math.Sign(data.Y) == 1 ? "+" : "") + data.Y.ToString("F1");

            var tmpRatio = 120d / Math.Sqrt(Math.Pow(data.X, 2) + Math.Pow(data.Y, 2));

            if (double.IsInfinity(tmpRatio))
                return;

            TheArrow.X2 = data.X * tmpRatio + 160;
            TheArrow.Y2 = data.Y * tmpRatio + 160;

            TheAngle.Text = " " + ((90 - (Math.Atan(-data.Y / (data.X == 0 ? 0.0001 : data.X)) / Math.PI) * 180) + ((Math.Sign(data.X) == -1) ? 180 : 0)).ToString("F0") + "°";
            Canvas.SetLeft(TheAngle, data.X * tmpRatio * (148d / 120d) + 160 - TheAngle.ActualWidth / 2);
            Canvas.SetTop(TheAngle, data.Y * tmpRatio * (148d / 120d) + 160 - TheAngle.ActualHeight / 2);
        }

        private void CompassWorkerCalibrationPhaseChanged(object sender, CalibrationEventArgs e)
        {
            Dispatcher.BeginInvoke(
                new CalibrationPhaseDelegate(UpdateCalibrationText),
                new object[] { e.Data, e.IsFirst }
                );
        }

        private void UpdateCalibrationText(CalibrationPhase data, bool isFirst)
        {
            switch (data)
            {
                case CalibrationPhase.Rotate:
                    TheCalibration.Text = isFirst ? "Place horizontally" : "Rotate 90° and wait";
                    break;
                case CalibrationPhase.Hold:
                    TheCalibration.Text = "Hold still!";
                    break;
                case CalibrationPhase.Calibrated:
                    Settings.Default.CalibrationOffset = _compassWorker.CalibrationOffset;
                    Settings.Default.IsCalibrated = true;
                    Settings.Default.Save();
                    UpdateCalibrationOffsetText();
                    TheCalibration.Text = "";
                    TheCalibrateButton.IsEnabled = true;
                    break;
            }
        }

        private void UpdateCalibrationOffsetText()
        {
            TheXOffset.Text = "[ " + (Math.Sign(_compassWorker.CalibrationOffset.X) == 1 ? "+" : "") + _compassWorker.CalibrationOffset.X.ToString("F1") + " ]";
            TheYOffset.Text = "[ " + (Math.Sign(-_compassWorker.CalibrationOffset.Y) == 1 ? "+" : "") + (-_compassWorker.CalibrationOffset.Y).ToString("F1") + " ]";
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_compassWorker != null)
                _compassWorker.Stop();
        }

        private void CalibrateClick(object sender, RoutedEventArgs e)
        {
            if (_compassWorker == null)
                return;

            TheCalibrateButton.IsEnabled = false;
            _compassWorker.Calibrate();
        }
    }
}
