// Decompiled with JetBrains decompiler
// Type: FullInspector.fiPersistentMetadata
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector.Internal;
using FullSerializer;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
namespace FullInspector;

public static class fiPersistentMetadata
{
  private static readonly fiIPersistentMetadataProvider[] s_providers;
  private static Dictionary<fiUnityObjectReference, fiGraphMetadata> s_metadata = new Dictionary<fiUnityObjectReference, fiGraphMetadata>();

  static fiPersistentMetadata()
  {
    fiPersistentMetadata.s_providers = fiRuntimeReflectionUtility.GetAssemblyInstances<fiIPersistentMetadataProvider>().ToArray<fiIPersistentMetadataProvider>();
    for (int index = 0; index < fiPersistentMetadata.s_providers.Length; ++index)
      fiLog.Log((object) typeof (fiPersistentMetadata), "Using provider {0} to support metadata of type {1}", (object) fiPersistentMetadata.s_providers[index].GetType().CSharpName(), (object) fiPersistentMetadata.s_providers[index].MetadataType.CSharpName());
  }

  public static fiGraphMetadata GetMetadataFor(Object target_)
  {
    fiUnityObjectReference unityObjectReference = new fiUnityObjectReference(target_);
    fiGraphMetadata metadataFor;
    if (!fiPersistentMetadata.s_metadata.TryGetValue(unityObjectReference, out metadataFor))
    {
      metadataFor = new fiGraphMetadata(unityObjectReference);
      fiPersistentMetadata.s_metadata[unityObjectReference] = metadataFor;
      for (int index = 0; index < fiPersistentMetadata.s_providers.Length; ++index)
        fiPersistentMetadata.s_providers[index].RestoreData(unityObjectReference.Target);
    }
    return metadataFor;
  }

  public static void Reset(Object target_)
  {
    fiUnityObjectReference key = new fiUnityObjectReference(target_);
    if (!fiPersistentMetadata.s_metadata.ContainsKey(key))
      return;
    fiPersistentMetadata.s_metadata.Remove(key);
    for (int index = 0; index < fiPersistentMetadata.s_providers.Length; ++index)
      fiPersistentMetadata.s_providers[index].Reset(key.Target);
  }
}
