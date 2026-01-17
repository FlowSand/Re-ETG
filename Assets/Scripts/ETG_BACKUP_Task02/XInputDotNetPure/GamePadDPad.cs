// Decompiled with JetBrains decompiler
// Type: XInputDotNetPure.GamePadDPad
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace XInputDotNetPure;

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
