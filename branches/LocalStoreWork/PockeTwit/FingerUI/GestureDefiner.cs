using System;

using System.Drawing;
using System.Collections.Generic;
using System.Text;

namespace FingerUI
{
    internal static class GestureDefiner
    {
        internal static Dictionary<List<Point>, GestureTypes> GetGestures(Point[,] Areas)
        {
            Dictionary<List<Point>, GestureTypes> DefinedGestures = new Dictionary<List<Point>, GestureTypes>();
            List<Point> ThisGesture = new List<Point>();
            ThisGesture.Add(Areas[0, 1]);
            ThisGesture.Add(Areas[1, 1]);
            ThisGesture.Add(Areas[2, 1]);
            DefinedGestures.Add(ThisGesture, GestureTypes.LeftToRight);

            ThisGesture = new List<Point>();
            ThisGesture.Add(Areas[2, 1]);
            ThisGesture.Add(Areas[1, 1]);
            ThisGesture.Add(Areas[0, 1]);
            DefinedGestures.Add(ThisGesture, GestureTypes.RightToLeft);

            ThisGesture = new List<Point>();
            ThisGesture.Add(Areas[1, 0]);
            ThisGesture.Add(Areas[1, 1]);
            ThisGesture.Add(Areas[1, 2]);
            DefinedGestures.Add(ThisGesture, GestureTypes.TopToBottom);

            ThisGesture = new List<Point>();
            ThisGesture.Add(Areas[1, 2]);
            ThisGesture.Add(Areas[1, 1]);
            ThisGesture.Add(Areas[1, 0]);
            DefinedGestures.Add(ThisGesture, GestureTypes.BottomToTop);

            ThisGesture = new List<Point>();
            ThisGesture.Add(Areas[0, 0]);
            ThisGesture.Add(Areas[1, 0]);
            ThisGesture.Add(Areas[2, 0]);
            ThisGesture.Add(Areas[2, 1]);
            ThisGesture.Add(Areas[2, 2]);
            ThisGesture.Add(Areas[1, 2]);
            ThisGesture.Add(Areas[0, 2]);
            DefinedGestures.Add(ThisGesture, GestureTypes.Clockwise);

            ThisGesture = new List<Point>();
            ThisGesture.Add(Areas[2, 0]);
            ThisGesture.Add(Areas[1, 0]);
            ThisGesture.Add(Areas[0, 0]);
            ThisGesture.Add(Areas[0, 1]);
            ThisGesture.Add(Areas[0, 2]);
            ThisGesture.Add(Areas[1, 2]);
            ThisGesture.Add(Areas[2, 2]);
            DefinedGestures.Add(ThisGesture, GestureTypes.CounterClockwise);

            ThisGesture = new List<Point>();
            ThisGesture.Add(Areas[0, 0]);
            ThisGesture.Add(Areas[1, 1]);
            ThisGesture.Add(Areas[2, 2]);
            ThisGesture.Add(Areas[2, 1]);
            ThisGesture.Add(Areas[2, 0]);
            ThisGesture.Add(Areas[1, 1]);
            ThisGesture.Add(Areas[0, 2]);
            DefinedGestures.Add(ThisGesture, GestureTypes.X);
            return DefinedGestures;
        }
    }
    
}
