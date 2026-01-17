// Decompiled with JetBrains decompiler
// Type: GameActorMaxHealthEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Actors.Enemy
{
    [Serializable]
    public class GameActorMaxHealthEffect : GameActorEffect
    {
      public float HealthMultiplier = 1f;
      public bool KeepHealthPercentage = true;

      public override void OnEffectApplied(
        GameActor actor,
        RuntimeGameActorEffectData effectData,
        float partialAmount = 1f)
      {
        float healthPercentage = actor.healthHaver.GetCurrentHealthPercentage();
        float targetValue = actor.healthHaver.GetMaxHealth() * this.HealthMultiplier;
        actor.healthHaver.SetHealthMaximum(targetValue);
        if (!this.KeepHealthPercentage)
          return;
        actor.healthHaver.ForceSetCurrentHealth(healthPercentage * targetValue);
      }

      public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
      {
        float healthPercentage = actor.healthHaver.GetCurrentHealthPercentage();
        float targetValue = actor.healthHaver.GetMaxHealth() / this.HealthMultiplier;
        actor.healthHaver.SetHealthMaximum(targetValue);
        if (!this.KeepHealthPercentage)
          return;
        actor.healthHaver.ForceSetCurrentHealth(healthPercentage * targetValue);
      }
    }

}
