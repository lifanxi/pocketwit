using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;

namespace FingerUI
{
    public enum GestureTypes
    {
        Unrecognized, RightToLeft, LeftToRight, TopToBottom, BottomToTop, Clockwise, CounterClockwise, X, S, Z, DoubleTap
    }

    internal class GestureRecognizer
    {
        [DllImport("coredll.dll", EntryPoint = "MessageBeep", SetLastError = true)]
        private static extern void MessageBeep(int type);

        public delegate void delGestureMade(GestureTypes GestureMade);
        public event delGestureMade GestureMade;

        #region Gesture areas
        private Point[,] Areas = new Point[3, 3];
        private double AreaRadius = 0;
        private Dictionary<List<Point>, GestureTypes> DefinedGestures = new Dictionary<List<Point>, GestureTypes>();
        private List<Point> CurrentGesture = new List<Point>();
        #endregion

        private long LastTap = DateTime.Now.Ticks;

        private Control _ParentControl;
        public Control ParentControl
        {
            get { return _ParentControl; }
            set
            {
                UnHookEvents();
                _ParentControl = value;
                HookEvents();
            }

        }

        ~GestureRecognizer()
        {
            UnHookEvents();
        }

        #region Mouse events
        private void HookEvents()
        {
            _ParentControl.MouseDown += new MouseEventHandler(Parent_MouseDown);
            _ParentControl.MouseUp += new MouseEventHandler(Parent_MouseUp);
            _ParentControl.MouseMove += new MouseEventHandler(Parent_MouseMove);
            _ParentControl.Resize += new EventHandler(Parent_Resize);
        }

        private void UnHookEvents()
        {
            if (_ParentControl != null)
            {
                _ParentControl.MouseDown -= new MouseEventHandler(Parent_MouseDown);
                _ParentControl.MouseUp -= new MouseEventHandler(Parent_MouseUp);
                _ParentControl.MouseMove -= new MouseEventHandler(Parent_MouseMove);
                _ParentControl.Resize -= new EventHandler(Parent_Resize);
            }
        }

        void Parent_Resize(object sender, EventArgs e)
        {
            DetermineGesturePoints();
        }
        void Parent_MouseMove(object sender, MouseEventArgs e)
        {
            
            Point CurrentPoint = new Point(e.X, e.Y);
            Point CurrentArea = FindAreaforPoint(CurrentPoint);
            //If we're not in an area, it'll return -1 for coordinates
            if (CurrentArea.X < 0)
            {
                return;
            }
            //Always capture first area we hit
            if (CurrentGesture.Count == 0)
            {
                CurrentGesture.Add(CurrentArea);
                return;
            }
            //Check to see if we're still in the same area as last time
            Point LastArea = CurrentGesture[CurrentGesture.Count - 1];
            if (LastArea != CurrentArea)
            {
                CurrentGesture.Add(CurrentArea);
            }
        }

        
        void Parent_MouseUp(object sender, MouseEventArgs e)
        {
            GestureTypes Gesture = FindGesture();
            if (Gesture != GestureTypes.Unrecognized)
            {
                MessageBeep(0);
                if (GestureMade != null)
                {
                    GestureMade(Gesture);
                }
            }
            CurrentGesture.Clear();
        }
        void Parent_MouseDown(object sender, MouseEventArgs e)
        {
            CurrentGesture.Clear();
            long NextClick = DateTime.Now.Ticks;
            long TimeSinceLast = NextClick - LastTap;
            if(TimeSinceLast < 100)
            {
                GestureMade(GestureTypes.DoubleTap);
            }
            LastTap = DateTime.Now.Ticks;
        }
        #endregion

        
        
        private void DetermineGesturePoints()
        {
            int pWidth = _ParentControl.Width;
            int pHeight = _ParentControl.Height;
            
            //We create a matrix of 9 points spaced evenly over the parent control.
            int pXDistance = pWidth / 2;
            int pYDistance = pHeight / 2;
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    Areas[x, y] = new Point(pXDistance * x, pYDistance * y);
                }
            }

            // The area radius is the smallest distance between Gesture points
            AreaRadius = (pWidth > pHeight) ? pYDistance/2 : pXDistance/2;
            DefinedGestures = GestureDefiner.GetGestures(Areas);
        }

        private GestureTypes FindGesture()
        {
            GestureTypes RecognizedGesture = GestureTypes.Unrecognized;
            foreach (List<Point> Gesture in DefinedGestures.Keys)
            {
                if(GesturesAreEqual(Gesture.ToArray(),CurrentGesture.ToArray()))
                {
                    RecognizedGesture = DefinedGestures[Gesture];
                }
            }
            return RecognizedGesture;
        }

        private Point FindAreaforPoint(Point CurrentPoint)
        {
            foreach (Point Area in Areas)
            {
                if (PointInArea(CurrentPoint, Area))
                {
                    return Area;
                }
            }
            return new Point(-1, -1);
        }
        private bool PointInArea(Point CurrentPoint, Point Area)
        {
            int a = Math.Abs(CurrentPoint.X - Area.X);
            int b = Math.Abs(CurrentPoint.Y - Area.Y);
            double c = Math.Sqrt(a*a + b*b);
            
            return c < AreaRadius;
        }
        private static bool GesturesAreEqual(Point[] Gesture1, Point[] Gesture2)
        {
            if (Gesture1.Length != Gesture2.Length) { return false; }
            for (int i = 0; i < Gesture1.Length; i++)
            {
                if (Gesture1[i] != Gesture2[i]) { return false; }
            }
            return true;
        }
    }
}
