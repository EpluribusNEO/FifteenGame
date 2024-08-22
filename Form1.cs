using System.Reflection.Metadata.Ecma335;

namespace Fifteen
{
    public partial class Form1 : Form
    {

        const int nw = 4, nh = 4; // размерность игрового поля

        System.Drawing.Graphics g; //графическая поверхность поля

        Bitmap pics; // картинка

        int cw, ch; //размер клетки

        int[,] field = new int[nw, nh]; //игровое поле хранит номер фрагментов картинки

        int ex, ey; //поординаты пустой кетки

        Boolean showNumbers = false; // признак отображения номера фишки
        String path;
        public Form1()
        {
            InitializeComponent();
            String path = "D:\\Den\\github.bmp";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                path = openFileDialog.FileName;

            }
            try
            {

                pics = new Bitmap(path);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Файл картинки не найден!\n" + exc.ToString(), "Собери картинку", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            //определить ширену и высоту клетки
            cw = (int)(pics.Width / nw);
            ch = (int)(pics.Height / nh);

            // установить размер клиентской области
            this.ClientSize = new System.Drawing.Size(cw * nw + 1, ch * nw + menuStrip1.Height);

            g = this.CreateGraphics();

            this.newGame();
        }

        private void newGame()
        {
            //расположим клетки в правильном порядке
            for (int j = 0; j < nh; j++)
            {
                for (int i = 0; i < nw; i++)
                {
                    field[i, j] = j * nw + i + 1;
                }
            }

            //последняя клетка пустая
            field[nw - 1, nh - 1] = 0;
            ex = nw - 1;
            ey = nh - 1;

            this.mixer();
            this.drawField();
        }


        private void mixer()
        {
            int d; // положение перемещённой клетки (относительно пустой) 0: слева, 1: справа, 2: сверху, 3: снизу

            int x, y; // перемещённая клетка

            Random rnd = new Random();

            // nw * nh * 10 количество перестановок---
            for (int i = 0; i < nw * nh * 10; i++)
            {
                x = ex;
                y = ey;

                d = rnd.Next(4);
                switch (d)
                {
                    case 0:
                        if (x > 0) { x--; }
                        break;
                    case 1:
                        if (x < nw - 1) { x++; }
                        break;
                    case 2:
                        if (y > 0) { y--; }
                        break;
                    case 3:
                        if (y < nh - 1) { y++; }
                        break;
                }

                // здесь определили клетку, которую нужно перемещать в пусстую
                field[ex, ey] = field[x, y];
                field[x, y] = 0;

                //запомнить координаты пустой клетки
                ex = x;
                ey = y;
            }
        }

        private void drawField()
        {
            
            // содержимое клеток
            for (int i = 0; i < nw; i++)
            {
                for (int j = 0; j < nh; j++)
                {
                    if (field[i, j] != 0)
                    {
                        // выводим клетку с картинкой
                        // ( (field[i,j]-1) % nw) * cw,
                        // (int)((field[i,j] -1) - nw)* ch) 
                        // координаты верхнего левого угла области картинки
                        g.DrawImage(pics, new Rectangle(i * cw, j * ch + menuStrip1.Height, cw, ch),
                            new Rectangle(((field[i, j] - 1) % nw) * cw, ((field[i, j] - 1) / nw) * ch, cw, ch), GraphicsUnit.Pixel);
                    }
                    else
                    {
                        // выведим пустую клетку
                        g.FillRectangle(SystemBrushes.Control, i * cw, j * ch + menuStrip1.Height, cw, ch);

                        //рисуем границу
                        g.DrawRectangle(Pens.Black, i * cw, j * ch + menuStrip1.Height, cw, ch);

                        //номер клетки
                        if ((showNumbers) && field[i, j] != 0)
                        {
                            g.DrawString(
                                Convert.ToString(field[i, j]),
                                new Font("Tahoma", 10, FontStyle.Bold),
                                Brushes.Black, i * cw + 5,
                                j * ch + 5 + menuStrip1.Height);
                        }
                    }
                }
            }
        }


        private Boolean finish()
        {
            //координаты клетки
            int i = 0;
            int j = 0;

            int c; // число в клетки

            for (c = 1; c < nw * nh; c++)
            {
                if (field[i, j] != c) { return false; }

                if (i < nw - 1) { i++; }
                else { i = 0; j++; }

            }
            return true;
        }

        private void move(int cx, int cy)
        {
            if (!(((Math.Abs(cx - ex) == 1) && (cy - ey == 0)) ||
                ((Math.Abs(cy - ey) == 1) && (cx - ex == 0))))
            { return; }

            //обмен клетками (x,y) <=> (ex, ey)
            field[ex, ey] = field[cx, cy];
            field[cx, cy] = 0;

            ex = cx; ey = cy;

            //отрисовка поля
            this.drawField();
            if (this.finish())
            {
                field[nw - 1, nh - 1] = nh * nw;
                this.drawField();

                DialogResult dr = MessageBox.Show("You Win!\nPlay again?", "Собери картинку", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    this.newGame();
                }
                else
                {
                    this.Close();
                }
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            drawField();
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            //преобразовать поординаты курсора в поординаты клетки
            move(e.X / cw, (e.Y - menuStrip1.Height) / ch);

        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.newGame();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Игра петнашка.", "Собери картинку!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}
