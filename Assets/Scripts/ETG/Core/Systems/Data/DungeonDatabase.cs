// Decompiled with JetBrains decompiler
// Type: DungeonDatabase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable

public class DungeonDatabase
  {
    public static Dungeonator.Dungeon GetOrLoadByName(string name)
    {
      AssetBundle assetBundle = ResourceManager.LoadAssetBundle("dungeons/" + name.ToLower());
      DebugTime.RecordStartTime();
      Dungeonator.Dungeon component = assetBundle.LoadAsset<GameObject>(name).GetComponent<Dungeonator.Dungeon>();
      DebugTime.Log("AssetBundle.LoadAsset<Dungeonator.Dungeon>({0})", (object) name);
      return component;
    }
  }

