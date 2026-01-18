#nullable disable
namespace XInputDotNetPure
{
  public struct GamePadTriggers
  {
    private float left;
    private float right;

    internal GamePadTriggers(float left, float right)
    {
      this.left = left;
      this.right = right;
    }

    public float Left => this.left;

    public float Right => this.right;
  }
}
