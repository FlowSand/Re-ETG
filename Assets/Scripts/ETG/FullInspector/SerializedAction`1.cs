using FullInspector.Modules.SerializableDelegates;

#nullable disable
namespace FullInspector
{
  public class SerializedAction<TParam1> : BaseSerializedAction
  {
    public void Invoke(TParam1 param1) => this.DoInvoke((object) param1);
  }
}
