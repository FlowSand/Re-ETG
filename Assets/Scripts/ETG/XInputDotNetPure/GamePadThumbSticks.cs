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
