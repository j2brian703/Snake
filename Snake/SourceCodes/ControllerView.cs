using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Snake
{
    public class ControllerView : UIView
    {
        public delegate void ControllerDelegate ();

        public event ControllerDelegate UpEvent;
        public event ControllerDelegate DownEvent;
        public event ControllerDelegate LeftEvent;
        public event ControllerDelegate RightEvent;

        public ControllerView(RectangleF frame) : base(frame)
        {
            // controls
            UIImageView controlBG = new UIImageView(UIImage.FromBundle("Assets/Controller/bg.png"));
            controlBG.Frame = new RectangleF(0, 0, 300, 300);
            this.AddSubview(controlBG);

            // upButton
            UIButton upButton = SnakeAppearance.GenerateButton();
            upButton.Frame = new RectangleF(frame.Width / 2 - 25, 40, upButton.Frame.Width, upButton.Frame.Height);
            upButton.SetImage(UIImage.FromBundle("Assets/Controller/up.png"), UIControlState.Normal);
            upButton.TouchUpInside += HandleUpTouchUpInside;
            this.AddSubview(upButton);

            // downButton
            UIButton downButton = SnakeAppearance.GenerateButton();
            downButton.Frame = new RectangleF(frame.Width / 2 - 25, (frame.Height / 2) - 50, downButton.Frame.Width, downButton.Frame.Height);
            downButton.SetImage(UIImage.FromBundle("Assets/Controller/down.png"), UIControlState.Normal);
            downButton.TouchUpInside += HandleDownTouchUpInside;
            this.AddSubview(downButton);

            // leftButton
            UIButton leftButton = SnakeAppearance.GenerateButton();
            leftButton.Frame = new RectangleF(40, (frame.Height / 2) - 50, leftButton.Frame.Width, leftButton.Frame.Height);
            leftButton.SetImage(UIImage.FromBundle("Assets/Controller/left.png"), UIControlState.Normal);
            leftButton.TouchUpInside += HandleLeftTouchUpInside;
            this.AddSubview(leftButton);

            // rightButton
            UIButton rightButton = SnakeAppearance.GenerateButton();
            rightButton.Frame = new RectangleF(frame.Width - 40 - 50, (frame.Height / 2) - 50, leftButton.Frame.Width, leftButton.Frame.Height);
            rightButton.SetImage(UIImage.FromBundle("Assets/Controller/right.png"), UIControlState.Normal);
            rightButton.TouchUpInside += HandleRightTouchUpInside;
            this.AddSubview(rightButton);
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
}

