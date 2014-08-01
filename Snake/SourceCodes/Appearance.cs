using System;
using System.Drawing;
using System.Globalization;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Snake
{
    public class SnakeAppearance
    {
        public static UIButton GenerateButton()
        {
            UIButton button = UIButton.FromType(UIButtonType.Custom);
            button.Frame = new RectangleF(0, 0, 50, 50);

            ApplyShadow(button);

            return button;
        }

        public static UILabel GenerateLabel()
        {
            UILabel label = new UILabel();
            label.Font = UIFont.BoldSystemFontOfSize(21f);
            label.TextColor = UIColor.Blue;
            label.TextAlignment = UITextAlignment.Center;

            ApplyShadow(label);

            return label;
        }

        public static void ApplyShadow(UIView view)
        {
            view.Layer.ShadowRadius = 2f;
            view.Layer.ShadowOpacity = 1f;
            view.Layer.MasksToBounds = false;
            view.Layer.ShadowOffset = new SizeF(1, 1);
        }
    }
}