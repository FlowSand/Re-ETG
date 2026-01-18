using UnityEngine;

#nullable disable

public class KillOnRoomUnseal : BraveBehaviour
  {
    public void Update()
    {
      if (!this.aiActor.enabled || !this.behaviorSpeculator.enabled || this.aiActor.ParentRoom.IsSealed || this.aiAnimator.IsPlaying("spawn") || this.aiAnimator.IsPlaying("awaken"))
        return;
      this.enabled = false;
      this.healthHaver.PreventAllDamage = false;
      this.healthHaver.ApplyDamage(100000f, Vector2.zero, "Room Clear", damageCategory: DamageCategory.Unstoppable, ignoreInvulnerabilityFrames: true);
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

