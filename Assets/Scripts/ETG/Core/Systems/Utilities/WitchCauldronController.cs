// Decompiled with JetBrains decompiler
// Type: WitchCauldronController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class WitchCauldronController : MonoBehaviour
    {
      public tk2dBaseSprite cauldronSprite;
      public GenericLootTable lootTableToUse;
      public float baseChanceOfImprovingItem = 0.5f;
      public float CurseToGive = 2f;
      public string[] cauldronIns;
      public string[] cauldronIdles;
      public string[] cauldronOuts;

      public bool IsGunInPot { get; private set; }

      public void Start() => this.StartCoroutine(this.HandleBackgroundBubblin());

      [DebuggerHidden]
      private IEnumerator HandleBackgroundBubblin()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new WitchCauldronController.<HandleBackgroundBubblin>c__Iterator0()
        {
          $this = this
        };
      }

      public bool TossPlayerEquippedGun(PlayerController player)
      {
        if (!((UnityEngine.Object) player.CurrentGun != (UnityEngine.Object) null) || !player.CurrentGun.CanActuallyBeDropped(player) || player.CurrentGun.InfiniteAmmo)
          return false;
        this.IsGunInPot = true;
        Gun currentGun = player.CurrentGun;
        this.TossObjectIntoPot(currentGun.GetSprite(), (Vector3) player.CenterPosition);
        player.inventory.RemoveGunFromInventory(currentGun);
        PickupObject.ItemQuality quality = currentGun.quality;
        if (quality < PickupObject.ItemQuality.S && (double) UnityEngine.Random.value < (double) this.baseChanceOfImprovingItem)
          ++quality;
        Gun ofTypeAndQuality = LootEngine.GetItemOfTypeAndQuality<Gun>(quality, this.lootTableToUse);
        if ((UnityEngine.Object) ofTypeAndQuality != (UnityEngine.Object) null)
          this.StartCoroutine(this.DelayedItemSpawn(ofTypeAndQuality.gameObject, 3f));
        else
          this.StartCoroutine(this.DelayedItemSpawn(currentGun.gameObject, 3f));
        UnityEngine.Object.Destroy((UnityEngine.Object) currentGun.gameObject);
        return true;
      }

      [DebuggerHidden]
      private IEnumerator DelayedItemSpawn(GameObject item, float delay)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new WitchCauldronController.<DelayedItemSpawn>c__Iterator1()
        {
          delay = delay,
          item = item,
          $this = this
        };
      }

      public void TossObjectIntoPot(tk2dBaseSprite spriteSource, Vector3 startPosition)
      {
        this.StartCoroutine(this.HandleObjectPotToss(spriteSource, startPosition));
      }

      [DebuggerHidden]
      private IEnumerator HandleObjectPotToss(tk2dBaseSprite spriteSource, Vector3 startPosition)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new WitchCauldronController.<HandleObjectPotToss>c__Iterator2()
        {
          spriteSource = spriteSource,
          startPosition = startPosition,
          $this = this
        };
      }

      private void OnAnimComplete(tk2dSpriteAnimator arg1, tk2dSpriteAnimationClip arg2)
      {
        arg1.AnimationCompleted -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnAnimComplete);
        arg1.Play("cauldron_idle");
      }
    }

}
