// Decompiled with JetBrains decompiler
// Type: GameActorSpeedEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Actors.Enemy
{
    [Serializable]
    public class GameActorSpeedEffect : GameActorEffect
    {
      public float SpeedMultiplier = 1f;
      public float CooldownMultiplier = 1f;
      public bool OnlyAffectPlayerWhenGrounded;

      public override void OnEffectApplied(
        GameActor actor,
        RuntimeGameActorEffectData effectData,
        float partialAmount = 1f)
      {
        if ((double) this.SpeedMultiplier != 1.0)
          actor.specRigidbody.OnPreMovement += new Action<SpeculativeRigidbody>(this.ModifyVelocity);
        if ((double) this.CooldownMultiplier == 1.0 || !(bool) (UnityEngine.Object) actor.behaviorSpeculator)
          return;
        actor.behaviorSpeculator.CooldownScale /= this.CooldownMultiplier;
      }

      public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
      {
        if ((double) this.SpeedMultiplier != 1.0)
          actor.specRigidbody.OnPreMovement -= new Action<SpeculativeRigidbody>(this.ModifyVelocity);
        if ((double) this.CooldownMultiplier == 1.0 || !(bool) (UnityEngine.Object) actor.behaviorSpeculator)
          return;
        actor.behaviorSpeculator.CooldownScale *= this.CooldownMultiplier;
      }

      public void ModifyVelocity(SpeculativeRigidbody myRigidbody)
      {
        if (this.OnlyAffectPlayerWhenGrounded)
        {
          PlayerController gameActor = myRigidbody.gameActor as PlayerController;
          if ((bool) (UnityEngine.Object) gameActor && (!gameActor.IsGrounded || gameActor.IsSlidingOverSurface))
            return;
        }
        myRigidbody.Velocity *= this.SpeedMultiplier;
      }
    }

}
