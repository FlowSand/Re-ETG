#nullable disable
namespace Dungeonator
{
  public enum PathFinderNodeType
  {
    Start = 1,
    End = 2,
    Open = 4,
    Close = 8,
    Current = 16, // 0x00000010
    Path = 32, // 0x00000020
  }
}
