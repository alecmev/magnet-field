/*Line tmpLine = new Line();
tmpLine.StrokeThickness = 2;
tmpLine.Stroke = new SolidColorBrush(Colors.Black);
tmpLine.X1 = _boardOffset.X + 40;
tmpLine.Y1 = _boardOffset.Y + 40;
tmpLine.X2 = _boardOffset.X + 80;
tmpLine.Y2 = _boardOffset.Y + 80;
TheCanvas.Children.Add(tmpLine);*/

//TheScrollViewer.Width = _boardSize.X + SystemParameters.ScrollWidth;
//TheScrollViewer.Height = _boardSize.Y + SystemParameters.ScrollHeight;
//_boardOffset = new Point((TheCanvas.Width - _boardSize.X) / 2d, (TheCanvas.Height - _boardSize.Y) / 2d);

/*_sensors[0] = new Sensor(TheCanvas, new Point(_boardOffset.X + _boardSize.X - 40, _boardOffset.Y + _boardSize.Y - 40));
_sensors[1] = new Sensor(TheCanvas, new Point(_boardOffset.X + 40, _boardOffset.Y + _boardSize.Y - 40));
_sensors[2] = new Sensor(TheCanvas, new Point(_boardOffset.X + 40, _boardOffset.Y + 40));
_sensors[3] = new Sensor(TheCanvas, new Point(_boardOffset.X + _boardSize.X - 40, _boardOffset.Y + 40));

tmpImage = new BitmapImage(new Uri("pack://application:,,,/Magnet.png", UriKind.Absolute));
TheMagnet.Width = tmpImage.PixelWidth;
TheMagnet.Height = tmpImage.PixelHeight;
SolveMagnet(2, 1, 1, 1);*/

//tmpSensor = new Sensor(TheCanvas, new Point(_boardOffset.X + 40, _boardOffset.Y + _boardSize.Y - 40));
//tmpSensor.UOrientation = new System.Windows.Media.Media3D.Point3D(640, 1280, 192);

//tmpSensor = new Sensor(TheCanvas, new Point(_boardOffset.X + 40, _boardOffset.Y + _boardSize.Y - 40));
//tmpSensor.UOrientation = new System.Windows.Media.Media3D.Point3D(640, 1280, 192);

/*test.FontFamily = new System.Windows.Media.FontFamily("XITS");
test.FontSize = 24;
test.Text = "|x⃗⨯y⃗|";
test.Margin = new Thickness(0);
test.Padding = new Thickness(0);
test.LineHeight = 1;
test.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
test.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;*/

/*      
private static double P2(double number)
{
    return number * number;
}

private static double P4(double number)
{
    return number * number * number * number;
}
 */

//          4  4      2  2  4      4    4      4  4      2  4  2      4  2  2    4  4
//       - B  C  + 4 A  B  C  - 8 A  B C  + 4 A  C  + 2 A  B  C  + 4 A  B  C  - A  B 

//var E1 = -1 * (P4(B) * P4(C)) - (8 * P4(A) * B * P4(C)) - (P4(A) * P4(B));
//var E2 = (4 * P2(A) * P2(B) * P4(C)) + (4 * P4(A) * P4(C)) + (2 * P2(A) * P4(B) * P2(C)) + (4 * P4(A) * P2(B) * P2(C));

//A *= A;
//B *= B;
//C *= C;

/*var A2 = P2(A);
var B2 = P2(B);
var C2 = P2(C);

var A4 = P4(A);
var B4 = P4(B);
var C4 = P4(C);

var E1 = (-1 * A4 * B4) - (4 * A4 * C4) - (B4 * C4);
var E2 = (4 * A4 * B2 * C2) + (2 * A2 * B4 * C2) + (4 * A2 * B2 * C4);

var F1 = Math.Sqrt(E1 + E2);
var F2 = ((F1 + (A2 * B2) + (B2 * C2)) / ((A4 * B4) - (2 * A4 * B2 * C2) + (2 * A4 * C4) - (2 * A2 * B2 * C4) + (B4 * C4)));

var K = A * B * C * L * Math.Sqrt(F2);

var KA = K / A;
var KB = K / B;
var KC = K / C;

var X = L / 2 + P2(K) / (2 * L * B2) - P2(K) / (2 * L * C2);
var Y = L / 2 + P2(K) / (2 * L * B2) - P2(K) / (2 * L * A2);

K = KA + KB + KC + X + Y;*/

/*
(((L*B)/(2*K)+K/(2*L)-(K*B)/(2*(C^2)*L))^2)+(((L*B)/(2*K)+K/(2*L)-(K*B)/(2*(A^2)*L))^2)=1

"K = A * C * L * Math.Sqrt((Math.Sqrt(-1 * P4(B) * P4(C) + 4 * P2(A) * P2(B) * P4(C) - 8 * P4(A) * B * P4(C) + 4 * P4(A) * P4(C) + 2 * P2(A) * P4(B) * P2(C) + 4 * P4(A) * P2(B) * P2(C) - P4(A) * P4(B)) + (P2(B) * P2(C)) - (2 * P2(A) * B * P2(C)) + (2 * P2(A) * P2(C)) + (P2(A) * P2(B))) / (P2(B) * P4(C) - 2 * P2(A) * B * P4(C) + 2 * P4(A) * P4(C) - 2 * P4(A) * B * P2(C) + P4(A) * P2(B)));"

"K = A * C * L * Math.Sqrt("
    "(Math.Sqrt(-1 * B^4 * C^4 + 4 * A^2 * B^2 * C^4 - 8 * A^4 * B * C^4 + 4 * A^4 * C^4 + 2 * A^2 * B^4 * C^2 + 4 * A^4 * B^2 * C^2 - A^4 * B^4) + "
    "(B^2 * C^2) - "
    "(2 * A^2 * B * C^2) + "
    "(2 * A^2 * C^2) + "
    "(A^2 * B^2)) / "
    "B^2 * C^4 - 2 * A^2 * B * C^4 + 2 * A^4 * C^4 - 2 * A^4 * B * C^2 + A^4 * B^2);"



                    2  2      2    2      2  2    2  2           4  4      2  2  4      4    4      4  4      2  4  2      4  2  2    4  4
                   B  C  - 2 A  B C  + 2 A  C  + A  B  + sqrt(- B  C  + 4 A  B  C  - 8 A  B C  + 4 A  C  + 2 A  B  C  + 4 A  B  C  - A  B )
K = A C L sqrt(-------------------------------------------------------------------------------------------------------------------------------);
                    2  4      2    4      4  4      4    2    4  2
                   B  C  - 2 A  B C  + 2 A  C  - 2 A  B C  + A  B

16.289893617021276595744680851064
76

solve(((((L*B)/(2*K))+(K/(2*L))-((K*B)/(2*(C^2)*L)))^2)+((((L*B)/(2*K))+(K/(2*L))-((K*B)/(2*(A^2)*L)))^2)=1, K)
solve([((((L*B)/(2*K))+(K/(2*L))-((K*B)/(2*(C^2)*L))))=cos(E), ((((L*B)/(2*K))+(K/(2*L))-((K*B)/(2*(A^2)*L))))=sin(E)], [K, E])

*/

// <math xmlns="http://www.w3.org/1998/Math/MathML"><mrow><msqrt><mrow><mo>(</mo><mi>K</mi><mo>*</mo><mi>A</mi><mo>+</mo><mi>K</mi><mo>*</mo><mi>B</mi><mo>+</mo><mi>L</mi><mo>*</mo><mi>A</mi><mo>*</mo><mi>B</mi><mo>)</mo><mo>*</mo><mo>(</mo><mo>-</mo><mi>K</mi><mo>*</mo><mi>A</mi><mo>+</mo><mi>K</mi><mo>*</mo><mi>B</mi><mo>+</mo><mi>L</mi><mo>*</mo><mi>A</mi><mo>*</mo><mi>B</mi><mo>)</mo><mo>*</mo><mo>(</mo><mi>K</mi><mo>*</mo><mi>A</mi><mo>-</mo><mi>K</mi><mo>*</mo><mi>B</mi><mo>+</mo><mi>L</mi><mo>*</mo><mi>A</mi><mo>*</mo><mi>B</mi><mo>)</mo><mo>*</mo><mo>(</mo><mi>K</mi><mo>*</mo><mi>A</mi><mo>+</mo><mi>K</mi><mo>*</mo><mi>B</mi><mo>-</mo><mi>L</mi><mo>*</mo><mi>A</mi><mo>*</mo><mi>B</mi><mo>)</mo></mrow></msqrt><mo>/</mo><mo>(</mo><mo>(</mo><mn>2</mn><mo>*</mo><mi>A</mi><mo>*</mo><mi>B</mi><mo>)</mo><mo>^</mo><mn>2</mn><mo>)</mo></mrow></math>
// <math xmlns="http://www.w3.org/1998/Math/MathML"><mrow><msqrt><mrow><mo>(</mo><mi>K</mi><mo>*</mo><mi>C</mi><mo>+</mo><mi>K</mi><mo>*</mo><mi>B</mi><mo>+</mo><mi>L</mi><mo>*</mo><mi>C</mi><mo>*</mo><mi>B</mi><mo>)</mo><mo>*</mo><mo>(</mo><mo>-</mo><mi>K</mi><mo>*</mo><mi>C</mi><mo>+</mo><mi>K</mi><mo>*</mo><mi>B</mi><mo>+</mo><mi>L</mi><mo>*</mo><mi>C</mi><mo>*</mo><mi>B</mi><mo>)</mo><mo>*</mo><mo>(</mo><mi>K</mi><mo>*</mo><mi>C</mi><mo>-</mo><mi>K</mi><mo>*</mo><mi>B</mi><mo>+</mo><mi>L</mi><mo>*</mo><mi>C</mi><mo>*</mo><mi>B</mi><mo>)</mo><mo>*</mo><mo>(</mo><mi>K</mi><mo>*</mo><mi>C</mi><mo>+</mo><mi>K</mi><mo>*</mo><mi>B</mi><mo>-</mo><mi>L</mi><mo>*</mo><mi>C</mi><mo>*</mo><mi>B</mi><mo>)</mo></mrow></msqrt><mo>/</mo><mo>(</mo><mo>(</mo><mn>2</mn><mo>*</mo><mi>C</mi><mo>*</mo><mi>B</mi><mo>)</mo><mo>^</mo><mn>2</mn><mo>)</mo></mrow></math>
// <math xmlns="http://www.w3.org/1998/Math/MathML"><mrow><msqrt><mrow><mo>(</mo><mi>K</mi><mo>*</mo><mi>C</mi><mo>+</mo><mi>K</mi><mo>*</mo><mi>D</mi><mo>+</mo><mi>L</mi><mo>*</mo><mi>C</mi><mo>*</mo><mi>D</mi><mo>)</mo><mo>*</mo><mo>(</mo><mo>-</mo><mi>K</mi><mo>*</mo><mi>C</mi><mo>+</mo><mi>K</mi><mo>*</mo><mi>D</mi><mo>+</mo><mi>L</mi><mo>*</mo><mi>C</mi><mo>*</mo><mi>D</mi><mo>)</mo><mo>*</mo><mo>(</mo><mi>K</mi><mo>*</mo><mi>C</mi><mo>-</mo><mi>K</mi><mo>*</mo><mi>D</mi><mo>+</mo><mi>L</mi><mo>*</mo><mi>C</mi><mo>*</mo><mi>D</mi><mo>)</mo><mo>*</mo><mo>(</mo><mi>K</mi><mo>*</mo><mi>C</mi><mo>+</mo><mi>K</mi><mo>*</mo><mi>D</mi><mo>-</mo><mi>L</mi><mo>*</mo><mi>C</mi><mo>*</mo><mi>D</mi><mo>)</mo></mrow></msqrt><mo>/</mo><mo>(</mo><mo>(</mo><mn>2</mn><mo>*</mo><mi>C</mi><mo>*</mo><mi>D</mi><mo>)</mo><mo>^</mo><mn>2</mn><mo>)</mo></mrow></math>
// <math xmlns="http://www.w3.org/1998/Math/MathML"><mrow><msqrt><mrow><mo>(</mo><mi>K</mi><mo>*</mo><mi>A</mi><mo>+</mo><mi>K</mi><mo>*</mo><mi>D</mi><mo>+</mo><mi>L</mi><mo>*</mo><mi>A</mi><mo>*</mo><mi>D</mi><mo>)</mo><mo>*</mo><mo>(</mo><mo>-</mo><mi>K</mi><mo>*</mo><mi>A</mi><mo>+</mo><mi>K</mi><mo>*</mo><mi>D</mi><mo>+</mo><mi>L</mi><mo>*</mo><mi>A</mi><mo>*</mo><mi>D</mi><mo>)</mo><mo>*</mo><mo>(</mo><mi>K</mi><mo>*</mo><mi>A</mi><mo>-</mo><mi>K</mi><mo>*</mo><mi>D</mi><mo>+</mo><mi>L</mi><mo>*</mo><mi>A</mi><mo>*</mo><mi>D</mi><mo>)</mo><mo>*</mo><mo>(</mo><mi>K</mi><mo>*</mo><mi>A</mi><mo>+</mo><mi>K</mi><mo>*</mo><mi>D</mi><mo>-</mo><mi>L</mi><mo>*</mo><mi>A</mi><mo>*</mo><mi>D</mi><mo>)</mo></mrow></msqrt><mo>/</mo><mo>(</mo><mo>(</mo><mn>2</mn><mo>*</mo><mi>A</mi><mo>*</mo><mi>D</mi><mo>)</mo><mo>^</mo><mn>2</mn><mo>)</mo></mrow></math>
// <math xmlns="http://www.w3.org/1998/Math/MathML"><mrow><msqrt><mrow><mo>(</mo><mi>K</mi><mo>*</mo><mi>A</mi><mo>+</mo><mi>K</mi><mo>*</mo><mi>B</mi><mo>+</mo><mi>L</mi><mo>*</mo><mi>A</mi><mo>*</mo><mi>B</mi><mo>)</mo><mo>*</mo><mo>(</mo><mo>-</mo><mi>K</mi><mo>*</mo><mi>A</mi><mo>+</mo><mi>K</mi><mo>*</mo><mi>B</mi><mo>+</mo><mi>L</mi><mo>*</mo><mi>A</mi><mo>*</mo><mi>B</mi><mo>)</mo><mo>*</mo><mo>(</mo><mi>K</mi><mo>*</mo><mi>A</mi><mo>-</mo><mi>K</mi><mo>*</mo><mi>B</mi><mo>+</mo><mi>L</mi><mo>*</mo><mi>A</mi><mo>*</mo><mi>B</mi><mo>)</mo><mo>*</mo><mo>(</mo><mi>K</mi><mo>*</mo><mi>A</mi><mo>+</mo><mi>K</mi><mo>*</mo><mi>B</mi><mo>-</mo><mi>L</mi><mo>*</mo><mi>A</mi><mo>*</mo><mi>B</mi><mo>)</mo></mrow></msqrt><mo>/</mo><mo>(</mo><mo>(</mo><mn>2</mn><mo>*</mo><mi>A</mi><mo>*</mo><mi>B</mi><mo>)</mo><mo>^</mo><mn>2</mn><mo>)</mo><mo>+</mo><msqrt><mrow><mo>(</mo><mi>K</mi><mo>*</mo><mi>C</mi><mo>+</mo><mi>K</mi><mo>*</mo><mi>B</mi><mo>+</mo><mi>L</mi><mo>*</mo><mi>C</mi><mo>*</mo><mi>B</mi><mo>)</mo><mo>*</mo><mo>(</mo><mo>-</mo><mi>K</mi><mo>*</mo><mi>C</mi><mo>+</mo><mi>K</mi><mo>*</mo><mi>B</mi><mo>+</mo><mi>L</mi><mo>*</mo><mi>C</mi><mo>*</mo><mi>B</mi><mo>)</mo><mo>*</mo><mo>(</mo><mi>K</mi><mo>*</mo><mi>C</mi><mo>-</mo><mi>K</mi><mo>*</mo><mi>B</mi><mo>+</mo><mi>L</mi><mo>*</mo><mi>C</mi><mo>*</mo><mi>B</mi><mo>)</mo><mo>*</mo><mo>(</mo><mi>K</mi><mo>*</mo><mi>C</mi><mo>+</mo><mi>K</mi><mo>*</mo><mi>B</mi><mo>-</mo><mi>L</mi><mo>*</mo><mi>C</mi><mo>*</mo><mi>B</mi><mo>)</mo></mrow></msqrt><mo>/</mo><mo>(</mo><mo>(</mo><mn>2</mn><mo>*</mo><mi>C</mi><mo>*</mo><mi>B</mi><mo>)</mo><mo>^</mo><mn>2</mn><mo>)</mo></mrow></math>
// (L^2)/2=((((K*C+K*A+(2^(0.5))*L*C*A)*(-K*C+K*A+(2^(0.5))*L*C*A)*(K*C-K*A+(2^(0.5))*L*C*A)*(K*C+K*A-(2^(0.5))*L*C*A))^(0.5))/((2*C*A)^2))+((((K*A+K*B+L*A*B)*(-K*A+K*B+L*A*B)*(K*A-K*B+L*A*B)*(K*A+K*B-L*A*B))^(0.5))/((2*A*B)^2))+((((K*C+K*B+L*C*B)*(-K*C+K*B+L*C*B)*(K*C-K*B+L*C*B)*(K*C+K*B-L*C*B))^(0.5))/((2*C*B)^2))

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

/*private byte _afterTimeout;
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private long _milestone = 0;
        private long _calibrationMilestone = 0;*/

/*private readonly object _calibrationLock = new object();
private byte _calibrationPhase;
private Int64 _calibrationX, _calibrationY;
private Point _calibrationOffset;
private int _calibrationMeasurements;
private double[,] _calibrationData = new double[128, 4];
private int _calibrationDataOffset;

public Point CalibrationOffset
{
    get { return _calibrationOffset; }
}*/

/*if (!calibrationOffset.HasValue)
{
    //Calibrate();
    return;
}

_calibrationOffset = calibrationOffset.Value;*/
