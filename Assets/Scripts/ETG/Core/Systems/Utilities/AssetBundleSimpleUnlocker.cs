using UnityEngine;

#nullable disable

public class AssetBundleSimpleUnlocker : MonoBehaviour
  {
    public GungeonFlags[] FlagsToSetUponLoad;

    public void OnGameStartup()
    {
      for (int index = 0; index < this.FlagsToSetUponLoad.Length; ++index)
        GameStatsManager.Instance.SetFlag(this.FlagsToSetUponLoad[index], true);
    }
  }

