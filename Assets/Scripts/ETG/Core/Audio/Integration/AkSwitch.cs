using UnityEngine;

#nullable disable

[AddComponentMenu("Wwise/AkSwitch")]
public class AkSwitch : AkUnityEventHandler
    {
        public int groupID;
        public int valueID;

        public override void HandleEvent(GameObject in_gameObject)
        {
            int num = (int) AkSoundEngine.SetSwitch((uint) this.groupID, (uint) this.valueID, !this.useOtherObject || !((Object) in_gameObject != (Object) null) ? this.gameObject : in_gameObject);
        }
    }

