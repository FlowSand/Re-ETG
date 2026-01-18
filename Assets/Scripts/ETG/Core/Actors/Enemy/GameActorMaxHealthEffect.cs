using System;

#nullable disable

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

