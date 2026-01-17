// Decompiled with JetBrains decompiler
// Type: KillOnRoomUnseal
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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
