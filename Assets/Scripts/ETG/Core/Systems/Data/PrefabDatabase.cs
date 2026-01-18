using UnityEngine;

#nullable disable

public class PrefabDatabase : ScriptableObject
    {
        public GameObject SuperReaper;
        public GameObject ResourcefulRatThief;
        private static PrefabDatabase m_instance;
        private static AssetBundle m_assetBundle;

        public static PrefabDatabase Instance
        {
            get
            {
                if ((Object) PrefabDatabase.m_instance == (Object) null)
                {
                    DebugTime.RecordStartTime();
                    PrefabDatabase.m_instance = PrefabDatabase.AssetBundle.LoadAsset<PrefabDatabase>(nameof (PrefabDatabase));
                    DebugTime.Log("Loading PrefabDatabase from AssetBundle");
                }
                return PrefabDatabase.m_instance;
            }
        }

        public static bool HasInstance => (Object) PrefabDatabase.m_instance != (Object) null;

        public static AssetBundle AssetBundle
        {
            get
            {
                if ((Object) PrefabDatabase.m_assetBundle == (Object) null)
                    PrefabDatabase.m_assetBundle = ResourceManager.LoadAssetBundle("shared_base_001");
                return PrefabDatabase.m_assetBundle;
            }
        }
    }

