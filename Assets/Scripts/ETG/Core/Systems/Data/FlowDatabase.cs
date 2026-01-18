using UnityEngine;

#nullable disable

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

