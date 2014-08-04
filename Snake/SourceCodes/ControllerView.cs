using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Snake
{
    public class ControllerView : UIView
    {
        private ControlerCounterView clockView;
        private ControlerCounterView tailView;

        public delegate void ControllerDelegate ();

        public event ControllerDelegate UpEvent;
        public event ControllerDelegate DownEvent;
        public event ControllerDelegate LeftEvent;
        public event ControllerDelegate RightEvent;

        public ControllerView(RectangleF frame) : base(frame)
        {
            // controlBG
            UIImageView controlBG = new UIImageView(UIImage.FromBundle("Assets/bg_controller.png"));
            controlBG.Frame = new RectangleF(0, 0, frame.Width, frame.Height);
            this.AddSubview(controlBG);

            // clockView
            clockView = new ControlerCounterView(new RectangleF(0, 0, frame.Width / 2, 40), "Time", "00:00:00");
            clockView.Frame = new RectangleF(frame.Width / 2 - (clockView.Frame.Width / 2) - 130, (frame.Height / 2) - (clockView.Frame.Height / 2), clockView.Frame.Width, clockView.Frame.Height);
            this.AddSubview(clockView);

            // tailView
            tailView = new ControlerCounterView(new RectangleF(0, 0, frame.Width / 2, 40), "Tail Count", "0");
            tailView.Frame = new RectangleF(frame.Width / 2 + (tailView.Frame.Width / 2) - 100, (frame.Height / 2) - (tailView.Frame.Height / 2), clockView.Frame.Width, clockView.Frame.Height);
            this.AddSubview(tailView);

            // upButton
            UIButton upButton = SnakeAppearance.GenerateButton();
            upButton.BackgroundColor = UIColor.Clear;
            upButton.Frame = new RectangleF(frame.Width / 2 - 40, 20, 30, 30);
            upButton.ShowsTouchWhenHighlighted = true;
            upButton.TouchUpInside += HandleUpTouchUpInside;
            this.AddSubview(upButton);

            // downButton
            UIButton downButton = SnakeAppearance.GenerateButton();
            downButton.BackgroundColor = UIColor.Clear;
            downButton.Frame = new RectangleF(upButton.Frame.X, upButton.Frame.Bottom + upButton.Frame.Height + 5, upButton.Frame.Width, upButton.Frame.Height);
            downButton.ShowsTouchWhenHighlighted = true;
            downButton.TouchUpInside += HandleDownTouchUpInside;
            this.AddSubview(downButton);

            // leftButton
            UIButton leftButton = SnakeAppearance.GenerateButton();
            leftButton.BackgroundColor = UIColor.Clear;
            leftButton.Frame = new RectangleF(upButton.Frame.X - upButton.Frame.Width - 3, upButton.Frame.Bottom + 2, upButton.Frame.Width, upButton.Frame.Height);
            leftButton.ShowsTouchWhenHighlighted = true;
            leftButton.TouchUpInside += HandleLeftTouchUpInside;
            this.AddSubview(leftButton);

            // rightButton
            UIButton rightButton = SnakeAppearance.GenerateButton();
            rightButton.BackgroundColor = UIColor.Clear;
            rightButton.Frame = new RectangleF(upButton.Frame.Right + 3, leftButton.Frame.Y, upButton.Frame.Width, upButton.Frame.Height);
            rightButton.ShowsTouchWhenHighlighted = true;
            rightButton.TouchUpInside += HandleRightTouchUpInside;
            this.AddSubview(rightButton);
        }

        public void UpdatePlayClock(String time)
        {
            clockView.Label.Text = time;
        }

        public void UpdateTailCount(Int32 count)
        {
            tailView.Label.Text = count.ToString();
        }

        void HandleUpTouchUpInside(object sender, EventArgs e)
        {
            if (UpEvent != null)
            {
                UpEvent();
            }
        }

        void HandleDownTouchUpInside(object sender, EventArgs e)
        {
            if (DownEvent != null)
            {
                DownEvent();
            }
        }

        void HandleLeftTouchUpInside(object sender, EventArgs e)
        {
            if (LeftEvent != null)
            {
                LeftEvent();
            }
        }

        void HandleRightTouchUpInside(object sender, EventArgs e)
        {
            if (RightEvent != null)
            {
                RightEvent();
            }
        }
    }

    public class ControlerCounterView : UIView
    {
        public UILabel Label { get; set; }

        public ControlerCounterView(RectangleF frame, String text0, String text1) : base(frame)
        {
            // menuLabel
            UILabel menuLabel = SnakeAppearance.GenerateLabel();
            menuLabel.Frame = new RectangleF(0, 0, frame.Width, frame.Height);
            menuLabel.Text = text0;
            menuLabel.SizeToFit();
            this.AddSubview(menuLabel);

            // Label
            Label = SnakeAppearance.GenerateLabel(30);
            Label.Frame = new RectangleF(menuLabel.Frame.Right + 20, menuLabel.Frame.Y, frame.Width - menuLabel.Frame.Right, 1);
            Label.Text = text1;
            Label.SizeToFit();
            this.AddSubview(Label);

            menuLabel.Frame = new RectangleF(menuLabel.Frame.X, (Label.Frame.Height / 2) - (menuLabel.Frame.Height / 2), menuLabel.Frame.Width, menuLabel.Frame.Height);

            this.SizeToFit();
        }
    }
}

