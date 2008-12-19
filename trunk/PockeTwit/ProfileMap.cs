using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TiledMaps;
using System.Reflection;

namespace PockeTwit
{
    public partial class ProfileMap : Form
    {
        private int _mapSize = 0;
        private int startCount = 0;
        private Bitmap markerImage;
        
        private List<Library.User> _Users = new List<Library.User>();
        public List<Library.User> Users
        {
            get { return _Users; }
            set
            {
                _Users = value;
                SetMarkers();
                RefreshBitmap();
            }
        }

        Bitmap myBitmap;
        GraphicsRenderer myRenderer = new GraphicsRenderer();
        GoogleMapSession mySession = new GoogleMapSession();
        

        public ProfileMap()
        {
            InitializeComponent();
            //marker = myRenderer.LoadBitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("PockeTwit.Marker.png"));
            PockeTwit.Themes.FormColors.SetColors(this);
            if (ClientSettings.IsMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            this.myPictureBox.Resize += new EventHandler(pictureBox1_Resize);
            if (DetectDevice.DeviceType == DeviceType.Professional)
            {
                myPictureBox.MouseDown += new MouseEventHandler(myPictureBox_MouseDown);
                myPictureBox.MouseMove += new MouseEventHandler(myPictureBox_MouseMove);
                myPictureBox.MouseUp += new MouseEventHandler(myPictureBox_MouseUp);
            }
            RefreshBitmap();
            Cursor.Current = Cursors.Default;
        }

        void myPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            Point p = new Point(e.X, e.Y);
            foreach (MapOverlay o in mySession.Overlays)
            {
                userMapDrawable marker = (userMapDrawable)o.Drawable;
                if (marker.Location.Contains(p))
                {
                    marker.IsOpened = !marker.IsOpened;
                    break;
                }
            }
            RefreshBitmap();
        }


        private void SetMarkers()
        {
            List<string> seenLocs = new List<string>();
            foreach (Library.User user in _Users)
            {
                string location = user.location;
                if (!seenLocs.Contains(location))
                {
                    seenLocs.Add(location);
                    Yedda.GoogleGeocoder.Coordinate c;
                    if (!Yedda.GoogleGeocoder.Coordinate.tryParse(location, out c))
                    {
                        c = Yedda.GoogleGeocoder.Geocode.GetCoordinates(location);
                    }
                    if (c.Latitude != 0 && c.Longitude != 0)
                    {
                        userMapDrawable marker = new userMapDrawable();
                        marker.userToDraw = user;
                        marker.charToUse = 'a';
                        MapOverlay o = new MapOverlay(marker, new Geocode((double)c.Latitude, (double)c.Longitude), new Point(0, -marker.Height / 2));
                        mySession.Overlays.Add(o);
                    }
                }
            }
        }

        Point myLastPos = Point.Empty;
        private void myPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            mySession.Pan(MousePosition.X - myLastPos.X, MousePosition.Y - myLastPos.Y);
            myLastPos = MousePosition;
            RefreshBitmap();
        }

        private void myPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            myLastPos = MousePosition;
        }

        void pictureBox1_Resize(object sender, EventArgs e)
        {
            RefreshBitmap();
        }

        void RefreshBitmap()
        {
            // clear out tiles that haven't been used in 10 seconds, just to keep from running out of memory.
            mySession.ClearAgedTiles(10000);

            if (myBitmap == null || myBitmap.Width != myPictureBox.ClientSize.Width || myBitmap.Height != myPictureBox.ClientSize.Height)
            {
                myBitmap = new Bitmap(myPictureBox.ClientSize.Width, myPictureBox.ClientSize.Height, System.Drawing.Imaging.PixelFormat.Format16bppRgb565);
                myRenderer.Graphics = Graphics.FromImage(myBitmap);
                myPictureBox.Image = myBitmap;
            }
            mySession.DrawMap(myRenderer, 0, 0, myBitmap.Width, myBitmap.Height, (o) =>
            {
                Invoke(new EventHandler((sender, args) =>
                {
                    RefreshBitmap();
                }));
            }, null);
            myPictureBox.Refresh();
        }

        
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == (Keys.LButton | Keys.MButton | Keys.Back))
            {
                if (mySession.CanZoomIn)
                {
                    mySession.ZoomIn();
                    RefreshBitmap();
                }
            }
            if (e.KeyCode == Keys.Right)
            {
                mySession.Pan(0 - ClientSettings.TextSize, 0);
                RefreshBitmap();
            }
            if (e.KeyCode == Keys.Left)
            {
                mySession.Pan(ClientSettings.TextSize, 0);
                RefreshBitmap();
            }
            if (e.KeyCode == Keys.Up)
            {
                mySession.Pan(0, ClientSettings.TextSize);
                RefreshBitmap();
            }
            if (e.KeyCode == Keys.Down)
            {
                mySession.Pan(0, 0 - ClientSettings.TextSize);
                RefreshBitmap();
            }
            
        }
        private void menuItem1_Click(object sender, EventArgs e)
        {
            this.myPictureBox.Resize -= new EventHandler(pictureBox1_Resize);
            if (this.myPictureBox.Image != null)
            {
                this.myPictureBox.Image.Dispose();
            }
            this.Close();
        }

        
        private void menuItem4_Click(object sender, EventArgs e)
        {
            if (mySession.CanZoomOut)
            {
                mySession.ZoomOut();
                RefreshBitmap();
            }
        }

        private void menuNext_Click(object sender, EventArgs e)
        {
            if (startCount < Users.Count)
            {
                startCount = startCount + 5;
                SetMarkers();
                RefreshBitmap();
            }
        }

        private void menuZoomIn_Click(object sender, EventArgs e)
        {
            if (mySession.CanZoomIn)
            {
                mySession.ZoomIn();
                RefreshBitmap();
            }
        }

        
    }
}