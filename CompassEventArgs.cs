using System;
using System.Windows.Media.Media3D;

namespace MagnetField
{
    class CompassEventArgs : EventArgs
    {
        public byte Sensor;
        public Vector3D Data;

        public CompassEventArgs(byte sensor, Vector3D data)
        {
            Sensor = sensor;
            Data = data;
        }
    }
}
