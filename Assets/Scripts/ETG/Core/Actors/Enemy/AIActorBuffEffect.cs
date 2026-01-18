using System;

#nullable disable

[Serializable]
public class AIActorBuffEffect : GameActorEffect
  {
    public float SpeedMultiplier = 1f;
    public float CooldownMultiplier = 1f;
    public float HealthMultiplier = 1f;
    public bool KeepHealthPercentage = true;

    public override void OnEffectApplied(
      GameActor actor,
      RuntimeGameActorEffectData effectData,
      float partialAmount = 1f)
    {
      HealthHaver healthHaver = actor.healthHaver;
      float num1 = actor.healthHaver.GetMaxHealth() * this.HealthMultiplier;
      bool healthPercentage = this.KeepHealthPercentage;
      double targetValue = (double) num1;
      float? amountOfHealthToGain = new float?();
      int num2 = healthPercentage ? 1 : 0;
      healthHaver.SetHealthMaximum((float) targetValue, amountOfHealthToGain, num2 != 0);
      if ((double) this.SpeedMultiplier != 1.0)
        actor.specRigidbody.OnPreMovement += new Action<SpeculativeRigidbody>(this.ModifyVelocity);
      if ((double) this.CooldownMultiplier == 1.0 || !(bool) (UnityEngine.Object) actor.behaviorSpeculator)
        return;
      actor.behaviorSpeculator.CooldownScale /= this.CooldownMultiplier;
    }

    public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
    {
      HealthHaver healthHaver = actor.healthHaver;
      float num1 = actor.healthHaver.GetMaxHealth() / this.HealthMultiplier;
      bool healthPercentage = this.KeepHealthPercentage;
      double targetValue = (double) num1;
      float? amountOfHealthToGain = new float?();
      int num2 = healthPercentage ? 1 : 0;
      healthHaver.SetHealthMaximum((float) targetValue, amountOfHealthToGain, num2 != 0);
      if ((double) this.SpeedMultiplier != 1.0)
        actor.specRigidbody.OnPreMovement -= new Action<SpeculativeRigidbody>(this.ModifyVelocity);
      if ((double) this.CooldownMultiplier == 1.0 || !(bool) (UnityEngine.Object) actor.behaviorSpeculator)
        return;
      actor.behaviorSpeculator.CooldownScale *= this.CooldownMultiplier;
    }

    public void ModifyVelocity(SpeculativeRigidbody myRigidbody)
    {
      myRigidbody.Velocity *= this.SpeedMultiplier;
    }
  }

