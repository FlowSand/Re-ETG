using UnityEngine;

#nullable disable

[AddComponentMenu("Wwise/AkState")]
public class AkState : AkUnityEventHandler
  {
    public int groupID;
    public int valueID;

    public override void HandleEvent(GameObject in_gameObject)
    {
      int num = (int) AkSoundEngine.SetState((uint) this.groupID, (uint) this.valueID);
    }
  }

