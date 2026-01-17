// Decompiled with JetBrains decompiler
// Type: SuperhotItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Passive
{
    public class SuperhotItem : PassiveItem
    {
      private bool m_active;

      protected override void Update()
      {
        base.Update();
        if (this.m_pickedUp && !GameManager.Instance.IsLoadingLevel && (Object) this.m_owner != (Object) null && (this.m_owner.CurrentInputState == PlayerInputState.AllInput || this.m_owner.CurrentInputState == PlayerInputState.OnlyMovement) && !this.m_owner.IsFalling && (bool) (Object) this.m_owner.healthHaver && !this.m_owner.healthHaver.IsDead)
        {
          this.m_active = true;
          float num = Mathf.Clamp01(this.m_owner.specRigidbody.Velocity.magnitude / this.m_owner.stats.MovementSpeed);
          if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
          {
            PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(this.m_owner);
            if ((bool) (Object) otherPlayer && (bool) (Object) otherPlayer.specRigidbody)
              num = Mathf.Max(num, Mathf.Clamp01(otherPlayer.specRigidbody.Velocity.magnitude / otherPlayer.stats.MovementSpeed));
          }
          float multiplier = Mathf.Lerp(0.01f, 1f, num);
          if (this.m_owner.IsDodgeRolling)
            multiplier = 1f;
          BraveTime.SetTimeScaleMultiplier(multiplier, this.gameObject);
        }
        else
        {
          if (!this.m_active)
            return;
          this.m_active = false;
          BraveTime.ClearMultiplier(this.gameObject);
        }
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
