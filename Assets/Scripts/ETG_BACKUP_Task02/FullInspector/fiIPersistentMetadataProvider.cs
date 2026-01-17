// Decompiled with JetBrains decompiler
// Type: FullInspector.fiIPersistentMetadataProvider
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace FullInspector;

public interface fiIPersistentMetadataProvider
{
  void RestoreData(UnityEngine.Object target);

  void Reset(UnityEngine.Object target);

  System.Type MetadataType { get; }
}
