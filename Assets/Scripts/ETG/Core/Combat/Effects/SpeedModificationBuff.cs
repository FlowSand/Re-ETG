// Decompiled with JetBrains decompiler
// Type: SpeedModificationBuff
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Effects
{
    public class SpeedModificationBuff : AppliedEffectBase
    {
      public float maximumSpeedModifier;
      public float lifespan;
      public float maxLifespan;
      private float elapsed;

      public static void ApplySpeedModificationToTarget(
        GameObject target,
        float maxSpeedMod,
        float lifetime,
        float maxLifetime)
      {
        if ((Object) target.GetComponent<SpeculativeRigidbody>() == (Object) null)
          return;
        SpeedModificationBuff component = target.GetComponent<SpeedModificationBuff>();
        if ((Object) component != (Object) null)
        {
          component.ExtendLength(lifetime);
        }
        else
        {
          SpeedModificationBuff modificationBuff = target.AddComponent<SpeedModificationBuff>();
          modificationBuff.maximumSpeedModifier = maxSpeedMod;
          modificationBuff.lifespan = lifetime;
          modificationBuff.maxLifespan = maxLifetime;
        }
      }

      public override void AddSelfToTarget(GameObject target)
      {
        if ((Object) target.GetComponent<SpeculativeRigidbody>() == (Object) null)
          return;
        SpeedModificationBuff component = target.GetComponent<SpeedModificationBuff>();
        if ((Object) component != (Object) null)
          component.ExtendLength(this.lifespan);
        else
          target.AddComponent<SpeedModificationBuff>().Initialize((AppliedEffectBase) this);
      }

      public override void Initialize(AppliedEffectBase source)
      {
        if (source is SpeedModificationBuff)
        {
          SpeedModificationBuff modificationBuff = source as SpeedModificationBuff;
          this.maximumSpeedModifier = modificationBuff.maximumSpeedModifier;
          this.lifespan = modificationBuff.lifespan;
          this.maxLifespan = modificationBuff.maxLifespan;
        }
        else
          Object.Destroy((Object) this);
      }

      public void ExtendLength(float time)
      {
        this.lifespan = Mathf.Min(this.lifespan + time, this.elapsed + this.maxLifespan);
      }

      [DebuggerHidden]
      private IEnumerator ApplyModification()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new SpeedModificationBuff__ApplyModificationc__Iterator0()
        {
          _this = this
        };
      }
    }

}
