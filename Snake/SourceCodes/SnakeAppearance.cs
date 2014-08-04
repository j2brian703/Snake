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
            button.Frame = new RectangleF(0, 0, 30, 30);

            return button;
        }

        public static UILabel GenerateLabel(float fontSize = 21f)
        {
            UILabel label = new UILabel();
            label.Font = ThemeFont(fontSize);

            if (fontSize > 21)
            {
                label.TextColor = SnakeAppearance.Color("eb4751");
            }
            else
            {
                label.TextColor = SnakeAppearance.Color("69545b");
            }

            return label;
        }

        public static UIFont ThemeFont(float size)
        {
            return UIFont.FromName("Museo-300", size);
        }

        public static UIColor Color(float r, float g, float b)
        {
            return new UIColor(r / 255f, g / 255f, b / 255f, 1f);
        }

        public static UIColor Color(String hexColor)
        {
            Int32 rgb = 0;
            Int32.TryParse(hexColor, NumberStyles.AllowHexSpecifier, null, out rgb);
            Int32 r = (rgb & 0xff0000) >> 16;
            Int32 g = (rgb & 0xff00) >> 8;
            Int32 b = (rgb & 0xff);

            return Color(r, g, b);
        }
    }
}