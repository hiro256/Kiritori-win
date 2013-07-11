﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kiritori
{
    public partial class ScreenWindow : Form
    {
        private Graphics g;
        private Bitmap bmp;
        private Boolean isOpen;
        private ArrayList captureArray;
        public ScreenWindow()
        {
            captureArray = new ArrayList();
            isOpen = true;
            InitializeComponent();
        }
        public Boolean isScreenOpen() {
            return this.isOpen;
        }
        private void Screen_Load(object sender, EventArgs e)
        {
            pictureBox1.MouseDown +=
                    new MouseEventHandler(ScreenWindow_MouseDown);
            pictureBox1.MouseMove +=
                    new MouseEventHandler(ScreenWindow_MouseMove);
            pictureBox1.MouseUp +=
                    new MouseEventHandler(ScreenWindow_MouseUp);
        }
        public void showScreen() {
            this.Opacity = 0.61;
            int h, w;
            //ディスプレイの高さ
            h = System.Windows.Forms.Screen.GetBounds(this).Height;
            //ディスプレイの幅
            w = System.Windows.Forms.Screen.GetBounds(this).Width;
            this.SetBounds(0, 0, w, h);
            bmp = new Bitmap(w, h);
            using (g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(
                    new Point(0, 0),
                    new Point(w, h), bmp.Size
                );
            }
            pictureBox1.SetBounds(0, 0, w, h);
            pictureBox1.SizeMode = PictureBoxSizeMode.Normal;
            pictureBox1.Image = bmp;
            pictureBox1.Refresh();
            this.Refresh();
            this.Update();
            this.TopLevel = true;
            this.Show();
        }
        //マウスのクリック位置を記憶
        private Point startPoint;
        private Point endPoint;
        private Rectangle rc;
        private Boolean isPressed = false;
        //マウスのボタンが押されたとき
        private void ScreenWindow_MouseDown(object sender,
            System.Windows.Forms.MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                //位置を記憶する
                startPoint = new Point(e.X, e.Y);
                isPressed = true;
            }
        }
        //マウスが動いたとき
        private void ScreenWindow_MouseMove(object sender,
            System.Windows.Forms.MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                rc = new Rectangle();
                Pen p = new Pen(Color.Black, 10);
                if (startPoint.X < e.X)
                {
                    rc.X = startPoint.X;
                    rc.Width = e.X - startPoint.X;
                }
                else
                {
                    rc.X = e.X;
                    rc.Width = startPoint.X - e.X;
                }
                if (startPoint.Y < e.Y)
                {
                    rc.Y = startPoint.Y;
                    rc.Height = e.Y - startPoint.Y;
                }
                else
                {
                    rc.Y = e.Y;
                    rc.Height = startPoint.Y - e.Y;
                }
                {
                    Pen blackPen = new Pen(Color.Black);
                    using (Graphics g = Graphics.FromImage(bmp)) {
                        blackPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        blackPen.Width = 1;
                        g.Clear(SystemColors.Control);
                        g.DrawRectangle(blackPen, rc);
                        Font fnt = new Font("Arial", 10);
                        g.DrawString(rc.Width.ToString() + "x" + rc.Height.ToString(), fnt, Brushes.Black, e.X + 5, e.Y + 10);
                        fnt.Dispose();
                        g.Dispose();
                    }
                    pictureBox1.Refresh();
                }
            }
        }
        //マウスのボタンが離されたとき
        private void ScreenWindow_MouseUp(object sender,
            System.Windows.Forms.MouseEventArgs e)
        {
            if (isPressed)
            {
                endPoint = new Point(e.X, e.Y);
                isPressed = false;
                this.CloseScreen();
                if(rc.Width != 0 || rc.Height != 0){
                    SnapWindow sw = new SnapWindow();
                    sw.capture(rc);
                    sw.Show();
                    sw.SetDesktopLocation(rc.X, rc.Y);
                    sw.FormClosing +=
                        new FormClosingEventHandler(SW_FormClosing);
                    captureArray.Add(sw);
                }
            }
        }
        public void hideWindows() {
            foreach(SnapWindow sw in captureArray){
                sw.minimizeWindow();
            }
        }
        public void showWindows()
        {
            foreach (SnapWindow sw in captureArray)
            {
                sw.showWindow();
            }
        }
        private void CloseScreen()
        {
            this.isOpen = false;
            this.Hide();
        }
        void SW_FormClosing(object sender, FormClosingEventArgs e)
        {
            captureArray.Remove(sender);
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch ((int)keyData)
            {
                case (int)HOTS.ESCAPE:
                case (int)HOTS.CLOSE:
                    this.CloseScreen();
                    break;
                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
            return true;
        }
    }
}
