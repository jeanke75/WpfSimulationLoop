namespace DrawingBase.Input
{
    public enum ButtonState
    {
        Up,
        Pressed,
        Released,
        Down,
    }

    public static class InputHelper
    {
        public static readonly MouseHelper Mouse = new MouseHelper();
        public static readonly KeyboardHelper Keyboard = new KeyboardHelper();

        public static void Update()
        {
            Mouse.Update();
            Keyboard.Update();
        }
    }
}