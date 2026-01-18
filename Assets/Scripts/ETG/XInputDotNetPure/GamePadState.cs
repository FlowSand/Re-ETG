#nullable disable
namespace XInputDotNetPure
{
  public struct GamePadState
  {
    private bool isConnected;
    private uint packetNumber;
    private GamePadButtons buttons;
    private GamePadDPad dPad;
    private GamePadThumbSticks thumbSticks;
    private GamePadTriggers triggers;

    internal GamePadState(bool isConnected, GamePadState.RawState rawState)
    {
      this.isConnected = isConnected;
      if (!isConnected)
      {
        rawState.dwPacketNumber = 0U;
        rawState.Gamepad.dwButtons = (ushort) 0;
        rawState.Gamepad.bLeftTrigger = (byte) 0;
        rawState.Gamepad.bRightTrigger = (byte) 0;
        rawState.Gamepad.sThumbLX = (short) 0;
        rawState.Gamepad.sThumbLY = (short) 0;
        rawState.Gamepad.sThumbRX = (short) 0;
        rawState.Gamepad.sThumbRY = (short) 0;
      }
      this.packetNumber = rawState.dwPacketNumber;
      this.buttons = new GamePadButtons(((int) rawState.Gamepad.dwButtons & 16 /*0x10*/) == 0 ? ButtonState.Released : ButtonState.Pressed, ((int) rawState.Gamepad.dwButtons & 32 /*0x20*/) == 0 ? ButtonState.Released : ButtonState.Pressed, ((int) rawState.Gamepad.dwButtons & 64 /*0x40*/) == 0 ? ButtonState.Released : ButtonState.Pressed, ((int) rawState.Gamepad.dwButtons & 128 /*0x80*/) == 0 ? ButtonState.Released : ButtonState.Pressed, ((int) rawState.Gamepad.dwButtons & 256 /*0x0100*/) == 0 ? ButtonState.Released : ButtonState.Pressed, ((int) rawState.Gamepad.dwButtons & 512 /*0x0200*/) == 0 ? ButtonState.Released : ButtonState.Pressed, ((int) rawState.Gamepad.dwButtons & 4096 /*0x1000*/) == 0 ? ButtonState.Released : ButtonState.Pressed, ((int) rawState.Gamepad.dwButtons & 8192 /*0x2000*/) == 0 ? ButtonState.Released : ButtonState.Pressed, ((int) rawState.Gamepad.dwButtons & 16384 /*0x4000*/) == 0 ? ButtonState.Released : ButtonState.Pressed, ((int) rawState.Gamepad.dwButtons & 32768 /*0x8000*/) == 0 ? ButtonState.Released : ButtonState.Pressed);
      this.dPad = new GamePadDPad(((int) rawState.Gamepad.dwButtons & 1) == 0 ? ButtonState.Released : ButtonState.Pressed, ((int) rawState.Gamepad.dwButtons & 2) == 0 ? ButtonState.Released : ButtonState.Pressed, ((int) rawState.Gamepad.dwButtons & 4) == 0 ? ButtonState.Released : ButtonState.Pressed, ((int) rawState.Gamepad.dwButtons & 8) == 0 ? ButtonState.Released : ButtonState.Pressed);
      this.thumbSticks = new GamePadThumbSticks(new GamePadThumbSticks.StickValue((float) rawState.Gamepad.sThumbLX / (float) short.MaxValue, (float) rawState.Gamepad.sThumbLY / (float) short.MaxValue), new GamePadThumbSticks.StickValue((float) rawState.Gamepad.sThumbRX / (float) short.MaxValue, (float) rawState.Gamepad.sThumbRY / (float) short.MaxValue));
      this.triggers = new GamePadTriggers((float) rawState.Gamepad.bLeftTrigger / (float) byte.MaxValue, (float) rawState.Gamepad.bRightTrigger / (float) byte.MaxValue);
    }

    public uint PacketNumber => this.packetNumber;

    public bool IsConnected => this.isConnected;

    public GamePadButtons Buttons => this.buttons;

    public GamePadDPad DPad => this.dPad;

    public GamePadTriggers Triggers => this.triggers;

    public GamePadThumbSticks ThumbSticks => this.thumbSticks;

    internal struct RawState
    {
      public uint dwPacketNumber;
      public GamePadState.RawState.GamePad Gamepad;

      public struct GamePad
      {
        public ushort dwButtons;
        public byte bLeftTrigger;
        public byte bRightTrigger;
        public short sThumbLX;
        public short sThumbLY;
        public short sThumbRX;
        public short sThumbRY;
      }
    }

    private enum ButtonsConstants
    {
      DPadUp = 1,
      DPadDown = 2,
      DPadLeft = 4,
      DPadRight = 8,
      Start = 16, // 0x00000010
      Back = 32, // 0x00000020
      LeftThumb = 64, // 0x00000040
      RightThumb = 128, // 0x00000080
      LeftShoulder = 256, // 0x00000100
      RightShoulder = 512, // 0x00000200
      A = 4096, // 0x00001000
      B = 8192, // 0x00002000
      X = 16384, // 0x00004000
      Y = 32768, // 0x00008000
    }
  }
}
