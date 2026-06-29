using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Fighter_jet_shooter_game
{
    public partial class Form1 : Form
    {
        bool goLeft, goRight, shooting, isGameOver;
        int score;
        int playerSpeed = 12;
        int enemySpeed;
        int bulletSpeed;
        Random rnd = new Random();

        // --- Cloud system ---
        List<Cloud> clouds = new List<Cloud>();

        // Nested Cloud class
        private class Cloud
        {
            public PictureBox PictureBox { get; private set; }
            private int speedOffset; // individual variation
            private Random rnd;
            private int formWidth;
            private int formHeight;

            public Cloud(PictureBox pb, int speedOffset, Random rnd, int formWidth, int formHeight)
            {
                this.PictureBox = pb;
                this.speedOffset = speedOffset;
                this.rnd = rnd;
                this.formWidth = formWidth;
                this.formHeight = formHeight;
            }

            // Move downward; base speed comes from caller, individual offset applied
            public void Move(int baseSpeed)
            {
                int actualSpeed = Math.Max(1, baseSpeed + speedOffset);
                PictureBox.Top += actualSpeed;

                // Recycle: when fully off the bottom, reset above the top
                if (PictureBox.Top > formHeight)
                {
                    PictureBox.Top = rnd.Next(-200, -50);
                    PictureBox.Left = rnd.Next(0, formWidth - PictureBox.Width);
                }
            }

            // Scatter clouds at random positions for fresh start
            public void Reset(int formWidth, int formHeight)
            {
                this.formWidth = formWidth;
                this.formHeight = formHeight;
                PictureBox.Left = rnd.Next(0, formWidth - PictureBox.Width);
                PictureBox.Top = rnd.Next(-300, formHeight); // spread across full height at start
            }
        }

        public Form1()
        {
            InitializeComponent();
            resetGame();
        }

        private void mainGameTimerEvent(object sender, EventArgs e)
        {
            txtScore.Text = score.ToString();

            enemyOne.Top += enemySpeed;
            enemyTwo.Top += enemySpeed;
            enemyThree.Top += enemySpeed;

            if (enemyOne.Top > 700 || enemyTwo.Top > 700 || enemyThree.Top > 700)
            {
                gameOver();
            }

            // Player movement
            if (goLeft == true && player.Left > 0)
                player.Left -= playerSpeed;
            if (goRight == true && player.Left < 435)
                player.Left += playerSpeed;

            // Bullet logic
            if (shooting == true)
            {
                bulletSpeed = 20;
                bullet.Top -= bulletSpeed;
            }
            else
            {
                bullet.Left = -300;
                bulletSpeed = 0;
            }
            if (bullet.Top < -30)
                shooting = false;

            // Bullet-enemy collisions
            if (bullet.Bounds.IntersectsWith(enemyOne.Bounds))
            {
                score += 1;
                enemyOne.Top = -450;
                enemyOne.Left = rnd.Next(20, 450);
                shooting = false;
            }
            if (bullet.Bounds.IntersectsWith(enemyTwo.Bounds))
            {
                score += 1;
                enemyTwo.Top = -650;
                enemyTwo.Left = rnd.Next(20, 450);
                shooting = false;
            }
            if (bullet.Bounds.IntersectsWith(enemyThree.Bounds))
            {
                score += 1;
                enemyThree.Top = -750;
                enemyThree.Left = rnd.Next(20, 450);
                shooting = false;
            }

            // Enemy speed scaling
            if (score == 5) enemySpeed = 7;
            if (score == 10) enemySpeed = 9;
            if (score == 15) enemySpeed = 11;

            // --- Move clouds ---
            // Cloud base speed lags behind enemies for parallax depth
            int cloudBaseSpeed = Math.Max(1, enemySpeed - 2);
            foreach (Cloud c in clouds)
                c.Move(cloudBaseSpeed);
        }

        private void keyisdown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) goLeft = true;
            if (e.KeyCode == Keys.Right) goRight = true;
        }

        private void keyisup(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) goLeft = false;
            if (e.KeyCode == Keys.Right) goRight = false;

            if (e.KeyCode == Keys.Space && shooting == false)
            {
                shooting = true;
                bullet.Top = player.Top - 35;
                bullet.Left = player.Left + (player.Width / 2);
            }
            if (e.KeyCode == Keys.Enter && isGameOver == true)
                resetGame();
        }

        private void resetGame()
        {
            isGameOver = false;
            gameTimer.Start();
            enemySpeed = 5;

            enemyOne.Left = rnd.Next(20, 450);
            enemyTwo.Left = rnd.Next(20, 450);
            enemyThree.Left = rnd.Next(20, 450);

            enemyOne.Top = rnd.Next(0, 220) * -1;
            enemyTwo.Top = rnd.Next(0, 320) * -1;
            enemyThree.Top = rnd.Next(0, 390) * -1;

            score = 0;
            bulletSpeed = 0;
            bullet.Left = -300;
            shooting = false;
            txtScore.Text = score.ToString();

            // --- Initialize cloud pool ---
            // Speed offsets give each cloud a unique feel: -1, 0, +1, +2
            int[] speedOffsets = { -1, 0, 1, 2 };
            PictureBox[] cloudBoxes = { pictureBox1, pictureBox2, pictureBox3, pictureBox4 };

            clouds.Clear();
            for (int i = 0; i < cloudBoxes.Length; i++)
            {
                Cloud c = new Cloud(cloudBoxes[i], speedOffsets[i], rnd,
                                    this.ClientSize.Width, this.ClientSize.Height);
                c.Reset(this.ClientSize.Width, this.ClientSize.Height);
                clouds.Add(c);
            }
        }

        private void gameOver()
        {
            isGameOver = true;
            gameTimer.Stop();
            txtScore.Text += Environment.NewLine + " Game Over!!" + Environment.NewLine + "Press Enter to try again.";
        }

        private void enemyTwo_Click(object sender, EventArgs e) { }

        private void pictureBox1_Click(object sender, EventArgs e) { }

        private void enemyOne_Click(object sender, EventArgs e)
        {

        }
        private void bullet_Click(object sender, EventArgs e)
        {

        }
        
    }
}