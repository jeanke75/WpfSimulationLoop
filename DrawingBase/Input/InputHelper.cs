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

        public static void Update()
        {
            Mouse.Update();
        }
    }
}