using FullInspector.Internal;

#nullable disable
namespace FullInspector
{
  public abstract class fiValueNullSerializer<T> : fiValueProxyEditor, fiIValueProxyAPI
  {
    public T Value;

    object fiIValueProxyAPI.Value
    {
      get => (object) this.Value;
      set => this.Value = (T) value;
    }

    void fiIValueProxyAPI.SaveState()
    {
    }

    void fiIValueProxyAPI.LoadState()
    {
    }
  }
}
