using FullInspector.Modules.SerializableDelegates;

#nullable disable
namespace FullInspector
{
  public class SerializedFunc<TParam1, TResult> : BaseSerializedFunc
  {
    public TResult Invoke(TParam1 param1) => (TResult) this.DoInvoke((object) param1);
  }
}
