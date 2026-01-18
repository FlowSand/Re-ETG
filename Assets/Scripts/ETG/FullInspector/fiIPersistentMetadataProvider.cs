#nullable disable
namespace FullInspector
{
    public interface fiIPersistentMetadataProvider
    {
        void RestoreData(UnityEngine.Object target);

        void Reset(UnityEngine.Object target);

        System.Type MetadataType { get; }
    }
}
