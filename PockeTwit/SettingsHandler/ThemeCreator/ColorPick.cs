using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Collections;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace PockeTwit
{

    public partial class ColorPick : Form
    {
        private string filename;
        private string ThemeName;
        Bitmap m_bmp;
        ArrayList m_arraylist;
        public ColorPick(string Theme)
        {
            m_bmp = CreateColorPallet();
            m_arraylist = new ArrayList();
            filename = ClientSettings.AppPath + "\\Themes\\" + Theme + ".txt";
            ThemeName = Theme;
            LoadColorFile();
            InitializeComponent();
            
            //Graphics.FromImage(m_bmp).FillRectangle(
        }
        void LoadColorFile()
        {
            StreamReader Reader;
            try
            {
                Reader = new StreamReader(filename);
            }
            catch
            {
                StreamWriter writer = new StreamWriter(filename);
                writer.Write(
                    "BackColor:10:10:10\n" +
                    "ForeColor:211:211:211\n" +
                    "LinkColor:150:150:255\n" +
                    "SelectedBackColor:110:110:200\n" +
                    "SelectedForeColor:255:255:255\n" +
                    "SelectedSmallTextColor:200:200:200\n" +
                    "SmallTextColor:128:128:128\n" +
                    "ErrorColor:255:0:0\n" +
                    "FieldBackColor:255:255:255\n" +
                    "FieldForeColor:0:0:0\n");
                writer.Close();
                Reader = new StreamReader(filename);
            }
            
            string line = "";
            int pos = 0;
            while (true)
            {
                line = Reader.ReadLine();
                if (line == null)
                {
                    break;
                }
                string[] parsed = line.Split(new char[] { ':' });
                if (parsed.Length < 4)
                {
                    continue;
                }
                CreateColorItem(pos,parsed[0], parsed[1], parsed[2], parsed[3]);
                pos++;
            }
            Reader.Close();
        }
        void CreateColorItem(int pos,string strLabel, string strR, string strG, string strB)
        {
            if (strLabel.Length <= 2) return;
            Int16 R, G, B;
            try
            {
                R = Int16.Parse(strR);
                G = Int16.Parse(strG);
                B = Int16.Parse(strB);
            }
            catch
            {
                return;
            }
            Label lbl = new Label();
            lbl.Text = strLabel;
            lbl.Left = 10;
            lbl.Top = pos * 20 + 10;
            lbl.Width = 120;
            lbl.Height = 15;


            PictureBox pb = new PictureBox();
            pb.Left = 150;
            pb.Top = pos * 20 + 10;
            pb.Width = 20;
            pb.Height = 15;
            pb.BackColor = Color.FromArgb(R, G, B);
            pb.Click += new EventHandler(pb_Click);

            this.Controls.Add(lbl);
            this.Controls.Add(pb);
            ColorSetting cs=new ColorSetting(strLabel,pb);
            
            m_arraylist.Add(cs);

        }

        void pb_Click(object sender, EventArgs e)
        {
            ColorChose cc = new ColorChose(Color.Red, m_bmp);
            cc.ShowDialog();
            PictureBox pb = (PictureBox)sender;
            pb.BackColor = cc.MyColor;
        }
        public static Color GetColorFromPallet(int i, int j)
        {
            return Color.FromArgb(i, j, (i+j) % 255);
        }
        Bitmap CreateColorPallet()
        {
            Bitmap bmp = new Bitmap(255, 255);

            for (int i = 0; i < 255; i++)
            {
                for (int j = 0; j < 255; j++)
                {
                    bmp.SetPixel(i, j, ColorPick.GetColorFromPallet(i,j));
                }
            }
            return bmp;
        }
        
        
        private void menuAccept_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(filename))
            {
                if (MessageBox.Show("That theme already exists, would you like to create a new one?", "New Theme?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    filename = CloneTheme(filename);
                }
            }
            string ColorSet = "";
            for (int i = 0; i < m_arraylist.Count; ++i)
            {
                ColorSetting cs = (ColorSetting)m_arraylist[i];
                ColorSet += cs.GetSetting();
            }
            try
            {
                StreamWriter writer = new StreamWriter(filename);
                writer.Write(ColorSet);
                writer.Close();

            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
            this.Close();
        }

        private string CloneTheme(string filename)
        {
            string newName = "New";
            string newFolder = ClientSettings.AppPath + "\\Themes\\" + newName;
            System.IO.Directory.CreateDirectory(ClientSettings.AppPath + "\\Themes\\" + newName);
            foreach (string oldItem in System.IO.Directory.GetFiles(ClientSettings.AppPath + "\\Themes\\" + ThemeName))
            {
                System.IO.File.Copy(oldItem, newFolder + "\\" + System.IO.Path.GetFileName(oldItem));
            }
            return newFolder + "\\colors.txt";
        }

        private void menuCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
    class ColorSetting
    {
        public string name;
        public PictureBox pb;
        public ColorSetting(string strName, PictureBox pBox)
        {
            this.name = strName;
            this.pb = pBox;
        }
        public string GetSetting()
        {
            if (name != null || pb != null)
            {
                return name + ":" + pb.BackColor.R + ":" + pb.BackColor.G + ":" + pb.BackColor.B + "\r\n";
            }
            else
            {
                return "";
            }
        }
    }
}