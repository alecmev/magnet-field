using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using System.Windows;
using System.Windows.Media.Media3D;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Compass
{
    class CompassWorker
    {
        public event EventHandler<CompassEventArgs> Changed;
        public event EventHandler<CalibrationEventArgs> CalibrationPhaseChanged;

        private SerialPort _serialPort;
        private readonly byte[] _inputBuffer = new byte[256];
        private byte _afterTimeout;
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private long _milestone = 0;
        private long _calibrationMilestone = 0;

        private readonly object _calibrationLock = new object();
        private byte _calibrationPhase;
        private Int64 _calibrationX, _calibrationY;
        private Point _calibrationOffset;
        private int _calibrationMeasurements;
        private double[,] _calibrationData = new double[128, 4];
        private int _calibrationDataOffset;

        public Point CalibrationOffset
        {
            get { return _calibrationOffset; }
        }

        private volatile string _portName;
        private volatile bool _stop;

        public CompassWorker(string portName, Point? calibrationOffset)
        {
            _portName = portName;

            if (!calibrationOffset.HasValue)
            {
                //Calibrate();
                return;
            }

            _calibrationOffset = calibrationOffset.Value;
        }

        public void Work()
        {
            while (!_stop)
            {
                try
                {
                    _serialPort = new SerialPort
                    {
                        PortName = _portName,
                        BaudRate = 9600,
                        StopBits = StopBits.One,
                        Parity = Parity.None,
                        DataBits = 8,
                        DtrEnable = false,
                        ReadTimeout = 200
                    };
                    _serialPort.Open();

                    while (!_stop)
                    {
                        for (byte i = 1; i <= 4; ++i)
                        {
                            _serialPort.ReadExisting();
                            _serialPort.Write(new byte[] { (byte)(i + 48) }, 0, 1);

                            _inputBuffer[0] = (byte)_serialPort.ReadByte();
                            _inputBuffer[1] = (byte)_serialPort.ReadByte();
                            _inputBuffer[2] = (byte)_serialPort.ReadByte();
                            _inputBuffer[3] = (byte)_serialPort.ReadByte();
                            _inputBuffer[4] = (byte)_serialPort.ReadByte();
                            _inputBuffer[5] = (byte)_serialPort.ReadByte();

                            var tmpX = (Int16)((_inputBuffer[0] << 8) | _inputBuffer[1]);
                            var tmpY = (Int16)((_inputBuffer[2] << 8) | _inputBuffer[3]);
                            var tmpZ = (Int16)((_inputBuffer[4] << 8) | _inputBuffer[5]);



                            /*if (_afterTimeout == 6)
                            {
                                var tmpX = (Int16)((_inputBuffer[0] << 8) | _inputBuffer[1]);
                                var tmpY = (Int16)((_inputBuffer[2] << 8) | _inputBuffer[3]);
                                var tmpZ = (Int16)((_inputBuffer[4] << 8) | _inputBuffer[5]);

                                lock (_calibrationLock)
                                {
                                    if (_calibrationPhase == 0)
                                    {
                                        if (Changed != null)
                                            Changed(this, new CompassEventArgs(new Point3D(tmpX, tmpY, tmpZ)));
                                    }
                                    else
                                    {
                                        if (_calibrationPhase % 2 == 1)
                                        {
                                            ++_calibrationPhase;

                                            if (CalibrationPhaseChanged != null)
                                                CalibrationPhaseChanged(this, new CalibrationEventArgs(CalibrationPhase.Rotate, _calibrationPhase == 2));

                                            Thread.Sleep(100);

                                            if (CalibrationPhaseChanged != null)
                                                CalibrationPhaseChanged(this, new CalibrationEventArgs(CalibrationPhase.Hold, false));

                                            _calibrationMilestone = _stopwatch.ElapsedMilliseconds;
                                            continue;
                                        }

                                        if (_calibrationDataOffset < 128)
                                        {
                                            _calibrationData[_calibrationDataOffset, 0] = tmpX;
                                            _calibrationData[_calibrationDataOffset, 1] = tmpY;
                                            _calibrationData[_calibrationDataOffset, 2] = tmpZ / 10d;
                                            _calibrationData[_calibrationDataOffset, 3] = 1;
                                            ++_calibrationDataOffset;
                                        }

                                        _calibrationX += tmpX;
                                        _calibrationY += tmpY;
                                        ++_calibrationMeasurements;

                                        if (_stopwatch.ElapsedMilliseconds - _calibrationMilestone > 4000)
                                        {
                                            if (_calibrationPhase++ == 8)
                                            {
                                                _calibrationPhase = 0;

                                                var tmpMatrixA = (new DenseMatrix(_calibrationData)).Transpose();
                                                var tmpMatrixB = new DenseMatrix(_calibrationData);
                                                var tmpMatrixC = (new DenseMatrix(_calibrationData)).Transpose();

                                                var tmpSquares = new double[128, 1];
                                                for (var i = 0; i < 128; ++i)
                                                {
                                                    tmpSquares[i, 0] = Math.Pow(_calibrationData[i, 0], 2) +
                                                                       Math.Pow(_calibrationData[i, 1], 2) +
                                                                       Math.Pow(_calibrationData[i, 2], 2);
                                                }

                                                var tmpMatrixD = new DenseMatrix(tmpSquares);

                                                var tmpBeta = tmpMatrixA.Multiply(tmpMatrixB).Inverse().Multiply(tmpMatrixC).Multiply(tmpMatrixD);

                                                _calibrationOffset.X = tmpBeta[0, 0] / 2;
                                                _calibrationOffset.Y = tmpBeta[1, 0] / 2;

                                                //_calibrationOffset.X = (double) (_calibrationX) / _calibrationMeasurements;
                                                //_calibrationOffset.Y = (double) (_calibrationY) / _calibrationMeasurements;

                                                if (CalibrationPhaseChanged != null)
                                                    CalibrationPhaseChanged(this, new CalibrationEventArgs(CalibrationPhase.Calibrated, false));
                                            }
                                        }
                                    }
                                }
                            }

                            _afterTimeout = 0;*/
                        }
                    }
                }
                catch (Exception)
                {
                    if (_serialPort != null && _serialPort.IsOpen)
                        _serialPort.Close();
                }
            }

            if (_serialPort != null && _serialPort.IsOpen)
                _serialPort.Close();
        }

        public void Calibrate()
        {
            lock (_calibrationLock)
            {
                _calibrationPhase = 1;
                _calibrationX = 0;
                _calibrationY = 0;
                _calibrationMeasurements = 0;
                _calibrationDataOffset = 0;
            }
        }

        public void Stop()
        {
            _stop = true;
        }
    }
}

/*if (_serialPort.BytesToRead == 6 || _serialPort.BytesToRead == 0)
{
Thread.Sleep(10);
if (_serialPort.BytesToRead == 6 || _serialPort.BytesToRead == 0)
{
_serialPort.Read(_inputBuffer, 0, 6);
}
else
{
_serialPort.ReadExisting();
continue;
}
}
else
{
_serialPort.ReadExisting();
continue;
}

++test[_inputBuffer[0]];*/

/*_milestone = _stopwatch.ElapsedMilliseconds;
_serialPort.Read(_inputBuffer, 0, 6);
if (_stopwatch.ElapsedMilliseconds - _milestone > 100)
{
Thread.Sleep(100);
_serialPort.ReadExisting();
continue;
}*/

//var tmpX = (Int16)((_inputBuffer[0] << 8) | _inputBuffer[1]);
//var tmpY = (Int16)((_inputBuffer[2] << 8) | _inputBuffer[3]);

//var x = tmpX;
//var y = tmpY;

/*var x = (Int16)(((_inputBuffer[0] & 128) > 0) ? -((tmpX ^ 65535) + 1) : tmpX);
var y = (Int16)(((_inputBuffer[2] & 128) > 0) ? -((tmpY ^ 65535) + 1) : tmpY);*/

/*var tmpX = (_inputBuffer[5] << 8) | _inputBuffer[4];
var tmpY = (_inputBuffer[3] << 8) | _inputBuffer[2];

var x = ((_inputBuffer[5] & 128) > 0) ? (tmpX ^ 65535) + 1 : tmpX;
var y = ((_inputBuffer[3] & 128) > 0) ? (tmpY ^ 65535) + 1 : tmpY;*/

//var x = ((((_inputBuffer[1] & 128) > 0) ? (_inputBuffer[1] ^ 255) : _inputBuffer[1]) << 8) | _inputBuffer[0];
//var y = ((((_inputBuffer[3] & 128) > 0) ? (_inputBuffer[3] ^ 255) : _inputBuffer[3]) << 8) | _inputBuffer[2];

/*_inputBuffer[4] = _inputBuffer[0];
_inputBuffer[0] = _inputBuffer[1];
_inputBuffer[1] = _inputBuffer[4];

_inputBuffer[4] = _inputBuffer[2];
_inputBuffer[2] = _inputBuffer[3];
_inputBuffer[3] = _inputBuffer[4];

var x = BitConverter.ToInt16(_inputBuffer, 0);
var y = BitConverter.ToInt16(_inputBuffer, 2);*/

/*if (Changed != null)
//Changed(this, new CompassEventArgs(new Point(x, y)));
Changed(this, new CompassEventArgs(new Point(_inputBuffer[0], _inputBuffer[1])));*/
