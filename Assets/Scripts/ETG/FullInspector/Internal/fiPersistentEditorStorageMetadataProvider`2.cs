// Decompiled with JetBrains decompiler
// Type: FullInspector.Internal.fiPersistentEditorStorageMetadataProvider`2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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
