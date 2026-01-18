#nullable disable
namespace FullInspector.Internal
{
  public abstract class fiPersistentEditorStorageMetadataProvider<TItem, TStorage> : 
    fiIPersistentMetadataProvider
    where TItem : new()
    where TStorage : fiIGraphMetadataStorage, new()
  {
    public void RestoreData(UnityEngine.Object target)
    {
      fiPersistentEditorStorage.Read<TStorage>(target).RestoreData(target);
    }

    public void Reset(UnityEngine.Object target) => fiPersistentEditorStorage.Reset<TStorage>(target);

    public System.Type MetadataType => typeof (TItem);
  }
}
