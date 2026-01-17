// Decompiled with JetBrains decompiler
// Type: ConsumableStealthItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Active
{
    public class ConsumableStealthItem : PlayerItem
    {
      public float Duration = 10f;
      public GameObject poofVfx;

      protected override void DoEffect(PlayerController user)
      {
        base.DoEffect(user);
        int num = (int) AkSoundEngine.PostEvent("Play_ENM_wizardred_appear_01", this.gameObject);
        user.StartCoroutine(this.HandleStealth(user));
      }

      [DebuggerHidden]
      private IEnumerator HandleStealth(PlayerController user)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ConsumableStealthItem.\u003CHandleStealth\u003Ec__Iterator0()
        {
          user = user,
          \u0024this = this
        };
      }

      private void BreakStealthOnSteal(PlayerController arg1, ShopItemController arg2)
      {
        this.BreakStealth(arg1);
      }

      private void BreakStealth(PlayerController obj)
      {
        obj.PlayEffectOnActor(this.poofVfx, Vector3.zero, false, true);
        obj.OnDidUnstealthyAction -= new Action<PlayerController>(this.BreakStealth);
        obj.OnItemStolen -= new Action<PlayerController, ShopItemController>(this.BreakStealthOnSteal);
        obj.specRigidbody.RemoveCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider));
        obj.ChangeSpecialShaderFlag(1, 0.0f);
        obj.SetIsStealthed(false, "smoke");
        obj.SetCapableOfStealing(false, nameof (ConsumableStealthItem));
        int num = (int) AkSoundEngine.PostEvent("Play_ENM_wizardred_appear_01", this.gameObject);
      }
    }

}
