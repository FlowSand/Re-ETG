// Decompiled with JetBrains decompiler
// Type: DelayedExplosiveBuff
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Effects
{
    public class DelayedExplosiveBuff : AppliedEffectBase
    {
      public bool additionalInstancesRefreshDelay = true;
      public float delayBeforeBurst = 0.25f;
      public ExplosionData explosionData;
      public GameObject vfx;
      [NonSerialized]
      public bool IsSecondaryBuff;
      private float elapsed;
      private GameObject instantiatedVFX;
      private HealthHaver hh;

      private void InitializeSelf(float delayBefore, bool doRefresh, ExplosionData data)
      {
        this.explosionData = data;
        this.additionalInstancesRefreshDelay = doRefresh;
        this.delayBeforeBurst = delayBefore;
        this.hh = this.GetComponent<HealthHaver>();
        if ((UnityEngine.Object) this.hh != (UnityEngine.Object) null)
          this.StartCoroutine(this.ApplyModification());
        else
          UnityEngine.Object.Destroy((UnityEngine.Object) this);
      }

      public override void Initialize(AppliedEffectBase source)
      {
        if (source is DelayedExplosiveBuff)
        {
          DelayedExplosiveBuff delayedExplosiveBuff = source as DelayedExplosiveBuff;
          this.InitializeSelf(delayedExplosiveBuff.delayBeforeBurst, delayedExplosiveBuff.additionalInstancesRefreshDelay, delayedExplosiveBuff.explosionData);
          if (!((UnityEngine.Object) delayedExplosiveBuff.vfx != (UnityEngine.Object) null))
            return;
          this.instantiatedVFX = SpawnManager.SpawnVFX(delayedExplosiveBuff.vfx, this.transform.position, Quaternion.identity, true);
          tk2dSprite component1 = this.instantiatedVFX.GetComponent<tk2dSprite>();
          tk2dSprite component2 = this.GetComponent<tk2dSprite>();
          if ((UnityEngine.Object) component1 != (UnityEngine.Object) null && (UnityEngine.Object) component2 != (UnityEngine.Object) null)
          {
            component2.AttachRenderer((tk2dBaseSprite) component1);
            component1.HeightOffGround = 0.1f;
            component1.IsPerpendicular = true;
            component1.usesOverrideMaterial = true;
          }
          BuffVFXAnimator component3 = this.instantiatedVFX.GetComponent<BuffVFXAnimator>();
          if (!((UnityEngine.Object) component3 != (UnityEngine.Object) null))
            return;
          component3.Initialize(this.GetComponent<GameActor>());
        }
        else
          UnityEngine.Object.Destroy((UnityEngine.Object) this);
      }

      public void ExtendLength() => this.elapsed = 0.0f;

      public override void AddSelfToTarget(GameObject target)
      {
        if ((UnityEngine.Object) target.GetComponent<HealthHaver>() == (UnityEngine.Object) null)
          return;
        bool flag = false;
        if (this.additionalInstancesRefreshDelay)
        {
          foreach (DelayedExplosiveBuff component in target.GetComponents<DelayedExplosiveBuff>())
          {
            flag = true;
            component.ExtendLength();
          }
        }
        DelayedExplosiveBuff delayedExplosiveBuff = target.AddComponent<DelayedExplosiveBuff>();
        delayedExplosiveBuff.IsSecondaryBuff = flag;
        delayedExplosiveBuff.Initialize((AppliedEffectBase) this);
      }

      [DebuggerHidden]
      private IEnumerator ApplyModification()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new DelayedExplosiveBuff__ApplyModificationc__Iterator0()
        {
          _this = this
        };
      }
    }

}
