﻿using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace FingerUI
{
    public partial class FullScreenTweet : UserControl
    {
        public PockeTwit.Library.status Status;
        private bool _Visible = false;
        public bool Visible
        {
            get
            {
                return _Visible;
            }
            set
            {
                _Visible = value;
                base.Visible = value;
                if (_Visible)
                {
                    PockeTwit.ThrottledArtGrabber.NewArtWasDownloaded += new PockeTwit.ThrottledArtGrabber.ArtIsReady(ThrottledArtGrabber_NewArtWasDownloaded);
                }
                else
                {
                    PockeTwit.ThrottledArtGrabber.NewArtWasDownloaded -= new PockeTwit.ThrottledArtGrabber.ArtIsReady(ThrottledArtGrabber_NewArtWasDownloaded);
                }
            }
        }
        public FullScreenTweet()
        {
            InitializeComponent();
            PockeTwit.Themes.FormColors.SetColors(this);
            avatarBox.Width = ClientSettings.SmallArtSize;
            avatarBox.Height = ClientSettings.SmallArtSize;
            
        }

        private delegate void delUpdateArt(string Argument);
        void ThrottledArtGrabber_NewArtWasDownloaded(string Argument)
        {
            
            if (Status != null)
            {
                if (Argument == Status.user.screen_name)
                {
                    if (InvokeRequired)
                    {
                        delUpdateArt d = new delUpdateArt(ThrottledArtGrabber_NewArtWasDownloaded);
                        this.Invoke(d, Argument);
                    }
                    else
                    {
                        avatarBox.Image = PockeTwit.ThrottledArtGrabber.GetArt(Status.user.screen_name, Status.user.high_profile_image_url);
                    }
                }
            }
        }
        public void Render()
        {
            if (Status != null)
            {
                avatarBox.Image = PockeTwit.ThrottledArtGrabber.GetArt(Status.user.screen_name, Status.user.high_profile_image_url);
                lblUserName.Text = Status.user.screen_name;
                lblTime.Text = Status.TimeStamp.ToString();
                //lblSource.Text = Status.source;
                string fullText;
                if (Yedda.ShortText.isShortTextURL(Status.text))
                {
                    string[] splitup = Status.text.Split(new char[] { ' ' });
                    fullText = Yedda.ShortText.getFullText(splitup[splitup.Length - 1]);
                }
                else
                {
                    fullText = Status.text;
                }
                lblText.Text = fullText;
            }
        }

        private void lnkDismiss_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }
    }
}