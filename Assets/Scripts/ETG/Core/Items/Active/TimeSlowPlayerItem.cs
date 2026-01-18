// Decompiled with JetBrains decompiler
// Type: TimeSlowPlayerItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;

#nullable disable

public class TimeSlowPlayerItem : PlayerItem
  {
    public float timeScale = 0.5f;
    public float duration = 5f;
    public bool HasSynergy;
    [LongNumericEnum]
    public CustomSynergyType RequiredSynergy;
    public float overrideTimeScale;
    public RadialSlowInterface test;

    protected override void DoEffect(PlayerController user)
    {
      user.StartCoroutine(this.HandleDuration(user));
    }

    [DebuggerHidden]
    private IEnumerator HandleDuration(PlayerController user)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new TimeSlowPlayerItem__HandleDurationc__Iterator0()
      {
        user = user,
        _this = this
      };
    }

    protected override void OnPreDrop(PlayerController user)
    {
      if (!this.IsCurrentlyActive)
        return;
      this.StopAllCoroutines();
      int num = (int) AkSoundEngine.PostEvent("State_Bullet_Time_off", this.gameObject);
      BraveTime.ClearMultiplier(this.gameObject);
      this.IsCurrentlyActive = false;
    }

    protected override void OnDestroy()
    {
      if (this.IsCurrentlyActive)
      {
        this.StopAllCoroutines();
        int num = (int) AkSoundEngine.PostEvent("State_Bullet_Time_off", this.gameObject);
        BraveTime.ClearMultiplier(this.gameObject);
        this.IsCurrentlyActive = false;
      }
      base.OnDestroy();
    }
  }

