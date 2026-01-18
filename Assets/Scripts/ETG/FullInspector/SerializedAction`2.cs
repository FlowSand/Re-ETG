using FullInspector.Modules.SerializableDelegates;

#nullable disable
namespace FullInspector
{
    public class SerializedAction<TParam1, TParam2> : BaseSerializedAction
    {
        public void Invoke(TParam1 param1, TParam2 param2)
        {
            this.DoInvoke((object) param1, (object) param2);
        }
    }
}
