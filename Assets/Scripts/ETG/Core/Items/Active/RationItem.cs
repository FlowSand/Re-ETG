// Decompiled with JetBrains decompiler
// Type: RationItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Active
{
    public class RationItem : PlayerItem
    {
      public float healingAmount = 2f;
      public GameObject healVFX;

      protected override void DoEffect(PlayerController user)
      {
        user.healthHaver.ApplyHealing(this.healingAmount);
        int num = (int) AkSoundEngine.PostEvent("Play_OBJ_med_kit_01", this.gameObject);
        if (!((Object) this.healVFX != (Object) null))
          return;
        user.PlayEffectOnActor(this.healVFX, Vector3.zero);
      }

      public void DoHealOnDeath(PlayerController user) => this.DoEffect(user);

      protected override void OnDestroy() => base.OnDestroy();
    }

}
