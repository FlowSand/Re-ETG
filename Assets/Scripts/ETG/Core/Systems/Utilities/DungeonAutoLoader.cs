using UnityEngine;

using Dungeonator;

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

