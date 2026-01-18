#nullable disable
namespace FullInspector.Internal
{
  public static class fiOption
  {
    public static fiOption<T> Just<T>(T value) => new fiOption<T>(value);
  }
}
