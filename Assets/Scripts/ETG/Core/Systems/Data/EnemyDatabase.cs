using System;

using UnityEngine;

#nullable disable

public class EnemyDatabase : AssetBundleDatabase<AIActor, EnemyDatabaseEntry>
    {
        private static EnemyDatabase m_instance;
        private static AssetBundle m_assetBundle;

        public static EnemyDatabase Instance
        {
            get
            {
                if ((UnityEngine.Object) EnemyDatabase.m_instance == (UnityEngine.Object) null)
                {
                    float realtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
                    int frameCount = UnityEngine.Time.frameCount;
                    EnemyDatabase.m_instance = EnemyDatabase.AssetBundle.LoadAsset<EnemyDatabase>(nameof (EnemyDatabase));
                    DebugTime.Log(realtimeSinceStartup, frameCount, "Loading EnemyDatabase from AssetBundle");
                }
                return EnemyDatabase.m_instance;
            }
        }

        public static bool HasInstance => (UnityEngine.Object) EnemyDatabase.m_instance != (UnityEngine.Object) null;

        public static AssetBundle AssetBundle
        {
            get
            {
                if ((UnityEngine.Object) EnemyDatabase.m_assetBundle == (UnityEngine.Object) null)
                    EnemyDatabase.m_assetBundle = ResourceManager.LoadAssetBundle("enemies_base_001");
                return EnemyDatabase.m_assetBundle;
            }
        }

        public override void DropReferences() => base.DropReferences();

        public AIActor InternalGetByName(string name)
        {
            int index = 0;
            for (int count = this.Entries.Count; index < count; ++index)
            {
                EnemyDatabaseEntry entry = this.Entries[index];
                if (entry != null && entry.name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return entry.GetPrefab<AIActor>();
            }
            return (AIActor) null;
        }

        public AIActor InternalGetByGuid(string guid)
        {
            int index = 0;
            for (int count = this.Entries.Count; index < count; ++index)
            {
                EnemyDatabaseEntry entry = this.Entries[index];
                if (entry != null && entry.myGuid == guid)
                    return entry.GetPrefab<AIActor>();
            }
            return (AIActor) null;
        }

        public static void Unload() => EnemyDatabase.m_instance = (EnemyDatabase) null;

        public static AIActor GetOrLoadByName(string name)
        {
            return EnemyDatabase.Instance.InternalGetByName(name);
        }

        public static AIActor GetOrLoadByGuid(string guid)
        {
            return EnemyDatabase.Instance.InternalGetByGuid(guid);
        }

        public static EnemyDatabaseEntry GetEntry(string guid)
        {
            return EnemyDatabase.Instance.InternalGetDataByGuid(guid);
        }
    }

