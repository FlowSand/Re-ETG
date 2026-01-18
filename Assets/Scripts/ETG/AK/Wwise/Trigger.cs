using System;

using UnityEngine;

#nullable disable
namespace AK.Wwise
{
    [Serializable]
    public class Trigger : BaseType
    {
        public void Post(GameObject gameObject)
        {
            if (!this.IsValid())
                return;
            this.Verify(AkSoundEngine.PostTrigger(this.GetID(), gameObject));
        }
    }
}
