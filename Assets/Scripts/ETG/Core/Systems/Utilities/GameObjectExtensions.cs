// Decompiled with JetBrains decompiler
// Type: GameObjectExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public static class GameObjectExtensions
    {
      public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
      {
        T orAddComponent = gameObject.GetComponent<T>();
        if ((UnityEngine.Object) orAddComponent == (UnityEngine.Object) null)
          orAddComponent = gameObject.AddComponent<T>();
        return orAddComponent;
      }

      public static void SetLayerRecursively(this GameObject gameObject, int layer)
      {
        gameObject.layer = layer;
        Transform transform = gameObject.transform;
        if (transform.childCount <= 0)
          return;
        for (int index = 0; index < transform.childCount; ++index)
          transform.GetChild(index).gameObject.SetLayerRecursively(layer);
      }

      public static void SetComponentEnabledRecursively<T>(this GameObject gameObject, bool enabled) where T : MonoBehaviour
      {
        foreach (T componentsInChild in gameObject.GetComponentsInChildren<T>())
          componentsInChild.enabled = enabled;
      }

      public static T[] GetInterfaces<T>(this GameObject gObj)
      {
        if (!typeof (T).IsInterface)
          throw new SystemException("Specified type is not an interface!");
        return ((IEnumerable<MonoBehaviour>) gObj.GetComponents<MonoBehaviour>()).Where<MonoBehaviour>((Func<MonoBehaviour, bool>) (a => ((IEnumerable<System.Type>) a.GetType().GetInterfaces()).Any<System.Type>((Func<System.Type, bool>) (k => k == typeof (T))))).Select<MonoBehaviour, T>((Func<MonoBehaviour, T>) (a => (T) a)).ToArray<T>();
      }

      public static T GetInterface<T>(this GameObject gObj)
      {
        if (!typeof (T).IsInterface)
          throw new SystemException("Specified type is not an interface!");
        return ((IEnumerable<T>) gObj.GetInterfaces<T>()).FirstOrDefault<T>();
      }

      public static T GetInterfaceInChildren<T>(this GameObject gObj)
      {
        if (!typeof (T).IsInterface)
          throw new Exception("Specified type is not an interface!");
        return ((IEnumerable<T>) gObj.GetInterfacesInChildren<T>()).FirstOrDefault<T>();
      }

      public static T[] GetInterfacesInChildren<T>(this GameObject gObj)
      {
        if (!typeof (T).IsInterface)
          throw new Exception("Specified type is not an interface!");
        return ((IEnumerable<MonoBehaviour>) gObj.GetComponentsInChildren<MonoBehaviour>()).Where<MonoBehaviour>((Func<MonoBehaviour, bool>) (a => ((IEnumerable<System.Type>) a.GetType().GetInterfaces()).Any<System.Type>((Func<System.Type, bool>) (k => k == typeof (T))))).Select<MonoBehaviour, T>((Func<MonoBehaviour, T>) (a => (T) a)).ToArray<T>();
      }

      public static int GetPhysicsCollisionMask(this GameObject gameObject, int layer = -1)
      {
        if (layer == -1)
          layer = gameObject.layer;
        int physicsCollisionMask = 0;
        for (int layer2 = 0; layer2 < 32 /*0x20*/; ++layer2)
          physicsCollisionMask |= (!Physics.GetIgnoreLayerCollision(layer, layer2) ? 1 : 0) << layer2;
        return physicsCollisionMask;
      }
    }

}
