using System;
using System.Drawing;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Snake
{
    public partial class SnakeViewController : UIViewController
    {
        private UIButton startButton;
        private ControllerView controllerView;
        private UIImageView target;
        private UIImageView head;
        private UILabel playClockLabel;
        private List<UIImageView> tailList;
        private NSTimer gameTimer;
        private NSTimer playTimer;
        private Random rand;
        private Int32 w_offset;
        private Int32 h_offset;
        private Int32 clock = 0;

        private static Int32 GAME_SPEED = 200;
        // Lower the faster

        public SnakeViewController()
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();
			
            // Release any cached data, images, etc that aren't in use.
        }

        #region View lifecycle

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			
            // Perform any additional setup after loading the view, typically from a nib.
            this.View.BackgroundColor = UIColor.FromPatternImage(UIImage.FromBundle("Assets/Background/bg.png"));

            w_offset = 20;
            h_offset = 0;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            InitializeControls();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
        }

        public override UIStatusBarStyle PreferredStatusBarStyle()
        {
            return UIStatusBarStyle.LightContent;
        }

        #endregion

        #region Initializations

        private void InitializeControls()
        {
            rand = new Random();

            if (playClockLabel == null)
            {
                playClockLabel = SnakeAppearance.GenerateLabel();
                playClockLabel.Frame = new RectangleF(0, 20, this.View.Frame.Width, 40);
                this.View.AddSubview(playClockLabel);
            }
            playClockLabel.Text = "00:00:00";

            // controller
            InitializeController();

            // startButton
            if (startButton == null)
            {
                startButton = SnakeAppearance.GenerateButton();
                startButton.Frame = new RectangleF(this.View.Center.X - 150, this.View.Center.Y - 50, 300, 100);
                startButton.BackgroundColor = UIColor.Gray;
                startButton.TouchUpInside += HandleStartTouchUpInside;
                this.View.AddSubview(startButton);
            }
            startButton.SetTitle("Start", UIControlState.Normal);

            // tailList
            if (tailList == null)
            {
                tailList = new List<UIImageView>();
            }
            else
            {
                tailList.Clear();
            }

            // head
            InitializeHead();

            // target
            InitializeTarget();
        }

        private void InitializeController()
        {
            controllerView = new ControllerView(new RectangleF(this.View.Center.X - 150, this.View.Frame.Bottom - 150, 300, 300));
            controllerView.UpEvent += HandleUpEvent;
            controllerView.DownEvent += HandleDownEvent;
            controllerView.LeftEvent += HandleLeftEvent;
            controllerView.RightEvent += HandleRightEvent;
            controllerView.UserInteractionEnabled = false;
            this.View.AddSubview(controllerView);
        }

        private void InitializeHead()
        {
            if (head == null)
            {
                head = new UIImageView(UIImage.FromBundle("Assets/snake.png"));
                head.Frame = new RectangleF(100, 250, 20, 20);
                this.View.AddSubview(head);
            }

            PointF randomPoint = GenerateNewPoint(head);
            head.Frame = new RectangleF(randomPoint.X, randomPoint.Y, head.Frame.Width, head.Frame.Height);
        }

        private void InitializeTarget()
        {
            if (target == null)
            {
                target = new UIImageView(UIImage.FromBundle("Assets/target.png"));
                target.Frame = new RectangleF(0, 0, 20, 20);
                this.View.AddSubview(target);
            }

            PointF randomPoint = GenerateNewPoint(target);
            target.Frame = new RectangleF(randomPoint.X, randomPoint.Y, target.Frame.Width, target.Frame.Height);
        }

        private void GameOver()
        {
            // Reset controls
            InitializeControls();

            // Stop timers
            playTimer.Invalidate();
            gameTimer.Invalidate();

            // Show start button again
            startButton.SetTitle("Press again to play", UIControlState.Normal);
            startButton.Hidden = false;
        }

        #endregion

        #region Controller

        void HandleUpEvent()
        {
            w_offset = 0;
            h_offset = -20;
        }

        void HandleDownEvent()
        {
            w_offset = 0;
            h_offset = 20;
        }

        void HandleLeftEvent()
        {
            w_offset = -20;
            h_offset = 0;
        }

        void HandleRightEvent()
        {
            w_offset = 20;
            h_offset = 0;
        }

        #endregion

        #region Snake Movement

        private void MoveSnake()
        {
            PointF orgin = head.Center;
            head.Center = new PointF(orgin.X + w_offset, orgin.Y + h_offset);

            for (Int32 i = tailList.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    tailList[i].Center = orgin;
                }
                else
                {
                    tailList[i].Center = new PointF(tailList[i - 1].Center.X, tailList[i - 1].Center.Y);
                }
            }

            // Check if the snake touched the target
            if (head.Frame.IntersectsWith(target.Frame))
            {
                InitializeTarget();

                AddTail();
            }
            // Check if the snake touched its body or went out of screen
            else if (SnakeTouchedItself || OutOfBound(head.Frame))
            {
                GameOver();
            }
        }

        private bool SnakeTouchedItself
        {
            get
            {
                for (Int32 i = 0; i < tailList.Count; i++)
                {
                    if (tailList[i].PointInside(head.Center, null))
                    {
                        return true;
                    }
                }

                return false;
            }
//            CGPoint point = head.center;
//            if ([tailArray count] > 3) {
//                for (int i = 3; i < [tailArray count]; i++) {
//                    UIImageView * tail = [tailArray objectAtIndex:i];
//                    CGRect rect = CGRectMake(tail.frame.origin.x, tail.frame.origin.y, tail.frame.size.w_offset, tail.frame.size.h_offset);
//                    if ([self isPointInRect:point rect:rect]) {
//                        return YES;
//                    }
//                }
//            }
//
//            return NO;
        }

        private bool OutOfBound(RectangleF frame)
        {
            // Left edge
            if (frame.X <= 0)
            {
                return true;
            }
            // Right edge
            else if (frame.X + frame.Width >= this.View.Frame.Width)
            {
                return true;
            }
            // Top edge
            else if (frame.Y <= 20)
            {
                return true;
            }
            // Bottom controller edge
            else if (frame.Y + frame.Height >= controllerView.Frame.Y)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Others

        private PointF GenerateNewPoint(UIImageView imageView)
        {
            Int32 x = rand.Next(0, (Int32)this.View.Frame.Width);
            Int32 y = rand.Next(64, (Int32)(controllerView.Frame.Y));
            PointF randomPoint = new PointF(x, y);

            RectangleF tempHeadFrame = new RectangleF(randomPoint.X, randomPoint.Y, imageView.Frame.Width, imageView.Frame.Height);
            if (tempHeadFrame.IntersectsWith(startButton.Frame) || OutOfBound(tempHeadFrame))
            {
                return GenerateNewPoint(imageView);
            }

            return randomPoint;
        }

        private void AddTail()
        {
            UIImageView tail = new UIImageView(UIImage.FromBundle("Assets/snake.png"));
            if (tailList.Count == 0)
            {
                tail.Frame = new RectangleF(head.Frame.X - 20, head.Frame.Y, 20, 20);
            }
            else
            {
                UIImageView lastTail = tailList[tailList.Count - 1];
                tail.Center = lastTail.Center;
            }
            this.View.AddSubview(tail);

            // Add to the list
            tailList.Add(tail);
        }

        private void UpdatePlayClock()
        {
            clock++;

            TimeSpan span = TimeSpan.FromSeconds(clock);
            if (span.Hours > 0)
            {
                playClockLabel.Text = span.Hours.ToString("00:") + span.Minutes.ToString("00:") + span.Seconds.ToString("00");
            }
            else if (span.Minutes > 0)
            {
                playClockLabel.Text = "00:" + span.Minutes.ToString("00:") + span.Seconds.ToString("00");
            }
            else
            {
                playClockLabel.Text = "00:00:" + span.Seconds.ToString("00");
            }

            playClockLabel.SetNeedsDisplay();
        }

        void HandleStartTouchUpInside(object sender, EventArgs e)
        {
            if (!startButton.Hidden)
            {
                startButton.Hidden = true;
            }

            controllerView.UserInteractionEnabled = true;

            playTimer = NSTimer.CreateRepeatingScheduledTimer(new TimeSpan(0, 0, 1), UpdatePlayClock);
            gameTimer = NSTimer.CreateRepeatingScheduledTimer(new TimeSpan(0, 0, 0, 0, GAME_SPEED), MoveSnake);
        }

        #endregion
    }
}

