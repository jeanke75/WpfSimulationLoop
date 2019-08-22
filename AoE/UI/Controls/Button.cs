using DrawingBase.Input;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AoE.UI.Controls
{
    enum ButtonState
    {
        Normal,
        OnMouseHover,
        OnMouseClick,
        Disabled
    }

    internal class Button
    {
        private readonly ImageSource imageSourceNormal;
        private readonly ImageSource imageSourceMouseHover;
        private readonly ImageSource imageSourceMouseClick;
        private readonly ImageSource imageSourceDisabled;
        public readonly Rect rect;

        private ButtonState state;

        public bool Enabled { get; set; }

        public Button(double x, double y, double width, double height, string imageId)
        {
            BitmapImage image = new BitmapImage(new Uri("pack://application:,,,/Images/UI/" + imageId));
            imageSourceNormal = new CroppedBitmap(image, new Int32Rect(0, 0, (int)(image.Width / 4), (int)image.Height));
            imageSourceMouseHover = new CroppedBitmap(image, new Int32Rect((int)(image.Width / 4), 0, (int)(image.Width / 4), (int)image.Height));
            imageSourceMouseClick = new CroppedBitmap(image, new Int32Rect(2 * (int)(image.Width / 4), 0, (int)(image.Width / 4), (int)image.Height));
            imageSourceDisabled = new CroppedBitmap(image, new Int32Rect(3 * (int)(image.Width / 4), 0, (int)(image.Width / 4), (int)image.Height));

            rect = new Rect(x, y, width, height);

            Enabled = true;
        }

        public void Update()
        {
            if (Enabled)
            {
                var mousePos = InputHelper.Mouse.GetPosition();
                bool mouseOver = mousePos.X >= rect.Left && mousePos.X <= rect.Right && mousePos.Y >= rect.Top && mousePos.Y <= rect.Bottom;
                if (mouseOver)
                {
                    var clicked = (InputHelper.Mouse.GetState(MouseButton.Left) == DrawingBase.Input.ButtonState.Pressed || InputHelper.Mouse.GetState(MouseButton.Left) == DrawingBase.Input.ButtonState.Down);
                    state = clicked ? ButtonState.OnMouseClick : ButtonState.OnMouseHover;
                }
                else
                    state = ButtonState.Normal;
            }
            else
            {
                state = ButtonState.Disabled;
            }
        }

        public void Draw(DrawingContext dc)
        {
            // Draw background
            switch (state)
            {
                case ButtonState.Normal:
                    dc.DrawImage(imageSourceNormal, rect);
                    break;
                case ButtonState.OnMouseHover:
                    dc.DrawImage(imageSourceMouseHover, rect);
                    break;
                case ButtonState.OnMouseClick:
                    dc.DrawImage(imageSourceMouseClick, rect);
                    break;
                case ButtonState.Disabled:
                    dc.DrawImage(imageSourceDisabled, rect);
                    break;
            }
        }

        public bool Clicked()
        {
            return state == ButtonState.OnMouseClick;
        }
    }
}