// Decompiled with JetBrains decompiler
// Type: XInputDotNetPure.GamePadButtons
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace XInputDotNetPure
{
  public struct GamePadButtons
  {
    private ButtonState start;
    private ButtonState back;
    private ButtonState leftStick;
    private ButtonState rightStick;
    private ButtonState leftShoulder;
    private ButtonState rightShoulder;
    private ButtonState a;
    private ButtonState b;
    private ButtonState x;
    private ButtonState y;

    internal GamePadButtons(
      ButtonState start,
      ButtonState back,
      ButtonState leftStick,
      ButtonState rightStick,
      ButtonState leftShoulder,
      ButtonState rightShoulder,
      ButtonState a,
      ButtonState b,
      ButtonState x,
      ButtonState y)
    {
      this.start = start;
      this.back = back;
      this.leftStick = leftStick;
      this.rightStick = rightStick;
      this.leftShoulder = leftShoulder;
      this.rightShoulder = rightShoulder;
      this.a = a;
      this.b = b;
      this.x = x;
      this.y = y;
    }

    public ButtonState Start => this.start;

    public ButtonState Back => this.back;

    public ButtonState LeftStick => this.leftStick;

    public ButtonState RightStick => this.rightStick;

    public ButtonState LeftShoulder => this.leftShoulder;

    public ButtonState RightShoulder => this.rightShoulder;

    public ButtonState A => this.a;

    public ButtonState B => this.b;

    public ButtonState X => this.x;

    public ButtonState Y => this.y;
  }
}
