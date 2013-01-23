using System;
using System.Windows.Media.Media3D;

namespace Compass
{
    class CompassEventArgs : EventArgs
    {
        public Point3D Data;

        public CompassEventArgs(Point3D data)
        {
            Data = data;
        }
    }
}
