#nullable disable
namespace XInputDotNetPure
{
    public struct GamePadDPad
    {
        private ButtonState up;
        private ButtonState down;
        private ButtonState left;
        private ButtonState right;

        internal GamePadDPad(ButtonState up, ButtonState down, ButtonState left, ButtonState right)
        {
            this.up = up;
            this.down = down;
            this.left = left;
            this.right = right;
        }

        public ButtonState Up => this.up;

        public ButtonState Down => this.down;

        public ButtonState Left => this.left;

        public ButtonState Right => this.right;
    }
}
