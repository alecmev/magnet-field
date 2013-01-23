using System;

namespace Compass
{
    class CalibrationEventArgs : EventArgs
    {
        public CalibrationPhase Data;
        public bool IsFirst;

        public CalibrationEventArgs(CalibrationPhase data, bool isFirst)
        {
            Data = data;
            IsFirst = isFirst;
        }
    }

    enum CalibrationPhase
    {
        Calibrated,
        Rotate,
        Hold
    }
}
