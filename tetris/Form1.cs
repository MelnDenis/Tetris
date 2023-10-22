using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tetrisish
{
    public partial class Form1 : Form
    {
        public const int breadth = 15, depth = 25, block = 15;
        public int[,] form = new int[2, 4];
        public int[,] pole = new int[breadth, depth];
        public Bitmap bitpole = new Bitmap(block * (breadth + 1) + 1, block * (depth + 3) + 1);
        public Graphics draw;
        public Form1()
        {
            InitializeComponent();
            draw = Graphics.FromImage(bitpole);
            for (int i = 0; i < breadth; i++)
            {
                pole[i, depth - 1] = 1;
            }
            for (int i = 0; i < depth; i++)
            {
                pole[0, i] = 1;
                pole[breadth - 1, i] = 1;
            }
            Figure();
            draw.Clear(Color.DarkGray);
            for (int i = 0; i < breadth; i++)
                for (int j = 0; j < depth; j++)
                    if (pole[i, j] == 1)
                    {
                        draw.FillRectangle(Brushes.MediumVioletRed, i * block, j * block, block, block);
                        draw.DrawRectangle(Pens.DarkGray, i * block, j * block, block, block);
                    }
            pictureBox1.Image = bitpole;
            timer1.Stop();
        }
        public void FillPole()
        {
            draw.Clear(Color.DarkGray);
            for (int i = 0; i < breadth; i++)
                for (int j = 0; j < depth; j++)
                    if (pole[i, j] == 1)
                    {
                        draw.FillRectangle(Brushes.MediumVioletRed, i * block, j * block, block, block);
                        draw.DrawRectangle(Pens.DarkGray, i * block, j * block, block, block);
                    }
            for (int i = 0; i < 4; i++)
            {
                draw.FillRectangle(Brushes.GreenYellow, form[1, i] * block, form[0, i] * block, block, block);
                draw.DrawRectangle(Pens.DarkGray, form[1, i] * block, form[0, i] * block, block, block);
            }
            pictureBox1.Image = bitpole;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (pole[8, 3] == 1)
            {
                timer1.Stop();
                for (int i = 1; i < breadth - 1; i++)
                    for (int j = 1; j < depth - 1; j++)
                        pole[i, j] = 0; ;
            }
                for (int i = 0; i < 4; i++)
                form[0, i]++;
            for (int i = depth - 2; i > 2; i--)
            {
                var cross = (from t in Enumerable.Range(0, pole.GetLength(0)).Select(j => pole[j, i]).ToArray() where t == 1 select t).Count();
                if (cross == breadth)
                    for (int k = i; k > 1; k--)
                        for (int l = 1; l < breadth - 1; l++)
                            pole[l, k] = pole[l, k - 1];
            }
            if (SearchMist())
            {
                for (int i = 0; i < 4; i++)
                    pole[form[1, i], --form[0, i]]++;
                Figure();
            }
            FillPole();
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    for (int i = 0; i < 4; i++)
                        form[1, i]--;
                    if (SearchMist())
                        for (int i = 0; i < 4; i++)
                            form[1, i]++;
                    break;
                case Keys.D:
                    for (int i = 0; i < 4; i++)
                        form[1, i]++;
                    if (SearchMist())
                        for (int i = 0; i < 4; i++)
                            form[1, i]--;
                    break;
                case Keys.W:
                    var shapeT = new int[2, 4];
                    Array.Copy(form, shapeT, form.Length);
                    int maxx = 0, maxy = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        if (form[0, i] > maxy)
                            maxy = form[0, i];
                        if (form[1, i] > maxx)
                            maxx = form[1, i];
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        int temp = form[0, i];
                        form[0, i] = maxy - (maxx - form[1, i]) - 1;
                        form[1, i] = maxx - (3 - (maxy - temp)) + 1;
                    }
                    if (SearchMist())
                        Array.Copy(shapeT, form, form.Length);
                    break;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 1; i < breadth-1; i++)
                for (int j = 1; j < depth-1; j++)
                    pole[i, j] = 0;
            Figure();
            timer1.Stop();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }
        public void Figure()
        {
            Random x = new Random(DateTime.Now.Millisecond);
            switch (x.Next(7))
            {
                case 0: form = new int[,] { { 2, 3, 4, 5 }, { 8, 8, 8, 8 } }; break;
                case 1: form = new int[,] { { 2, 3, 2, 3 }, { 8, 8, 9, 9 } }; break;
                case 2: form = new int[,] { { 2, 3, 4, 4 }, { 8, 8, 8, 9 } }; break;
                case 3: form = new int[,] { { 2, 3, 4, 4 }, { 8, 8, 8, 7 } }; break;
                case 4: form = new int[,] { { 3, 3, 4, 4 }, { 7, 8, 8, 9 } }; break;
                case 5: form = new int[,] { { 3, 3, 4, 4 }, { 9, 8, 8, 7 } }; break;
                case 6: form = new int[,] { { 3, 4, 4, 4 }, { 8, 7, 8, 9 } }; break;
            }
        }
        public bool SearchMist()
        {
            for (int i = 0; i < 4; i++)
                if (form[1, i] >= breadth || form[0, i] >= depth ||
                    form[1, i] <= 0 || form[0, i] <= 0 ||
                    pole[form[1, i], form[0, i]] == 1)
                    return true;
            return false;
        }
    }
}
