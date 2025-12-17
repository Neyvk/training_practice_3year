using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TrainingPractice_neyaskin
{
    public partial class Form1 : Form
    {
        Rectangle player;
        List<Rectangle> objects = new List<Rectangle>();
        Random rand = new Random();

        int score = 0;
        int timeLeft = 60;
        int speed = 5;
        int speedX = 0;
        int speedY = 0;

        public Form1()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.KeyPreview = true;

            player = new Rectangle(190, 190, 20, 20);

            gameTimer.Tick += GameTimer_Tick;
            tickTimer.Tick += TickTimer_Tick;
            spawnTimer.Tick += SpawnTimer_Tick;
            panelGame.Paint += PanelGame_Paint;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            score = 0;
            objects.Clear();
            player.X = 190;
            player.Y = 190;

            labelScore.Text = "Счёт: 0";
            timeLeft = 60;
            labelTime.Text = "Время: 60";

            tickTimer.Start();
            spawnTimer.Start();
            gameTimer.Start();
            this.KeyUp += Form1_KeyUp;
            this.ActiveControl = null;
            this.Focus();
        }

        private void TickTimer_Tick(object sender, EventArgs e)
        {
            float moveX = speedX;
            float moveY = speedY;
            if (moveX != 0 && moveY != 0)
            {
                float factor = 1f / (float)Math.Sqrt(2);
                moveX *= factor;
                moveY *= factor;
            }

            player.X += (int)moveX;
            player.Y += (int)moveY;

            if (player.X < 0) player.X = 0;
            if (player.Y < 0) player.Y = 0;
            if (player.X + player.Width > panelGame.Width) player.X = panelGame.Width - player.Width;
            if (player.Y + player.Height > panelGame.Height) player.Y = panelGame.Height - player.Height;

            for (int i = objects.Count - 1; i >= 0; i--)
            {
                if (player.IntersectsWith(objects[i]))
                {
                    objects.RemoveAt(i);
                    score++;
                    labelScore.Text = $"Счёт: {score}";
                }
            }

            panelGame.Invalidate();
            this.ActiveControl = null;
            this.Focus();
        }

        private void SpawnTimer_Tick(object sender, EventArgs e)
        {
            Rectangle obj = new Rectangle(
                rand.Next(0, panelGame.Width - 15),
                rand.Next(0, panelGame.Height - 15),
                15, 15);

            objects.Add(obj);

            if (spawnTimer.Interval > 300)
                spawnTimer.Interval -= 50;
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            timeLeft--;
            labelTime.Text = $"Время: {timeLeft}";

            if (timeLeft <= 0)
            {
                tickTimer.Stop();
                spawnTimer.Stop();
                gameTimer.Stop();
                MessageBox.Show($"Игра окончена!\nСчёт: {score}");
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Left:
                    speedX = -speed;
                    return true;
                case Keys.Right:
                    speedX = speed;
                    return true;
                case Keys.Up:
                    speedY = -speed;
                    return true;
                case Keys.Down:
                    speedY = speed;
                    return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right) speedX = 0;
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down) speedY = 0;
        }

        private void PanelGame_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillRectangle(Brushes.Blue, player);
            foreach (var obj in objects)
            {
                g.FillRectangle(Brushes.Red, obj);
            }
        }
    }
}
