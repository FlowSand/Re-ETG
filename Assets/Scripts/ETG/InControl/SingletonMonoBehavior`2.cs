using System;
using System.Reflection;
using UnityEngine;

#nullable disable
namespace InControl
{
  public abstract class SingletonMonoBehavior<T, P> : MonoBehaviour
    where T : MonoBehaviour
    where P : MonoBehaviour
  {
    private static T instance;
    private static bool hasInstance;
    private static object lockObject = new object();

    public static T Instance => SingletonMonoBehavior<T, P>.GetInstance();

    private static void CreateInstance()
    {
      GameObject gameObject;
      if (typeof (P) == typeof (MonoBehaviour))
      {
        gameObject = new GameObject();
        gameObject.name = typeof (T).Name;
      }
      else
      {
        P objectOfType = UnityEngine.Object.FindObjectOfType<P>();
        if ((bool) (UnityEngine.Object) objectOfType)
        {
          gameObject = objectOfType.gameObject;
        }
        else
        {
          Debug.LogError((object) ("Could not find object with required component " + typeof (P).Name));
          return;
        }
      }
      Debug.Log((object) ("Creating instance of singleton component " + typeof (T).Name));
      SingletonMonoBehavior<T, P>.instance = gameObject.AddComponent<T>();
      SingletonMonoBehavior<T, P>.hasInstance = true;
    }

    private static T GetInstance()
    {
      lock (SingletonMonoBehavior<T, P>.lockObject)
      {
        if (SingletonMonoBehavior<T, P>.hasInstance)
          return SingletonMonoBehavior<T, P>.instance;
        System.Type element = typeof (T);
        T[] objectsOfType = UnityEngine.Object.FindObjectsOfType<T>();
        if (objectsOfType.Length > 0)
        {
          SingletonMonoBehavior<T, P>.instance = objectsOfType[0];
          SingletonMonoBehavior<T, P>.hasInstance = true;
          if (objectsOfType.Length > 1)
          {
            Debug.LogWarning((object) $"Multiple instances of singleton {(object) element} found; destroying all but the first.");
            for (int index = 1; index < objectsOfType.Length; ++index)
              UnityEngine.Object.DestroyImmediate((UnityEngine.Object) objectsOfType[index].gameObject);
          }
          return SingletonMonoBehavior<T, P>.instance;
        }
        if (!(Attribute.GetCustomAttribute((MemberInfo) element, typeof (SingletonPrefabAttribute)) is SingletonPrefabAttribute customAttribute))
        {
          SingletonMonoBehavior<T, P>.CreateInstance();
        }
        else
        {
          string name = customAttribute.Name;
          GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(BraveResources.Load<GameObject>(name));
          if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
          {
            Debug.LogError((object) $"Could not find prefab {name} for singleton of type {(object) element}.");
            SingletonMonoBehavior<T, P>.CreateInstance();
          }
          else
          {
            gameObject.name = name;
            SingletonMonoBehavior<T, P>.instance = gameObject.GetComponent<T>();
            if ((UnityEngine.Object) SingletonMonoBehavior<T, P>.instance == (UnityEngine.Object) null)
            {
              Debug.LogWarning((object) $"There wasn't a component of type \"{(object) element}\" inside prefab \"{name}\"; creating one now.");
              SingletonMonoBehavior<T, P>.instance = gameObject.AddComponent<T>();
              SingletonMonoBehavior<T, P>.hasInstance = true;
            }
          }
        }
        return SingletonMonoBehavior<T, P>.instance;
      }
    }

    protected bool EnforceSingleton()
    {
      lock (SingletonMonoBehavior<T, P>.lockObject)
      {
        if (SingletonMonoBehavior<T, P>.hasInstance)
        {
          T[] objectsOfType = UnityEngine.Object.FindObjectsOfType<T>();
          for (int index = 0; index < objectsOfType.Length; ++index)
          {
            if (objectsOfType[index].GetInstanceID() != SingletonMonoBehavior<T, P>.instance.GetInstanceID())
              UnityEngine.Object.DestroyImmediate((UnityEngine.Object) objectsOfType[index].gameObject);
          }
        }
      }
      return this.GetInstanceID() == SingletonMonoBehavior<T, P>.Instance.GetInstanceID();
    }

    protected bool EnforceSingletonComponent()
    {
      lock (SingletonMonoBehavior<T, P>.lockObject)
      {
        if (SingletonMonoBehavior<T, P>.hasInstance)
        {
          if (this.GetInstanceID() != SingletonMonoBehavior<T, P>.instance.GetInstanceID())
          {
            UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this);
            return false;
          }
        }
      }
      return true;
    }

    private void OnDestroy() => SingletonMonoBehavior<T, P>.hasInstance = false;
  }
}
