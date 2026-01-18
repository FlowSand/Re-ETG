using FullInspector.Modules.SerializableDelegates;

#nullable disable
namespace FullInspector
{
    public class SerializedAction : BaseSerializedAction
    {
        public void Invoke() => this.DoInvoke((object[]) null);
    }
}
