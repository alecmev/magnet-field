using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Collections.Generic;
using MagnetField.Properties;

namespace MagnetField
{
    class CompassWorker
    {
        public event EventHandler<CompassEventArgs> Changed;

        private SerialPort _serialPort;
        private byte[] _inputBuffer = new byte[6];
        private Queue<Vector3D>[] _filterBuffer = new Queue<Vector3D>[5]; // 5 is for convenience (first sensor is 1, not 0)
        private Vector3D[] _offset = new Vector3D[5]; // same as above

        private int _writeDelay = 10;
        private int _bulkWriteLength;
        private byte[] _bulkWrite;

        private volatile bool _useMagnet = true;
        private volatile bool _stop;

        public CompassWorker()
        {
            _bulkWriteLength = 4 + _writeDelay * 3;
            _bulkWrite = new byte[_bulkWriteLength];
            var tmpPeriod = _writeDelay + 1;

            for (int i = 0; i < 4; ++i)
                _bulkWrite[i * tmpPeriod] = (byte)(49 + i); // 49 == '1'
        }

        public void Work()
        {
            while (!_stop)
            {
                try
                {
                    foreach (var tmpPortName in SerialPort.GetPortNames())
                    {
                        _serialPort = new SerialPort
                        {
                            PortName = tmpPortName,
                            BaudRate = 9600,
                            StopBits = StopBits.One,
                            Parity = Parity.None,
                            DataBits = 8,
                            DtrEnable = false,
                            ReadTimeout = 200
                        };

                        _serialPort.Open();
                        _serialPort.ReadExisting();

                        Write();
                        Thread.Sleep(500);

                        if (_serialPort.BytesToRead != 24)
                            _serialPort.Close();
                    }

                    if (!_serialPort.IsOpen)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    Write(6);
                    _serialPort.ReadExisting();

                    for (byte i = 1; i <= 4; ++i)
                        _filterBuffer[i] = new Queue<Vector3D>();

                    for (int j = 0; j < Settings.Default.FilterLength; ++j)
                    {
                        Write();

                        for (byte i = 1; i <= 4; ++i)
                            _filterBuffer[i].Enqueue(Read());
                    }

                    for (byte i = 1; i <= 4; ++i)
                        _offset[i] = Filter(_filterBuffer[i]);

                    Write(5);

                    while (!_stop)
                    {
                        Write();

                        for (byte i = 1; i <= 4; ++i)
                            Changed(this, new CompassEventArgs(i, Filter(_filterBuffer[i]) - _offset[i]));

                        for (byte i = 1; i <= 4; ++i)
                        {
                            _filterBuffer[i].Dequeue();
                            _filterBuffer[i].Enqueue(Read());
                        }
                    }

                    Write(6);
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

        private void Write(byte sensor = 0)
        {
            if (sensor < 0 || sensor > 6)
                return;

            if (sensor > 0)
                _serialPort.Write(new byte[] { (byte)(48 + sensor) }, 0, 1);
            else
                _serialPort.Write(_bulkWrite, 0, _bulkWriteLength);
        }

        private Vector3D Read()
        {
            _inputBuffer[0] = (byte)_serialPort.ReadByte();
            _inputBuffer[1] = (byte)_serialPort.ReadByte();
            _inputBuffer[2] = (byte)_serialPort.ReadByte();
            _inputBuffer[3] = (byte)_serialPort.ReadByte();
            _inputBuffer[4] = (byte)_serialPort.ReadByte();
            _inputBuffer[5] = (byte)_serialPort.ReadByte();

            return new Vector3D(
                (Int16)((_inputBuffer[0] << 8) | _inputBuffer[1]),
                (Int16)((_inputBuffer[2] << 8) | _inputBuffer[3]),
                (Int16)((_inputBuffer[4] << 8) | _inputBuffer[5]));
        }

        private Vector3D Filter(Queue<Vector3D> data)
        {
            List<double> dataX = new List<double>();
            List<double> dataY = new List<double>();
            List<double> dataZ = new List<double>();

            foreach (var tmpData in data)
            {
                dataX.Add(tmpData.X);
                dataY.Add(tmpData.Y);
                dataZ.Add(tmpData.Z);
            }

            dataX.Sort();
            dataY.Sort();
            dataZ.Sort();

            int sampleCount = dataX.Count - 2;
            double averageX = 0, averageY = 0, averageZ = 0;

            for (int i = 1; i <= sampleCount; ++i)
            {
                averageX += dataX[i];
                averageY += dataY[i];
                averageZ += dataZ[i];
            }

            return new Vector3D(averageX, averageY, averageZ) / sampleCount;
        }

        public void ToggleMagnet()
        {
            _useMagnet = !_useMagnet;
        }

        public void Stop()
        {
            _stop = true;
        }
    }
}
