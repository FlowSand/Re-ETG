using UnityEngine;

using Dungeonator;

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

