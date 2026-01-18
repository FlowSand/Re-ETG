using System;

#nullable disable
namespace AK.Wwise
{
  [Serializable]
  public class State : BaseGroupType
  {
    public void SetValue()
    {
      if (!this.IsValid())
        return;
      this.Verify(AkSoundEngine.SetState(this.GetGroupID(), this.GetID()));
    }
  }
}
