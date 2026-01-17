// Decompiled with JetBrains decompiler
// Type: FullInspector.Internal.fiInstalledSerializerManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace FullInspector.Internal;

public static class fiInstalledSerializerManager
{
  public const string GeneratedTypeName = "fiLoadedSerializers";
  private static fiISerializerMetadata _defaultMetadata;

  static fiInstalledSerializerManager()
  {
    List<Type> typeList1 = new List<Type>();
    List<Type> typeList2 = new List<Type>();
    fiInstalledSerializerManager.LoadedMetadata = new List<fiISerializerMetadata>();
    fiILoadedSerializers serializers;
    if (fiInstalledSerializerManager.TryGetLoadedSerializerType(out serializers))
    {
      fiInstalledSerializerManager._defaultMetadata = fiInstalledSerializerManager.GetProvider(serializers.DefaultSerializerProvider);
      foreach (Type serializerProvider in serializers.AllLoadedSerializerProviders)
      {
        fiISerializerMetadata provider = fiInstalledSerializerManager.GetProvider(serializerProvider);
        fiInstalledSerializerManager.LoadedMetadata.Add(provider);
        typeList1.AddRange((IEnumerable<Type>) provider.SerializationOptInAnnotationTypes);
        typeList2.AddRange((IEnumerable<Type>) provider.SerializationOptOutAnnotationTypes);
      }
    }
    foreach (Type type in fiRuntimeReflectionUtility.AllSimpleTypesDerivingFrom(typeof (fiISerializerMetadata)))
    {
      fiISerializerMetadata provider = fiInstalledSerializerManager.GetProvider(type);
      fiInstalledSerializerManager.LoadedMetadata.Add(provider);
      typeList1.AddRange((IEnumerable<Type>) provider.SerializationOptInAnnotationTypes);
      typeList2.AddRange((IEnumerable<Type>) provider.SerializationOptOutAnnotationTypes);
    }
    fiInstalledSerializerManager.SerializationOptInAnnotations = typeList1.ToArray();
    fiInstalledSerializerManager.SerializationOptOutAnnotations = typeList2.ToArray();
  }

  private static fiISerializerMetadata GetProvider(Type type)
  {
    return (fiISerializerMetadata) Activator.CreateInstance(type);
  }

  public static bool TryGetLoadedSerializerType(out fiILoadedSerializers serializers)
  {
    string name = "FullInspector.Internal.fiLoadedSerializers";
    TypeCache.Reset();
    Type type = TypeCache.FindType(name);
    if (type == null)
    {
      serializers = (fiILoadedSerializers) null;
      return false;
    }
    serializers = (fiILoadedSerializers) Activator.CreateInstance(type);
    return true;
  }

  public static List<fiISerializerMetadata> LoadedMetadata { get; private set; }

  public static fiISerializerMetadata DefaultMetadata
  {
    get
    {
      return fiInstalledSerializerManager._defaultMetadata != null ? fiInstalledSerializerManager._defaultMetadata : throw new InvalidOperationException("Please register a default serializer. You should see a popup window on the next serialization reload.");
    }
  }

  public static bool IsLoaded(Guid serializerGuid)
  {
    if (fiInstalledSerializerManager.LoadedMetadata == null)
      return false;
    for (int index = 0; index < fiInstalledSerializerManager.LoadedMetadata.Count; ++index)
    {
      if (fiInstalledSerializerManager.LoadedMetadata[index].SerializerGuid == serializerGuid)
        return true;
    }
    return false;
  }

  public static bool HasDefault => fiInstalledSerializerManager._defaultMetadata != null;

  public static Type[] SerializationOptInAnnotations { get; private set; }

  public static Type[] SerializationOptOutAnnotations { get; private set; }
}
