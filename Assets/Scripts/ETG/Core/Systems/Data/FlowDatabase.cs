// Decompiled with JetBrains decompiler
// Type: FlowDatabase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Data
{
    public class FlowDatabase
    {
      private static AssetBundle m_assetBundle;

      public static DungeonFlow GetOrLoadByName(string name)
      {
        if (!(bool) (Object) FlowDatabase.m_assetBundle)
          FlowDatabase.m_assetBundle = ResourceManager.LoadAssetBundle("flows_base_001");
        string name1 = name;
        if (name1.Contains("/"))
          name1 = name.Substring(name.LastIndexOf("/") + 1);
        DebugTime.RecordStartTime();
        DungeonFlow orLoadByName = FlowDatabase.m_assetBundle.LoadAsset<DungeonFlow>(name1);
        DebugTime.Log("AssetBundle.LoadAsset<DungeonFlow>({0})", (object) name1);
        return orLoadByName;
      }
    }

}
