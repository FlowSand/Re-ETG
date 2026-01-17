// Decompiled with JetBrains decompiler
// Type: PathologicalGames.PoolManagerUtils
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace PathologicalGames;

public static class PoolManagerUtils
{
  internal static void SetActive(GameObject obj, bool state) => obj.SetActive(state);

  internal static bool activeInHierarchy(GameObject obj) => obj.activeInHierarchy;
}
