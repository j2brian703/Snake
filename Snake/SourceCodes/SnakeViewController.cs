using System;
using System.Drawing;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;

namespace Snake
{
    public partial class SnakeViewController : UIViewController
    {
        private UIButton startButton;
        private UIView border;
        private ControllerView controllerView;
        private UIView target;
        private UIView head;
        private List<UIView> tailList;
        private NSTimer gameTimer;
        private NSTimer playTimer;
        private Random rand;
        private Int32 clock = 0;
        private DIRECTION direction;

        private static Int32 TAIL_SIZE = 20;
        private static Int32 GAME_SPEED = 150; // Lower the faster

        public enum DIRECTION
        {
            Up,
            Down,
            Left,
            Right
        }

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
            this.View.BackgroundColor = UIColor.FromPatternImage(UIImage.FromBundle("Assets/bg.png"));
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            // controller
            InitializeController();

            // border
            border = new UIView(new RectangleF(12, 20, 1000, 600));
            border.Tag = 100;
            border.BackgroundColor = UIColor.Clear;
            border.Layer.BorderColor = UIColor.White.CGColor;
            border.Layer.BorderWidth = 1f;
            this.View.AddSubview(border);

            // Initialize other controls
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
            direction = DIRECTION.Right;
            clock = 0;

            // Init startButton
            UIImage startImg = UIImage.FromBundle("Assets/start.png");
            startButton = SnakeAppearance.GenerateButton();
            startButton.Frame = new RectangleF(this.View.Center.X - (startImg.Size.Width / 2), border.Center.Y - (startImg.Size.Height / 2), startImg.Size.Width, startImg.Size.Height);
            startButton.SetImage(UIImage.FromBundle("Assets/start.png"), UIControlState.Normal);
            startButton.TouchUpInside += HandleStartTouchUpInside;
            this.View.AddSubview(startButton);

            // Init tailList
            if (tailList == null)
            {
                tailList = new List<UIView>();
            }
            else
            {
                tailList.Clear();
            }

            // Init controllerView
            controllerView.UpdateTailCount(0);
            controllerView.UpdatePlayClock("00:00:00");
            controllerView.UserInteractionEnabled = false;

            // Init head
            InitializeHead();

            // Init target
            InitializeTarget();
        }

        private void InitializeController()
        {
            controllerView = new ControllerView(new RectangleF(0, this.View.Frame.Bottom - 136, this.View.Frame.Width, 136));
            controllerView.Tag = 100;
            controllerView.UpEvent += HandleUpEvent;
            controllerView.DownEvent += HandleDownEvent;
            controllerView.LeftEvent += HandleLeftEvent;
            controllerView.RightEvent += HandleRightEvent;
            this.View.AddSubview(controllerView);
        }

        private void InitializeHead()
        {
            head = new UIView(new RectangleF(0, 0, TAIL_SIZE, TAIL_SIZE));
            head.BackgroundColor = SnakeAppearance.Color("ee5c65");
            head.Layer.BorderColor = SnakeAppearance.Color("797272").CGColor;
            head.Layer.BorderWidth = 1.0f;
            this.View.AddSubview(head);

            PointF randomPoint = GenerateNewPoint(head);
            head.Frame = new RectangleF(randomPoint.X, randomPoint.Y, head.Frame.Width, head.Frame.Height);

            tailList.Add(head);
        }

        private void InitializeTarget()
        {
            if (target != null)
            {
                target.RemoveFromSuperview();
                target.Dispose();
                target = null;
            }

            target = new UIView(new RectangleF(0, 0, TAIL_SIZE, TAIL_SIZE));
            target.BackgroundColor = SnakeAppearance.Color("505050");
            target.Layer.BorderColor = SnakeAppearance.Color("797272").CGColor;
            target.Layer.BorderWidth = 1.0f;
            this.View.AddSubview(target);

            PointF randomPoint = GenerateNewPoint(target);
            target.Frame = new RectangleF(randomPoint.X, randomPoint.Y, target.Frame.Width, target.Frame.Height);
        }

        private void GameOver()
        {
            foreach (UIView subview in this.View)
            {
                if (subview.Tag != 100)
                {
                    subview.RemoveFromSuperview();
                    subview.Dispose();
                }
            }

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
            // If tail count is more than 1, do not change to opposite direction
            if (direction == DIRECTION.Down && tailList.Count > 1)
            {
                return;
            }

            direction = DIRECTION.Up;
        }

        void HandleDownEvent()
        {
            // If tail count is more than 1, do not change to opposite direction
            if (direction == DIRECTION.Up && tailList.Count > 1)
            {
                return;
            }

            direction = DIRECTION.Down;
        }

        void HandleLeftEvent()
        {
            // If tail count is more than 1, do not change to opposite direction
            if (direction == DIRECTION.Right && tailList.Count > 1)
            {
                return;
            }

            direction = DIRECTION.Left;
        }

        void HandleRightEvent()
        {
            // If tail count is more than 1, do not change to opposite direction
            if (direction == DIRECTION.Left && tailList.Count > 1)
            {
                return;
            }

            direction = DIRECTION.Right;
        }

        #endregion

        #region Snake Movement

        private void MoveSnake()
        {
            // Make sure to update tail position from the last tail and update head position last otherwise, tails won't show
            for (Int32 i = tailList.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    PointF newOffset = NewHeadOffset();
                    head.Center = new PointF(head.Center.X + newOffset.X, head.Center.Y + newOffset.Y);
                }
                else
                {
                    tailList[i].Center = new PointF(tailList[i - 1].Center.X, tailList[i - 1].Center.Y);
                }
            }

            // Check if the snake touched the target
            if (head.Frame.IntersectsWith(target.Frame))
            {
                // Place target in the random point
                InitializeTarget();

                // Add tail to snake
                AddTail();
            }
            // Check if the snake touched its body
            else if (SnakeTouchedItself)
            {
                GameOver();
            }
            else if (OutOfBound(head.Frame))
            {
                border.Layer.BorderColor = UIColor.Red.CGColor;
                border.Layer.BorderWidth = 2f;

                GameOver();
            }
        }

        private bool SnakeTouchedItself
        {
            get
            {
                for (Int32 i = 0; i < tailList.Count; i++)
                {
                    if (tailList[i] != head)
                    {
                        if (tailList[i].Frame.IntersectsWith(head.Frame))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        private bool OutOfBound(RectangleF frame)
        {
            // Left border edge
            if (frame.Left < border.Frame.Left)
            {
                return true;
            }
            // Right border edge
            else if (frame.Right > border.Frame.Right)
            {
                return true;
            }
            // Top border edge
            else if (frame.Top < border.Frame.Top)
            {
                return true;
            }
            // Bottom border edge
            else if (frame.Bottom > border.Frame.Bottom)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Others

        private PointF GenerateNewPoint(UIView view)
        {
            // New point must be divisible by TAIL_SIZE since the target has to be intersect with the snake at exact point
            Int32 x = rand.Next(1, (Int32)(controllerView.Frame.Width / TAIL_SIZE));
            Int32 y = rand.Next(1, (Int32)(controllerView.Frame.Y - 20) / TAIL_SIZE);
            PointF randomPoint = new PointF(12 + x * TAIL_SIZE, y * TAIL_SIZE);

            // If the tempFrame is out of bound or behind the startButton, generate it again
            RectangleF tempFrame = new RectangleF(randomPoint.X, randomPoint.Y, view.Frame.Width, view.Frame.Height);
            if (tempFrame.IntersectsWith(startButton.Frame) || OutOfBound(tempFrame))
            {
                return GenerateNewPoint(view);
            }

            return randomPoint;
        }

        private void AddTail()
        {
            // newTail
            UIView newTail = new UIView(new RectangleF(0, 0, TAIL_SIZE, TAIL_SIZE));
            newTail.BackgroundColor = SnakeAppearance.Color("f2edc6");
            newTail.Layer.BorderColor = SnakeAppearance.Color("797272").CGColor;
            newTail.Layer.BorderWidth = 1.0f;

            PointF newOffset = NewHeadOffset();
            UIView oldTail = tailList[tailList.Count - 1];
            newTail.Center = new PointF(oldTail.Center.X - newOffset.X, oldTail.Center.Y - newOffset.Y);
            this.View.AddSubview(newTail);

            // Add to the list
            tailList.Add(newTail);

            // Update count label
            controllerView.UpdateTailCount(tailList.Count - 1);
        }

        private PointF NewHeadOffset()
        {
            Int32 x_offset = 0;
            Int32 y_offset = 0;

            if (direction == DIRECTION.Up)
            {
                x_offset = 0;
                y_offset = -TAIL_SIZE;
            }
            else if (direction == DIRECTION.Down)
            {
                x_offset = 0;
                y_offset = TAIL_SIZE;
            }
            else if (direction == DIRECTION.Left)
            {
                x_offset = -TAIL_SIZE;
                y_offset = 0;
            }
            else if (direction == DIRECTION.Right)
            {
                x_offset = TAIL_SIZE;
                y_offset = 0;
            }

            return new PointF(x_offset, y_offset);
        }

        private void UpdatePlayClock()
        {
            if (controllerView != null)
            {
                clock++;

                TimeSpan span = TimeSpan.FromSeconds(clock);
                if (span.Hours > 0)
                {
                    controllerView.UpdatePlayClock(span.Hours.ToString("00:") + span.Minutes.ToString("00:") + span.Seconds.ToString("00"));
                }
                else if (span.Minutes > 0)
                {
                    controllerView.UpdatePlayClock("00:" + span.Minutes.ToString("00:") + span.Seconds.ToString("00"));
                }
                else
                {
                    controllerView.UpdatePlayClock("00:00:" + span.Seconds.ToString("00"));
                }
            }
        }

        void HandleStartTouchUpInside(object sender, EventArgs e)
        {
            border.Layer.BorderColor = UIColor.White.CGColor;
            border.Layer.BorderWidth = 1f;

            startButton.Hidden = true;

            controllerView.UserInteractionEnabled = true;

            playTimer = NSTimer.CreateRepeatingScheduledTimer(new TimeSpan(0, 0, 1), UpdatePlayClock);
            gameTimer = NSTimer.CreateRepeatingScheduledTimer(new TimeSpan(0, 0, 0, 0, GAME_SPEED), MoveSnake);
        }

        #endregion
    }
}

