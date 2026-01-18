using FullInspector.Internal;
using System;
using System.Collections.Generic;

#nullable disable
namespace FullInspector
{
  public class fiGraphMetadata
  {
    private Dictionary<string, List<object>> _precomputedData;
    [ShowInInspector]
    private CullableDictionary<int, fiGraphMetadata, IntDictionary<fiGraphMetadata>> _childrenInt;
    [ShowInInspector]
    private CullableDictionary<string, fiGraphMetadata, Dictionary<string, fiGraphMetadata>> _childrenString;
    [ShowInInspector]
    private CullableDictionary<System.Type, object, Dictionary<System.Type, object>> _metadata;
    private fiGraphMetadata _parentMetadata;
    private fiUnityObjectReference _targetObject;
    private string _accessPath;

    public fiGraphMetadata()
      : this((fiUnityObjectReference) null)
    {
    }

    public fiGraphMetadata(fiUnityObjectReference targetObject)
      : this((fiGraphMetadata) null, string.Empty)
    {
      this._targetObject = targetObject;
    }

    private fiGraphMetadata(fiGraphMetadata parentMetadata, string accessKey)
    {
      this._childrenInt = new CullableDictionary<int, fiGraphMetadata, IntDictionary<fiGraphMetadata>>();
      this._childrenString = new CullableDictionary<string, fiGraphMetadata, Dictionary<string, fiGraphMetadata>>();
      this._metadata = new CullableDictionary<System.Type, object, Dictionary<System.Type, object>>();
      this._parentMetadata = parentMetadata;
      this._precomputedData = this._parentMetadata != null ? this._parentMetadata._precomputedData : new Dictionary<string, List<object>>();
      this.RebuildAccessPath(accessKey);
      if (!this._precomputedData.ContainsKey(this._accessPath))
        return;
      foreach (object obj in this._precomputedData[this._accessPath])
        this._metadata[obj.GetType()] = obj;
    }

    public bool ShouldSerialize() => !this._childrenInt.IsEmpty || !this._childrenString.IsEmpty;

    public void Serialize<TPersistentData>(out string[] keys_, out TPersistentData[] values_) where TPersistentData : IGraphMetadataItemPersistent
    {
      List<string> keys = new List<string>();
      List<TPersistentData> values = new List<TPersistentData>();
      this.AddSerializeData<TPersistentData>(keys, values);
      keys_ = keys.ToArray();
      values_ = values.ToArray();
    }

    private void AddSerializeData<TPersistentData>(List<string> keys, List<TPersistentData> values) where TPersistentData : IGraphMetadataItemPersistent
    {
      foreach (KeyValuePair<System.Type, object> keyValuePair in this._metadata.Items)
      {
        if (keyValuePair.Key == typeof (TPersistentData) && ((IGraphMetadataItemPersistent) keyValuePair.Value).ShouldSerialize())
        {
          keys.Add(this._accessPath);
          values.Add((TPersistentData) keyValuePair.Value);
        }
      }
      foreach (KeyValuePair<int, fiGraphMetadata> keyValuePair in this._childrenInt.Items)
        keyValuePair.Value.AddSerializeData<TPersistentData>(keys, values);
      foreach (KeyValuePair<string, fiGraphMetadata> keyValuePair in this._childrenString.Items)
        keyValuePair.Value.AddSerializeData<TPersistentData>(keys, values);
    }

    public void Deserialize<TPersistentData>(string[] keys, TPersistentData[] values)
    {
      for (int index = 0; index < keys.Length; ++index)
      {
        string key = keys[index];
        List<object> objectList;
        if (!this._precomputedData.TryGetValue(key, out objectList))
        {
          objectList = new List<object>();
          this._precomputedData[key] = objectList;
        }
        objectList.Add((object) values[index]);
      }
    }

    public void BeginCullZone()
    {
      this._childrenInt.BeginCullZone();
      this._childrenString.BeginCullZone();
      this._metadata.BeginCullZone();
    }

    public void EndCullZone()
    {
      this._childrenInt.EndCullZone();
      this._childrenString.EndCullZone();
      this._metadata.EndCullZone();
    }

    private UnityEngine.Object TargetObject
    {
      get
      {
        if (this._targetObject != null && this._targetObject.IsValid)
          return this._targetObject.Target;
        return this._parentMetadata != null ? this._parentMetadata.TargetObject : (UnityEngine.Object) null;
      }
    }

    public string Path => this._accessPath;

    private void RebuildAccessPath(string accessKey)
    {
      this._accessPath = string.Empty;
      if (this._parentMetadata != null && !string.IsNullOrEmpty(this._parentMetadata._accessPath))
      {
        fiGraphMetadata fiGraphMetadata = this;
        fiGraphMetadata._accessPath = $"{fiGraphMetadata._accessPath}{this._parentMetadata._accessPath}.";
      }
      this._accessPath += accessKey;
    }

    public void SetChild(int identifier, fiGraphMetadata metadata)
    {
      this._childrenInt[identifier] = metadata;
      metadata.RebuildAccessPath(identifier.ToString());
    }

    public void SetChild(string identifier, fiGraphMetadata metadata)
    {
      this._childrenString[identifier] = metadata;
      metadata.RebuildAccessPath(identifier);
    }

    public static void MigrateMetadata<T>(fiGraphMetadata metadata, T[] previous, T[] updated)
    {
      List<fiGraphMetadata.MetadataMigration> neededMigrations = fiGraphMetadata.ComputeNeededMigrations<T>(metadata, previous, updated);
      string[] strArray1 = new string[neededMigrations.Count];
      string[] strArray2 = new string[neededMigrations.Count];
      for (int index = 0; index < neededMigrations.Count; ++index)
      {
        strArray1[index] = metadata.Enter(neededMigrations[index].OldIndex).Metadata._accessPath;
        strArray2[index] = metadata.Enter(neededMigrations[index].NewIndex).Metadata._accessPath;
      }
      List<fiGraphMetadata> fiGraphMetadataList = new List<fiGraphMetadata>(neededMigrations.Count);
      for (int key = 0; key < neededMigrations.Count; ++key)
        fiGraphMetadataList.Add(metadata._childrenInt[key]);
      for (int index = 0; index < neededMigrations.Count; ++index)
        metadata._childrenInt[neededMigrations[index].NewIndex] = fiGraphMetadataList[index];
    }

    private static List<fiGraphMetadata.MetadataMigration> ComputeNeededMigrations<T>(
      fiGraphMetadata metadata,
      T[] previous,
      T[] updated)
    {
      List<fiGraphMetadata.MetadataMigration> neededMigrations = new List<fiGraphMetadata.MetadataMigration>();
      for (int index = 0; index < updated.Length; ++index)
      {
        int num = Array.IndexOf<T>(previous, updated[index]);
        if (num != -1 && num != index)
          neededMigrations.Add(new fiGraphMetadata.MetadataMigration()
          {
            NewIndex = index,
            OldIndex = num
          });
      }
      return neededMigrations;
    }

    public fiGraphMetadataChild Enter(int childIdentifier)
    {
      fiGraphMetadata fiGraphMetadata;
      if (!this._childrenInt.TryGetValue(childIdentifier, out fiGraphMetadata))
      {
        fiGraphMetadata = new fiGraphMetadata(this, childIdentifier.ToString());
        this._childrenInt[childIdentifier] = fiGraphMetadata;
      }
      return new fiGraphMetadataChild()
      {
        Metadata = fiGraphMetadata
      };
    }

    public fiGraphMetadataChild Enter(string childIdentifier)
    {
      fiGraphMetadata fiGraphMetadata;
      if (!this._childrenString.TryGetValue(childIdentifier, out fiGraphMetadata))
      {
        fiGraphMetadata = new fiGraphMetadata(this, childIdentifier);
        this._childrenString[childIdentifier] = fiGraphMetadata;
      }
      return new fiGraphMetadataChild()
      {
        Metadata = fiGraphMetadata
      };
    }

    public T GetPersistentMetadata<T>() where T : IGraphMetadataItemPersistent, new()
    {
      return this.GetPersistentMetadata<T>(out bool _);
    }

    public T GetPersistentMetadata<T>(out bool wasCreated) where T : IGraphMetadataItemPersistent, new()
    {
      return this.GetCommonMetadata<T>(out wasCreated);
    }

    public T GetMetadata<T>() where T : IGraphMetadataItemNotPersistent, new()
    {
      return this.GetMetadata<T>(out bool _);
    }

    public T GetMetadata<T>(out bool wasCreated) where T : IGraphMetadataItemNotPersistent, new()
    {
      return this.GetCommonMetadata<T>(out wasCreated);
    }

    private T GetCommonMetadata<T>(out bool wasCreated) where T : new()
    {
      object commonMetadata;
      if (!this._metadata.TryGetValue(typeof (T), out commonMetadata))
      {
        commonMetadata = (object) new T();
        this._metadata[typeof (T)] = commonMetadata;
        wasCreated = true;
      }
      else
        wasCreated = false;
      return (T) commonMetadata;
    }

    public T GetInheritedMetadata<T>() where T : IGraphMetadataItemNotPersistent, new()
    {
      object inheritedMetadata;
      if (this._metadata.TryGetValue(typeof (T), out inheritedMetadata))
        return (T) inheritedMetadata;
      return this._parentMetadata == null ? this.GetMetadata<T>() : this._parentMetadata.GetInheritedMetadata<T>();
    }

    public bool TryGetMetadata<T>(out T metadata) where T : IGraphMetadataItemNotPersistent, new()
    {
      object obj;
      bool metadata1 = this._metadata.TryGetValue(typeof (T), out obj);
      metadata = (T) obj;
      return metadata1;
    }

    public bool TryGetInheritedMetadata<T>(out T metadata) where T : IGraphMetadataItemNotPersistent, new()
    {
      object obj;
      if (this._metadata.TryGetValue(typeof (T), out obj))
      {
        metadata = (T) obj;
        return true;
      }
      if (this._parentMetadata != null)
        return this._parentMetadata.TryGetInheritedMetadata<T>(out metadata);
      metadata = default (T);
      return false;
    }

    public struct MetadataMigration
    {
      public int NewIndex;
      public int OldIndex;
    }
  }
}
