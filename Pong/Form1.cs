/*
 * Description:     A basic PONG simulator
 * Author:           
 * Date:            
 */

#region libraries

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Windows.Forms;

#endregion

namespace Pong
{
    public partial class Form1 : Form
    {
        #region global values

        //graphics objects for drawing
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        SolidBrush redBrush = new SolidBrush(Color.Red);
        SolidBrush blueBrush = new SolidBrush(Color.Blue);
        Font drawFont = new Font("Courier New", 10);

        // Sounds for game
        SoundPlayer scoreSound = new SoundPlayer(Properties.Resources.score);
        SoundPlayer collisionSound = new SoundPlayer(Properties.Resources.collision);

        //determines whether a key is being pressed or not
        Boolean wKeyDown, sKeyDown, upKeyDown, downKeyDown;

        // check to see if a new game can be started
        Boolean newGameOk = true;

        //ball values
        Boolean ballMoveRight = true;
        Boolean ballMoveDown = true;
        int BALL_SPEEDX = 4;
        int BALL_SPEEDY = 5;
        const int BALL_WIDTH = 20;
        const int BALL_HEIGHT = 20;
        Rectangle ball;

        //player values
        int PADDLE_SPEED = 4;
        const int PADDLE_EDGE = 20;  // buffer distance between screen edge and paddle            
        const int PADDLE_WIDTH = 10;
        const int PADDLE_HEIGHT = 40;
        Rectangle player1, player2;

        //player and game scores
        int player1Score = 0;
        int player2Score = 0;
        int gameWinScore = 1;  // number of points needed to win game

        List<Rectangle> balls = new List<Rectangle>();

        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //check to see if a key is pressed and set is KeyDown value to true if it has
            switch (e.KeyCode)
            {
                case Keys.W:
                    wKeyDown = true;
                    break;
                case Keys.S:
                    sKeyDown = true;
                    break;
                case Keys.Up:
                    upKeyDown = true;
                    break;
                case Keys.Down:
                    downKeyDown = true;
                    break;
                case Keys.Y:
                case Keys.Space:
                    if (newGameOk)
                    {
                        SetParameters();
                    }
                    break;
                case Keys.Escape:
                    if (newGameOk)
                    {
                        Close();
                    }
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //check to see if a key has been released and set its KeyDown value to false if it has
            switch (e.KeyCode)
            {
                case Keys.W:
                    wKeyDown = false;
                    break;
                case Keys.S:
                    sKeyDown = false;
                    break;
                case Keys.Up:
                    upKeyDown = false;
                    break;
                case Keys.Down:
                    downKeyDown = false;
                    break;
            }
        }


        /// <summary>
        /// sets the ball and paddle positions for game start
        /// </summary>
        private void SetParameters()
        {
            if (newGameOk)
            {
                player1Score = player2Score = 0;
                newGameOk = false;
                startLabel.Visible = false;
                winnerLabel.Visible = false;
                gameUpdateLoop.Start();
                BALL_SPEEDX = 4;
                BALL_SPEEDY = 5;
            }

            //player start positions
            player1 = new Rectangle(PADDLE_EDGE, this.Height / 2 - PADDLE_HEIGHT / 2, PADDLE_WIDTH, PADDLE_HEIGHT);
            player2 = new Rectangle(this.Width - PADDLE_EDGE - PADDLE_WIDTH, this.Height / 2 - PADDLE_HEIGHT / 2, PADDLE_WIDTH, PADDLE_HEIGHT);

            // TODO create a ball rectangle in the middle of screen

            ball = new Rectangle(this.Width / 2 - BALL_WIDTH, this.Height / 2 - BALL_HEIGHT, BALL_WIDTH, BALL_HEIGHT);

        }

        /// <summary>
        /// This method is the game engine loop that updates the position of all elements
        /// and checks for collisions.
        /// </summary>
        private void gameUpdateLoop_Tick(object sender, EventArgs e)
        {
            //Ball Movement
            ball.X += BALL_SPEEDX;
            ball.Y += BALL_SPEEDY;

            //Player Controls
            if (wKeyDown == true && player1.Y > 0)
            {
                player1.Y -= PADDLE_SPEED;
            }
            if (sKeyDown == true && player1.Y < this.Height - player1.Height)
            {
                player1.Y += PADDLE_SPEED;
            }

            if (upKeyDown == true && player2.Y > 0)
            {
                player2.Y -= PADDLE_SPEED;
            }
            if (downKeyDown == true && player2.Y < this.Height - player2.Height)
            {
                player2.Y += PADDLE_SPEED;
            }
            if (player1.Y == 0 || player2.Y == this.Height - PADDLE_HEIGHT)
            {

            }

            //Collision with top and bottom
            if (ball.Y == this.Height - BALL_HEIGHT || ball.Y == 0)
            {
                BALL_SPEEDY *= -1;
            }

            //Collision with paddles
            if (player1.IntersectsWith(ball))
            {
                BALL_SPEEDX *= -1;
            }
            else if (player2.IntersectsWith(ball))
            {
                BALL_SPEEDX *= -1;
            }

            if (ball.X == this.Width)
            {
                player2Score += 1;
                ball.X = this.Width / 2;
                ball.Y = this.Height / 2;
                BALL_SPEEDX = 0;
                BALL_SPEEDY = 0;
                if (player2Score == gameWinScore)
                {
                    GameOver("Player 2 wins");
                }


            }
            if (ball.X == 0)
            {
                player1Score += 1;
                ball.X = this.Width / 2;
                ball.Y = this.Height / 2;
                BALL_SPEEDX = 0;
                BALL_SPEEDY = 0;
                if (player1Score == gameWinScore)
                {
                    GameOver("Player 1 wins");
                }
            }
            plaery2ScoreLabel.Text = $"{player2Score}";
            player1ScoreLabel.Text = $"{player1Score}";

            if (player1.IntersectsWith(ball) || player2.IntersectsWith(ball))
            {
                balls.Add(new Rectangle(this.Width / 2, this.Height / 2, BALL_WIDTH, BALL_HEIGHT));  
            }


                //refresh the screen, which causes the Form1_Paint method to run
                this.Refresh();
        }

        /// <summary>
        /// </summary>
        /// <param name="winner">The player name to be shown as the winner</param>
        private void GameOver(string winner)
        {
            newGameOk = true;
            startLabel.Visible = true;
            gameUpdateLoop.Stop();
            winnerLabel.Visible = true;
            winnerLabel.Text = winner;
            // TODO create game over logic
            // --- stop the gameUpdateLoop
            // --- show a message on the startLabel to indicate a winner, (may need to Refresh).
            // --- use the startLabel to ask the user if they want to play again

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // TODO draw player2 using FillRectangle
            e.Graphics.FillRectangle(blueBrush, player1);
            e.Graphics.FillRectangle(redBrush, player2);
            e.Graphics.FillRectangle(whiteBrush, ball);

            for (int i = 0; i < balls.Count(); i++)
            {
                e.Graphics.FillRectangle(whiteBrush, balls[i]);

            }

        }
    }
}
