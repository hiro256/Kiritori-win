﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Kiritori
{
    enum HOTS
    {
        MOVE_LEFT   = Keys.Left,
        MOVE_RIGHT  = Keys.Right,
        MOVE_UP     = Keys.Up,
        MOVE_DOWN   = Keys.Down,
        SHIFT_MOVE_LEFT = Keys.Left     | Keys.Shift,
        SHIFT_MOVE_RIGHT = Keys.Right   | Keys.Shift,
        SHIFT_MOVE_UP = Keys.Up         | Keys.Shift,
        SHIFT_MOVE_DOWN = Keys.Down     | Keys.Shift,
        FLOAT = (int)Keys.Control + (int)Keys.A,
        SAVE        = (int)Keys.Control + (int)Keys.S,
        LOAD        = (int)Keys.Control + (int)Keys.O,
        OPEN        = (int)Keys.Control + (int)Keys.N,
        ZOOM_IN     = (int)Keys.Control + (int)Keys.Oemplus,
        ZOOM_OUT    = (int)Keys.Control + (int)Keys.OemMinus,
        CLOSE       = (int)Keys.Control + (int)Keys.W,
        ESCAPE      = Keys.Escape,
        COPY        = (int)Keys.Control + (int)Keys.C,
        CUT         = (int)Keys.Control + (int)Keys.X,
        PRINT       = (int)Keys.Control + (int)Keys.P,
        MINIMIZE    = (int)Keys.Control + (int)Keys.H
    }
    
    public partial class SnapWindow : Form
    {
        public DateTime date;
        private int ws, hs;
        private Boolean isWindowShadow = true;
        private Boolean isAfloatWindow = true;
        //マウスのクリック位置を記憶
        private Point mousePoint;
        private double alpha_value;
        private const double DRAG_ALPHA = 0.3;
        private const double MIN_ALPHA = 0.1;
        private const int    MOVE_STEP = 3;
        private const int SHIFT_MOVE_STEP = MOVE_STEP * 10;
        private const int THUMB_WIDTH = 250;
        private MainApplication ma;
        public Bitmap main_image;
        public Bitmap thumbnail_image;
        public SnapWindow(MainApplication mainapp)
        {
            this.ma = mainapp;
            this.isWindowShadow = Properties.Settings.Default.isWindowShadow;
            this.isAfloatWindow = Properties.Settings.Default.isAfloatWindow;
            this.alpha_value = Properties.Settings.Default.alpha_value / 100.0;
//            this.TransparencyKey = BackColor;
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.MouseDown +=
                new MouseEventHandler(Form1_MouseDown);
            pictureBox1.MouseMove +=
                new MouseEventHandler(Form1_MouseMove);
            pictureBox1.MouseUp +=
                new MouseEventHandler(Form1_MouseUp);
        }
        public void capture(Rectangle rc) {
            Bitmap bmp = new Bitmap(rc.Width, rc.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(
                    new Point(rc.X, rc.Y),
                    new Point(0, 0), new Size(rc.Width, rc.Height),
                    CopyPixelOperation.SourceCopy
                    );
            }
            this.Size = bmp.Size;
            pictureBox1.Size = bmp.Size;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom; //autosize
            pictureBox1.Image = bmp;
            date = DateTime.Now;
            this.Text = date.ToString("yyyyMMdd-HHmmss") + ".png";
            this.TopMost = this.isAfloatWindow;
            this.Opacity = this.alpha_value;

            this.main_image = bmp;
            this.setThumbnail(bmp);
            ma.setHistory(this);
        }
        private void setThumbnail(Bitmap bmp) {
            this.main_image = bmp;
            if (bmp.Size.Width > THUMB_WIDTH)
            {
                int resizeWidth = THUMB_WIDTH;
                int resizeHeight = (int)(bmp.Height * ((double)resizeWidth / (double)bmp.Width));
                Bitmap resizeBmp = new Bitmap(resizeWidth, resizeHeight);
                Graphics g = Graphics.FromImage(resizeBmp);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.DrawImage(bmp, 0, 0, resizeWidth, resizeHeight);
                g.Dispose();
                this.thumbnail_image = resizeBmp;
            }
            else
            {
                this.thumbnail_image = bmp;
            }
        }
        //マウスのボタンが押されたとき
        private void Form1_MouseDown(object sender,
            System.Windows.Forms.MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                //位置を記憶する
                mousePoint = new Point(e.X, e.Y);
            }
        }

        //マウスが動いたとき
        private void Form1_MouseMove(object sender,
            System.Windows.Forms.MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                this.Left += e.X - mousePoint.X;
                this.Top += e.Y - mousePoint.Y;
                this.Opacity = this.alpha_value * DRAG_ALPHA;
            }
        }

        //マウスのボタンが押されたとき
        private void Form1_MouseUp(object sender,
            System.Windows.Forms.MouseEventArgs e)
        {
            this.Opacity = this.alpha_value;
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
        private void Form1_Resize(object sender, System.EventArgs e)
        {
            Control control = (Control)sender;
            this.pictureBox1.Width = control.Size.Width;
            this.pictureBox1.Height = control.Size.Height;
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch((int)keyData){
                case (int)HOTS.MOVE_LEFT:
                    this.SetDesktopLocation(this.Location.X - MOVE_STEP, this.Location.Y);
                    break;
                case (int)HOTS.MOVE_RIGHT:
                    this.SetDesktopLocation(this.Location.X + MOVE_STEP, this.Location.Y);
                    break;
                case (int)HOTS.MOVE_UP:
                    this.SetDesktopLocation(this.Location.X, this.Location.Y - MOVE_STEP);
                    break;
                case (int)HOTS.MOVE_DOWN:
                    this.SetDesktopLocation(this.Location.X, this.Location.Y + MOVE_STEP);
                    break;
                case (int)HOTS.SHIFT_MOVE_LEFT:
                    this.SetDesktopLocation(this.Location.X - SHIFT_MOVE_STEP, this.Location.Y);
                    break;
                case (int)HOTS.SHIFT_MOVE_RIGHT:
                    this.SetDesktopLocation(this.Location.X + SHIFT_MOVE_STEP, this.Location.Y);
                    break;
                case (int)HOTS.SHIFT_MOVE_UP:
                    this.SetDesktopLocation(this.Location.X, this.Location.Y - SHIFT_MOVE_STEP);
                    break;
                case (int)HOTS.SHIFT_MOVE_DOWN:
                    this.SetDesktopLocation(this.Location.X, this.Location.Y + SHIFT_MOVE_STEP);
                    break;
                case (int)HOTS.FLOAT:
                    this.TopMost = !this.TopMost;
                    break;
                case (int)HOTS.ESCAPE:
                case (int)HOTS.CLOSE:
                    this.Close();
                    break;
                case (int)HOTS.SAVE:
                    saveImage();
                    break;
                case (int)HOTS.LOAD:
                    loadImage();
                    break;
                case (int)HOTS.OPEN:
                    openImage();
                    break;
                case (int)HOTS.ZOOM_IN:
                    zoomIn();
                    break;
                case (int)HOTS.ZOOM_OUT:
                    zoomOut();
                    break;
                case (int)HOTS.COPY:
                    Clipboard.SetImage(this.pictureBox1.Image);
                    break;
                case (int)HOTS.CUT:
                    Clipboard.SetImage(this.pictureBox1.Image);
                    this.Close();
                    break;
                case (int)HOTS.PRINT:
                    printImage();
                    break;
                case (int)HOTS.MINIMIZE:
                    minimizeWindow();
                    break;
                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
            return true;
        }
        public void zoomIn() {
            ws = (int)(this.pictureBox1.Width * 0.1);
            hs = (int)(this.pictureBox1.Height * 0.1);
            this.pictureBox1.Width += ws;
            this.pictureBox1.Height += hs;
            this.SetDesktopLocation(this.Location.X - ws / 2, this.Location.Y - hs / 2);
        }
        public void zoomOut() {
            ws = (int)(this.pictureBox1.Width * 0.1);
            hs = (int)(this.pictureBox1.Height * 0.1);
            this.pictureBox1.Width -= ws;
            this.pictureBox1.Height -= hs;
            this.SetDesktopLocation(this.Location.X + ws / 2, this.Location.Y + hs / 2);
        }
        public void zoomOff()
        {
            this.pictureBox1.Width = this.pictureBox1.Image.Width;
            this.pictureBox1.Height = this.pictureBox1.Image.Height;
        }
        public void saveImage()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = this.Text;
            sfd.Filter =
                "Image Files(*.png;*.PNG)|*.png;*.PNG|All Files(*.*)|*.*";
            sfd.FilterIndex = 1;
            sfd.Title = "Select a path to save the image";
            sfd.RestoreDirectory = true;
            sfd.OverwritePrompt = true;
            sfd.CheckPathExists = true;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                this.pictureBox1.Image.Save(sfd.FileName);
            }
        }
        public void loadImage()
        {
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Title = "Load Image";


                // ファイルのフィルタを設定する
                openFileDialog1.Filter = "Image|*.png;*.PNG;*.jpg;*.JPG;*.jpeg;*.JPEG;*.gif;*.GIF;*.bmp;*.BMP|すべてのファイル|*.*";
                openFileDialog1.FilterIndex = 1;

                // 有効な Win32 ファイル名だけを受け入れるようにする (初期値 true)
                openFileDialog1.ValidateNames = false;

                // ダイアログを表示し、戻り値が [OK] の場合は、選択したファイルを表示する
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    // 不要になった時点で破棄する (正しくは オブジェクトの破棄を保証する を参照)
                    openFileDialog1.Dispose();

                    Bitmap bmp = new System.Drawing.Bitmap(openFileDialog1.FileName);
                    Size bs = bmp.Size;
                    int sw = System.Windows.Forms.Screen.GetBounds(this).Width;
                    int sh = System.Windows.Forms.Screen.GetBounds(this).Height;
                    if (bs.Height > sh)
                    {
                        bs.Height = sh;
                        bs.Width = (int)((double)bs.Height * ((double)bmp.Width / (double)bmp.Height));
                    }
                    this.Size = bs;
                    pictureBox1.Size = bs;
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom; //autosize
                    pictureBox1.Image = bmp;
                    this.Text = openFileDialog1.FileName;
                    this.StartPosition = FormStartPosition.CenterScreen;
                    this.SetDesktopLocation(this.DesktopLocation.X - (int)(this.Size.Width / 2.0), this.DesktopLocation.Y - (int)(this.Size.Height / 2.0));

                    this.main_image = bmp;
                    this.setThumbnail(bmp);
                    ma.setHistory(this);
                }
                else
                {
                    openFileDialog1.Dispose();
                }
            }
            catch {
                this.Close();
            }
        }
        public void openImage()
        {
            this.ma.openImage();
        }
        public void setImageFromPath(String fname)
        {
            try
            {
                Bitmap bmp = new System.Drawing.Bitmap(fname);
                Size bs = bmp.Size;
                int sw = System.Windows.Forms.Screen.GetBounds(this).Width;
                int sh = System.Windows.Forms.Screen.GetBounds(this).Height;
                if (bs.Height > sh)
                {
                    bs.Height = sh;
                    bs.Width = (int)((double)bs.Height * ((double)bmp.Width / (double)bmp.Height));
                }
                this.Size = bs;
                pictureBox1.Size = bs;
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom; //autosize
                pictureBox1.Image = bmp;
                this.Text = fname;
                this.TopMost = this.isAfloatWindow;
                this.Opacity = this.alpha_value;
                this.StartPosition = FormStartPosition.CenterScreen;
                this.SetDesktopLocation(this.DesktopLocation.X - (int)(this.Size.Width /2.0), this.DesktopLocation.Y - (int)(this.Size.Height /2.0));

                this.main_image = bmp;
                this.setThumbnail(bmp);
                ma.setHistory(this);
            }
            catch {
                this.Close();
            }
        }
        public void setImageFromBMP(Bitmap bmp)
        {
            try
            {
                Size bs = bmp.Size;
                int sw = System.Windows.Forms.Screen.GetBounds(this).Width;
                int sh = System.Windows.Forms.Screen.GetBounds(this).Height;
                if (bs.Height > sh)
                {
                    bs.Height = sh;
                    bs.Width = (int)((double)bs.Height * ((double)bmp.Width / (double)bmp.Height));
                }
                this.Size = bs;
                pictureBox1.Size = bs;
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom; //autosize
                pictureBox1.Image = bmp;
                this.TopMost = this.isAfloatWindow;
                this.Opacity = this.alpha_value;
                this.StartPosition = FormStartPosition.CenterScreen;
                this.SetDesktopLocation(this.DesktopLocation.X - (int)(this.Size.Width / 2.0), this.DesktopLocation.Y - (int)(this.Size.Height / 2.0));

                this.main_image = bmp;
                this.setThumbnail(bmp);
//                ma.setHistory(this);
            }
            catch
            {
                this.Close();
            }
        }
        public void printImage()
        {
            PrintDialog printDialog1 = new PrintDialog();
            printDialog1.PrinterSettings = new System.Drawing.Printing.PrinterSettings();
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                System.Drawing.Printing.PrintDocument pd =
                    new System.Drawing.Printing.PrintDocument();
                pd.PrintPage +=
                    new System.Drawing.Printing.PrintPageEventHandler(pd_PrintPage);
                pd.Print();
            }
            printDialog1.Dispose();
        }
        private void pd_PrintPage(object sender,
                System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawImage(this.pictureBox1.Image, e.MarginBounds);
            e.HasMorePages = false;
        }
        public void minimizeWindow()
        {
            this.WindowState = FormWindowState.Minimized;
        }
        public void showWindow()
        {
            this.WindowState = FormWindowState.Normal;
        }
        const int CS_DROPSHADOW = 0x00020000;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                if (this.isWindowShadow)
                {
                    cp.ClassStyle |= CS_DROPSHADOW;
                }
                return cp;
            }
        }

        private void closeESCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cutCtrlXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetImage(this.pictureBox1.Image);
            this.Close();
        }

        private void copyCtrlCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetImage(this.pictureBox1.Image);
        }

        private void keepAfloatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TopMost = !this.TopMost;
        }

        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveImage();
        }

        private void originalSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zoomOff();
        }

        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zoomIn();
        }

        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zoomOut();
        }

        private void dropShadowToolStripMenuItem_Click(object sender, EventArgs e)
        {
//            this.FormBorderStyle = FormBorderStyle.None;
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrefForm pref = new PrefForm();
            pref.Show();
        }
        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printImage();
        }
        public void setAlpha(double alpha) {
            this.Opacity = alpha;
            this.alpha_value = alpha;
        }
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            setAlpha(1.0);
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            setAlpha(0.9);
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            setAlpha(0.8);
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            setAlpha(0.5);
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            setAlpha(0.3);
        }

        private void minimizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.minimizeWindow();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
