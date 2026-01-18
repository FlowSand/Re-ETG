using FullInspector.Modules.SerializableDelegates;

#nullable disable
namespace FullInspector
{
  public class SerializedFunc<TParam1, TParam2, TParam3, TParam4, TResult> : BaseSerializedFunc
  {
    public TResult Invoke(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
    {
      return (TResult) this.DoInvoke((object) param1, (object) param2, (object) param3, (object) param4);
    }
  }
}
