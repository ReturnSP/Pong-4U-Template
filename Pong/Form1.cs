/*
 * Description:     A basic PONG simulator
 * Author:           Saahil Patel
 * Date:            2024-02-06
 */

#region libraries

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Media;
using System.Security.AccessControl;
using System.Drawing.Drawing2D;
using System.Reflection;

#endregion

namespace Pong
{
    public partial class Form1 : Form
    {
        #region global values

        //graphics objects for drawing
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        SolidBrush greyBrush = new SolidBrush(Color.Gray);
        SolidBrush blackBrush = new SolidBrush(Color.Black);
        SolidBrush redBrush = new SolidBrush(Color.Red);
        SolidBrush deepPinkBrush = new SolidBrush(Color.DeepPink);
        SolidBrush purpleBrush = new SolidBrush(Color.Purple);
        Pen drawPen = new Pen(Color.DeepPink, 5);
        Pen purpleDrawPen = new Pen(Color.Purple, 3);
        Pen largerPurpleDrawPen = new Pen(Color.Purple, 2);
        Font drawFont = new Font("Courier New", 10);

        // Sounds for game
        SoundPlayer scoreSound = new SoundPlayer(Properties.Resources.score);
        SoundPlayer collisionSound = new SoundPlayer(Properties.Resources.collision);

        //determines whether a key is being pressed or not
        Boolean wKeyDown, sKeyDown, upKeyDown, downKeyDown, dKeyDown, aKeyDown, yKeyDown, leftKeyDown, rightKeyDown;

        // check to see if a new game can be started
        Boolean newGameOk = true;

        //ball values
        Boolean ballMoveRight = true;
        Boolean ballMoveDown = true;
        double ball_Speed = 4;
        const int BALL_WIDTH = 10;
        const int BALL_HEIGHT = 10;
        Rectangle ball;

        Region reg;
        Region temp1;
        Region temp2;

        int secondZoneMultiplier;
        const int GRAVITRON_WIDTH = 15;
        const int GRAVITRON_HEIGHT = 15;
        int Gravitron_X;
        int Gravitron_Y;
        Rectangle Gravitron;
        Random gravitron_rand = new Random();
        Random GravityZoneMultipler = new Random();

        //player values
        const int PADDLE_SPEED = 12;
        const int PADDLE_EDGE = 20;  // buffer distance between screen edge and paddle            
        const int PADDLE_WIDTH = 10;
        const int PADDLE_HEIGHT = 40;
        Rectangle player1, player2;

        //player and game scoresz
        int player1Score = 0;
        int player2Score = 0;
        int gameWinScore = 2;  // number of points needed to win game

        #endregion

        public Form1()
        {
            InitializeComponent();
        }
        ////TO DO LIST
        //Track a unsername bar for player 1 and player 2
        //Let players move left and right
        //Spawn powerups which give different abilities
        private void randGravitron()
        {
            int Gravitron_X = gravitron_rand.Next((this.Width / 2) - 50, (this.Width / 2) + 50 + 1);
            
            int Gravitron_Y = gravitron_rand.Next(0, this.Height + 1);

            Gravitron = new Rectangle(Gravitron_X, Gravitron_Y, GRAVITRON_WIDTH, GRAVITRON_HEIGHT);
        }

        private GraphicsPath gravitronPath()
        {
            GraphicsPath grav = new GraphicsPath();
            grav.AddEllipse(Gravitron);
            return grav;
        }
        private void gravityEffect()
        {
            //double radius = Math.Sqrt(Math.Pow(Math.Abs(ball.X - Gravitron_X), 2) + Math.Pow(Math.Abs(ball.Y - Gravitron_Y), 2));
            //double effect = 0.05;

            //double gravitationalEffect = effect / radius;
            //ball_Speed *= gravitationalEffect;

            ///OLD GRAVITY CODE
            //int xSubtraction = ball.X - Gravitron.X;
            //if (xSubtraction != 0)
            //{
            //    double slope = ((ball.Y - Gravitron.Y) / (xSubtraction)); //(y2-y1)/(x2-x1)
            //    if (slope != 0)
            //    {
            //        double multiplier = 0.02147 / 9.8;
            //        slope *= multiplier;
            //        if (slope >= 1)
            //        {
            //            ball_Speed *= Convert.ToInt32(slope / 0.000213);

            //        }
            //    }
            //}
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
                case Keys.D:
                    dKeyDown = true;
                    break;
                case Keys.A:
                    aKeyDown = true;
                    break;
                case Keys.Left:
                    leftKeyDown = true;
                    break;
                case Keys.Right:
                    rightKeyDown = true;
                    break;
                case Keys.Y:
                    yKeyDown = true;
                    break;
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
                case Keys.D:
                    dKeyDown = false;
                    break;
                case Keys.A:
                    aKeyDown = false;
                    break;
                case Keys.Left:
                    leftKeyDown = false;
                    break;
                case Keys.Right:
                    rightKeyDown = false;
                    break;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// sets the ball and paddle positions for game start
        /// </summary>
        /// 
        private void SetParameters()
        {
            if (newGameOk)
            {
                player1Score = player2Score = 0;
                newGameOk = false;
                startLabel.Visible = false;
                player1ScoreLabel.Text = $"{player1Score}";
                player2ScoreLabel.Text = $"{player2Score}";
                gameUpdateLoop.Start();
            }

            //player start positions
            player1 = new Rectangle(PADDLE_EDGE, this.Height / 2 - PADDLE_HEIGHT / 2, PADDLE_WIDTH, PADDLE_HEIGHT);
            player2 = new Rectangle(this.Width - PADDLE_EDGE - PADDLE_WIDTH, this.Height / 2 - PADDLE_HEIGHT / 2, PADDLE_WIDTH, PADDLE_HEIGHT);

            // TODO create a ball rectangle in the middle of screen
            ball = new Rectangle(this.Width / 2 - BALL_WIDTH / 2, this.Height / 2, BALL_WIDTH, BALL_HEIGHT);
        }

        /// <summary>
        /// This method is the game engine loop that updates the position of all elements
        /// and checks for collisions.
        /// </summary>
        private void gameUpdateLoop_Tick(object sender, EventArgs e)
        {
            #region update ball position
            gravityEffect();
            // TODO create code to move ball either left or right based on ballMoveRight and using BALL_SPEED
            if (ballMoveRight == true)
            {
                ball.X += Convert.ToInt32(ball_Speed);
            }
            else if (ballMoveRight == false)
            {
                ball.X -= Convert.ToInt32(ball_Speed);
            }

            // TODO create code move ball either down or up based on ballMoveDown and using BALL_SPEED
            if (ballMoveDown == true)
            {
                ball.Y += Convert.ToInt32(ball_Speed);
            }
            else if (ballMoveDown == false)
            {
                ball.Y -= Convert.ToInt32(ball_Speed);
            }
            #endregion

            #region update paddle positions

            if (wKeyDown == true && player1.Y > 0)
            {
                // TODO create code to move player 1 up
                player1.Y -= PADDLE_SPEED;
            }

            // TODO create an if statement and code to move player 1 down 
            if (sKeyDown == true && player1.Y < this.Height - PADDLE_HEIGHT)
            {
                player1.Y += PADDLE_SPEED;
            }
            // TODO create an if statement and code to move player 2 up
            if (upKeyDown == true && player2.Y > 0)
            {
                // TODO create code to move player 2 up
                player2.Y -= PADDLE_SPEED;
            }
            // TODO create an if statement and code to move player 2 down
            if (downKeyDown == true && player2.Y < this.Height - PADDLE_HEIGHT)
            {
                player2.Y += PADDLE_SPEED;
            }

            //Side to side movement
            if (aKeyDown == true)
            {
                player1.X -= PADDLE_SPEED;
            }

            if (dKeyDown == true)
            {
                player1.X += PADDLE_SPEED;
            }
            if (leftKeyDown == true)
            {
                player2.X -= PADDLE_SPEED;
            }

            if (rightKeyDown == true)
            {
                player2.X += PADDLE_SPEED;
            }


            //teleportation movement
            if (player1.Y <= 0 || player1.Y > this.Height - PADDLE_HEIGHT)
            {
                player1.Y = this.Height / 2 - PADDLE_HEIGHT / 2;
                player1.X = PADDLE_EDGE + 20;
            }
            if (player1.X <= 0 || player1.X > this.Width / 2 - PADDLE_WIDTH)
            {
                player1.Y = this.Height / 2 - PADDLE_HEIGHT / 2;
                player1.X = PADDLE_EDGE + 20;
            }
            if (player2.Y <= 0 || player2.Y > this.Height - PADDLE_HEIGHT)
            {
                player2.Y = this.Height / 2;
                player2.X = this.Width - 20;
            }
            if (player2.X >= this.Width - PADDLE_WIDTH / 2 || player2.X <= this.Width / 2)
            {
                player2.Y = this.Height / 2;
                player2.X = this.Width - 20;
            }


            #endregion

            #region ball collision with top and bottom lines

            if (ball.Y < 0) // if ball hits top line
            {
                // TODO use ballMoveDown boolean to change direction
                ballMoveDown = true;

                // TODO play a collision sound
                collisionSound.Play();
            }
            // TODO In an else if statement check for collision with bottom line
            // If true use ballMoveDown boolean to change direction
            else if (ball.Y > this.Height - BALL_HEIGHT)
            {
                ballMoveDown = false;
                collisionSound.Play();
            }

            #endregion

            #region ball collision with paddles

            // TODO create if statment that checks if player1 collides with ball and if it does
            // --- play a "paddle hit" sound and
            // --- use ballMoveRight boolean to change direction


            //if (player1.IntersectsWith(ball))
            //{
            //    collisionSound.Play();
            //    ballMoveRight = true;
            //}


            // TODO create if statment that checks if player2 collides with ball and if it does
            // --- play a "paddle hit" sound and
            // --- use ballMoveRight boolean to change direction


            //if (player2.IntersectsWith(ball))
            //{
            //    collisionSound.Play();
            //    ballMoveRight = false;
            //}


            /*  ENRICHMENT
             *  Instead of using two if statments as noted above see if you can create one
             *  if statement with multiple conditions to play a sound and change direction
             */
            if (player1.IntersectsWith(ball) || player2.IntersectsWith(ball))
            {
                collisionSound.Play();
                ballMoveRight = !ballMoveRight; //Switches the state of ball movement
                randGravitron();
            }
            if (ball.IntersectsWith(Gravitron))
            {
                randGravitron();
            }

            #endregion

            #region ball collision with side walls (point scored)

            if (ball.X < 0 + BALL_WIDTH)  // ball hits left wall logic
            {
                // TODO
                // --- play score sound
                // --- update player 2 score and display it to the label
                scoreSound.Play();
                player2Score += 1;
                player2ScoreLabel.Text = $"{player2Score}";
                // TODO use if statement to check to see if player 2 has won the game. If true run 
                // GameOver() method. Else change direction of ball and call SetParameters() method.
                if (player2Score == 3)
                {
                    GameOver("player2");
                }
                else
                {
                    ballMoveRight = !ballMoveRight;
                    ballMoveDown = !ballMoveDown;
                    SetParameters();
                }
            }
            // TODO same as above but this time check for collision with the right wall
            if (ball.X > this.Width - BALL_WIDTH)
            {
                scoreSound.Play();
                player1Score += 1;
                player1ScoreLabel.Text = $"{player1Score}";
                if (player1Score == 3)
                {
                    GameOver("player1");
                }
                else
                {
                    ballMoveRight = !ballMoveRight;
                    ballMoveDown = !ballMoveDown;
                    SetParameters();
                }
            }
            #endregion

            //refresh the screen, which causes the Form1_Paint method to run
            this.Refresh();
        }

        /// <summary>
        /// Displays a message for the winner when the game is over and allows the user to either select
        /// to play again or end the program
        /// </summary>
        /// <param name="winner">The player name to be shown as the winner</param>
        private void GameOver(string winner)
        {
            newGameOk = true;
            // TODO create game over logic
            // --- stop the gameUpdateLoop
            gameUpdateLoop.Stop();
            // --- show a message on the startLabel to indicate a winner, (may need to Refresh).
            Refresh();
            ball_Speed = 4;
            startLabel.Visible = true;
            startLabel.Text = $"The winner is {winner}";
            Refresh();
            Thread.Sleep(5000);
            // --- use the startLabel to ask the user if they want to play again
            startLabel.Text = "Would you like to play again?";
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // TODO draw player2 using FillRectangle
            e.Graphics.FillRectangle(greyBrush, player1);
            e.Graphics.FillRectangle(greyBrush, player2);

            // TODO draw ball using FillRectangle
            e.Graphics.FillRectangle(blackBrush, ball);
            e.Graphics.FillRectangle(redBrush, Gravitron);
            e.Graphics.DrawLine(drawPen, ball.X, ball.Y, Gravitron.X, Gravitron.Y);
            e.Graphics.DrawEllipse(purpleDrawPen, Gravitron);
            int zoneMultiplier = GravityZoneMultipler.Next(10, 23);
            int secondZoneMultiplier = GravityZoneMultipler.Next(10, 23);
            
            e.Graphics.DrawEllipse(purpleDrawPen, Gravitron.X - zoneMultiplier * GRAVITRON_WIDTH, Gravitron.Y - zoneMultiplier * GRAVITRON_HEIGHT, GRAVITRON_WIDTH + 350, GRAVITRON_HEIGHT + 350);
            e.Graphics.DrawEllipse(largerPurpleDrawPen, Gravitron.X - secondZoneMultiplier * GRAVITRON_WIDTH, Gravitron.Y - secondZoneMultiplier * GRAVITRON_HEIGHT, GRAVITRON_WIDTH + 400, GRAVITRON_HEIGHT + 400);

            //Gravitron Regions
            GraphicsPath grav = gravitronPath();
            GraphicsPath grp = new GraphicsPath();
            grp.AddEllipse(Gravitron.X - zoneMultiplier * GRAVITRON_WIDTH, Gravitron.Y - zoneMultiplier * GRAVITRON_HEIGHT, GRAVITRON_WIDTH + 350, GRAVITRON_HEIGHT + 350);
            grp.AddEllipse(Gravitron.X - secondZoneMultiplier * GRAVITRON_WIDTH, Gravitron.Y - secondZoneMultiplier * GRAVITRON_HEIGHT, GRAVITRON_WIDTH + 400, GRAVITRON_HEIGHT + 400);
            grp.CloseFigure();
            Region reg = new Region(grp);
            Region temp1 = new Region(grp);
            Region temp2 = new Region(grav);

            if (ball.X <= Gravitron.X - zoneMultiplier * GRAVITRON_WIDTH + GRAVITRON_WIDTH + 350 && ball.Y >= Gravitron.Y - zoneMultiplier * GRAVITRON_HEIGHT)
            {
                ball.Y += 6;
                ball_Speed += 1;
                ball.X += 1;
                //e.Graphics.FillEllipse(greyBrush, Gravitron.X - zoneMultiplier * GRAVITRON_WIDTH, Gravitron.Y - zoneMultiplier * GRAVITRON_HEIGHT, GRAVITRON_WIDTH + 350, GRAVITRON_HEIGHT + 350);
            }
            else
            {
                ball_Speed = 4;
            }

            //Second Ellipse thingy
            if (ball.X <= Gravitron.X - zoneMultiplier * GRAVITRON_WIDTH + GRAVITRON_WIDTH + 350 && ball.Y >= Gravitron.Y - zoneMultiplier * GRAVITRON_HEIGHT)
            {
                ball.Y -= 6;
                ball.X -= 5;
                ball_Speed -= 1;
                //e.Graphics.FillEllipse(greyBrush, Gravitron.X - secondZoneMultiplier * GRAVITRON_WIDTH, Gravitron.Y - secondZoneMultiplier * GRAVITRON_HEIGHT, GRAVITRON_WIDTH + 400, GRAVITRON_HEIGHT + 400);
            }
            else
            {
                ball_Speed = 4;
            }


        }
    }
}


    

