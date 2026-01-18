using System;
using UnityEngine;

#nullable disable
namespace AK.Wwise
{
  [Serializable]
  public class Switch : BaseGroupType
  {
    public void SetValue(GameObject gameObject)
    {
      if (!this.IsValid())
        return;
      this.Verify(AkSoundEngine.SetSwitch(this.GetGroupID(), this.GetID(), gameObject));
    }
  }
}
