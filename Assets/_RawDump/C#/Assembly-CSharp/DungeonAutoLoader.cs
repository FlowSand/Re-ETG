// Decompiled with JetBrains decompiler
// Type: DungeonAutoLoader
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable
public class DungeonAutoLoader : MonoBehaviour
{
  public void Awake()
  {
    if ((bool) (Object) GameManager.Instance.DungeonToAutoLoad)
    {
      Object.Instantiate<Dungeon>(GameManager.Instance.DungeonToAutoLoad);
      GameManager.Instance.DungeonToAutoLoad = (Dungeon) null;
    }
    Object.Destroy((Object) this.gameObject);
  }
}
