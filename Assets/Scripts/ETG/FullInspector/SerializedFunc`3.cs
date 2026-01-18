using FullInspector.Modules.SerializableDelegates;

#nullable disable
namespace FullInspector
{
    public class SerializedFunc<TParam1, TParam2, TResult> : BaseSerializedFunc
    {
        public TResult Invoke(TParam1 param1, TParam2 param2)
        {
            return (TResult) this.DoInvoke((object) param1, (object) param2);
        }
    }
}
