using FullSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
namespace FullInspector.Internal
{
  public class fiPersistentEditorStorage
  {
    private static Dictionary<System.Type, System.Type> _cachedRealComponentTypes = new Dictionary<System.Type, System.Type>();
    private const string SceneStorageName = "fiPersistentEditorStorage";
    private static GameObject _cachedSceneStorage;

    public static void Reset<T>(UnityEngine.Object key_)
    {
      fiUnityObjectReference unityObjectReference = new fiUnityObjectReference(key_);
      fiBaseStorageComponent<T> unityObject = !fiLateBindings.EditorUtility.IsPersistent(unityObjectReference.Target) ? fiPersistentEditorStorage.GetStorageDictionary<T>(fiPersistentEditorStorage.SceneStorage) : fiPersistentEditorStorage.GetStorageDictionary<T>(fiPersistentEditorStorage.SceneStorage);
      unityObject.Data.Remove(unityObjectReference.Target);
      fiLateBindings.EditorUtility.SetDirty((UnityEngine.Object) unityObject);
    }

    public static T Read<T>(UnityEngine.Object key_) where T : new()
    {
      fiUnityObjectReference unityObjectReference = new fiUnityObjectReference(key_);
      fiBaseStorageComponent<T> unityObject = !fiLateBindings.EditorUtility.IsPersistent(unityObjectReference.Target) ? fiPersistentEditorStorage.GetStorageDictionary<T>(fiPersistentEditorStorage.SceneStorage) : fiPersistentEditorStorage.GetStorageDictionary<T>(fiPersistentEditorStorage.SceneStorage);
      if (unityObject.Data.ContainsKey(unityObjectReference.Target))
        return unityObject.Data[unityObjectReference.Target];
      T obj1 = new T();
      unityObject.Data[unityObjectReference.Target] = obj1;
      T obj2 = obj1;
      fiLateBindings.EditorUtility.SetDirty((UnityEngine.Object) unityObject);
      return obj2;
    }

    private static fiBaseStorageComponent<T> GetStorageDictionary<T>(GameObject container)
    {
      System.Type type;
      if (!fiPersistentEditorStorage._cachedRealComponentTypes.TryGetValue(typeof (fiBaseStorageComponent<T>), out type))
      {
        type = fiRuntimeReflectionUtility.AllSimpleTypesDerivingFrom(typeof (fiBaseStorageComponent<T>)).FirstOrDefault<System.Type>();
        fiPersistentEditorStorage._cachedRealComponentTypes[typeof (fiBaseStorageComponent<T>)] = type;
      }
      Component storageDictionary = type != null ? container.GetComponent(type) : throw new InvalidOperationException("Unable to find derived component type for " + typeof (fiBaseStorageComponent<T>).CSharpName());
      if ((UnityEngine.Object) storageDictionary == (UnityEngine.Object) null)
        storageDictionary = container.AddComponent(type);
      return (fiBaseStorageComponent<T>) storageDictionary;
    }

    public static GameObject SceneStorage
    {
      get
      {
        if ((UnityEngine.Object) fiPersistentEditorStorage._cachedSceneStorage == (UnityEngine.Object) null)
        {
          fiPersistentEditorStorage._cachedSceneStorage = GameObject.Find(nameof (fiPersistentEditorStorage));
          if ((UnityEngine.Object) fiPersistentEditorStorage._cachedSceneStorage == (UnityEngine.Object) null)
            fiPersistentEditorStorage._cachedSceneStorage = fiLateBindings.EditorUtility.CreateGameObjectWithHideFlags(nameof (fiPersistentEditorStorage), HideFlags.HideInHierarchy);
        }
        return fiPersistentEditorStorage._cachedSceneStorage;
      }
    }
  }
}
