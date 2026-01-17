// Decompiled with JetBrains decompiler
// Type: tk2dUtil
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public static class tk2dUtil
{
  private static string label = string.Empty;
  private static bool undoEnabled;

  public static bool UndoEnabled
  {
    get => tk2dUtil.undoEnabled;
    set => tk2dUtil.undoEnabled = value;
  }

  public static void BeginGroup(string name)
  {
    tk2dUtil.undoEnabled = true;
    tk2dUtil.label = name;
  }

  public static void EndGroup() => tk2dUtil.label = string.Empty;

  public static void DestroyImmediate(Object obj)
  {
    if (obj == (Object) null)
      return;
    Object.DestroyImmediate(obj);
  }

  public static GameObject CreateGameObject(string name) => new GameObject(name);

  public static Mesh CreateMesh() => new Mesh();

  public static T AddComponent<T>(GameObject go) where T : Component => go.AddComponent<T>();

  public static void SetActive(GameObject go, bool active)
  {
    if (active == go.activeSelf)
      return;
    go.SetActive(active);
  }

  public static void SetTransformParent(Transform t, Transform parent) => t.parent = parent;
}
