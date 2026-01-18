#nullable disable
namespace FullInspector
{
    public class SharedInstance<TInstance, TSerializer> : BaseScriptableObject<TSerializer> where TSerializer : BaseSerializer
    {
        public TInstance Instance;
    }
}
