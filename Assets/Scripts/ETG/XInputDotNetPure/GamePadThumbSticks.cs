// Decompiled with JetBrains decompiler
// Type: XInputDotNetPure.GamePadThumbSticks
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace XInputDotNetPure
{
  public struct GamePadThumbSticks
  {
    private GamePadThumbSticks.StickValue left;
    private GamePadThumbSticks.StickValue right;

    internal GamePadThumbSticks(
      GamePadThumbSticks.StickValue left,
      GamePadThumbSticks.StickValue right)
    {
      this.left = left;
      this.right = right;
    }

    public GamePadThumbSticks.StickValue Left => this.left;

    public GamePadThumbSticks.StickValue Right => this.right;

    public struct StickValue
    {
      private Vector2 vector;

      internal StickValue(float x, float y) => this.vector = new Vector2(x, y);

      public float X => this.vector.x;

      public float Y => this.vector.y;

      public Vector2 Vector => this.vector;
    }
  }
}
