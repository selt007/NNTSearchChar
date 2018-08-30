﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace NNTSearchChar
{
    public partial class MainForm : Form
    {
        MashineVision MV = new MashineVision();
        Point CurrentPoint;
        Graphics g;
        Bitmap bmap;

        public MainForm()
        {
            InitializeComponent();
            bmap = new Bitmap(picture1.Width, picture1.Height);
            picture1.MouseDown += picture1_MouseDown;
            picture1.Paint += picture1_Paint;
            g = picture1.CreateGraphics();

            picture2.Image = Image.FromFile("ikea.jpg");
            Bitmap stBm = new Bitmap(picture2.Image);
            Rectangle rect = new Rectangle(
                picture2.Location.X + (picture2.Location.X / 3 - 50), 
                picture2.Location.Y + (picture2.Location.X + 100), 
                picture2.Width, picture2.Height);
            picture2.Image = stBm.Clone(rect, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
        }

        private void picture1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                using (Graphics g = Graphics.FromImage(bmap))
                {
                    g.FillEllipse(Brushes.Black, e.X, e.Y, 10, 10);
                }
                picture1.Invalidate();
            }
        }

        private void picture1_MouseDown(object sender, MouseEventArgs e)
        {
            CurrentPoint = e.Location;
        }

        private void picture1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(bmap, Point.Empty);
        }

        private void button_Click(object sender, EventArgs e)
        {
            double hue, sat, val;
            Bitmap bm = new Bitmap(picture2.Image);

            for (var i = 0; i < picture2.Height; i++)
            {
                bool err = false;
                for (var j = 0; j < picture2.Width; j++)
                {
                    try
                    {
                        MashineVision.ColToHSV(bm.GetPixel(i, j), out hue, out sat, out val);
                        bm.SetPixel(i, j, MashineVision.ColFromHSV(hue, sat, val));
                    }
                    catch (ArgumentException ex)
                    {
                        MessageBox.Show($"Сообщение об ошибке: \n{ex}\n\n" +
                            $"Один из аргументов имеет значение выходящее из-за диапазона [0-255].", "Error!",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        err = true; break;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Неизвестная ошибка !\n\nСообщение об ошибке: \n{ex}", "Error!",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        err = true; break;
                    }
                }
                if (err) break;
            }
            picture2.Image = bm;
        }
    }
}