using System;

using UnityEngine;

#nullable disable
namespace AK.Wwise
{
    [Serializable]
    public class RTPC : BaseType
    {
        public void SetValue(GameObject gameObject, float value)
        {
            if (!this.IsValid())
                return;
            this.Verify(AkSoundEngine.SetRTPCValue(this.GetID(), value, gameObject));
        }

        public void SetGlobalValue(float value)
        {
            if (!this.IsValid())
                return;
            this.Verify(AkSoundEngine.SetRTPCValue(this.GetID(), value));
        }
    }
}
