using FullInspector.Modules.SerializableDelegates;

#nullable disable
namespace FullInspector
{
    public class SerializedFunc<TResult> : BaseSerializedFunc
    {
        public TResult Invoke() => (TResult) this.DoInvoke((object[]) null);
    }
}
