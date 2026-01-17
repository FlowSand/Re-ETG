// Decompiled with JetBrains decompiler
// Type: EncounterDatabase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Data
{
    public class EncounterDatabase : AssetBundleDatabase<EncounterTrackable, EncounterDatabaseEntry>
    {
      public static EncounterDatabase m_instance;
      private static AssetBundle m_assetBundle;

      public static EncounterDatabase Instance
      {
        get
        {
          if ((Object) EncounterDatabase.m_instance == (Object) null)
          {
            float realtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
            int frameCount = UnityEngine.Time.frameCount;
            EncounterDatabase.m_instance = EncounterDatabase.AssetBundle.LoadAsset<EncounterDatabase>(nameof (EncounterDatabase));
            DebugTime.Log(realtimeSinceStartup, frameCount, "Loading EncounterDatabase from AssetBundle");
          }
          return EncounterDatabase.m_instance;
        }
      }

      public static bool HasInstance => (Object) EncounterDatabase.m_instance != (Object) null;

      public static AssetBundle AssetBundle
      {
        get
        {
          if ((Object) EncounterDatabase.m_assetBundle == (Object) null)
            EncounterDatabase.m_assetBundle = ResourceManager.LoadAssetBundle("encounters_base_001");
          return EncounterDatabase.m_assetBundle;
        }
      }

      public static void Unload() => EncounterDatabase.m_instance = (EncounterDatabase) null;

      public static EncounterDatabaseEntry GetEntry(string guid)
      {
        EncounterDatabaseEntry dataByGuid = EncounterDatabase.Instance.InternalGetDataByGuid(guid);
        if (dataByGuid != null && string.IsNullOrEmpty(dataByGuid.ProxyEncounterGuid))
          EncounterDatabase.Instance.InternalGetDataByGuid(dataByGuid.ProxyEncounterGuid);
        return dataByGuid;
      }

      public static bool IsProxy(string guid)
      {
        EncounterDatabaseEntry entry = EncounterDatabase.GetEntry(guid);
        return entry != null && !string.IsNullOrEmpty(entry.ProxyEncounterGuid);
      }
    }

}
