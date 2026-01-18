// Decompiled with JetBrains decompiler
// Type: GameActorEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

[Serializable]
  public abstract class GameActorEffect
  {
    public bool AffectsPlayers = true;
    public bool AffectsEnemies = true;
    public string effectIdentifier = "effect";
    public EffectResistanceType resistanceType;
    public GameActorEffect.EffectStackingMode stackMode;
    public float duration = 10f;
[ShowInInspectorIf("stackMode", 1, false)]
    public float maxStackedDuration = -1f;
    public bool AppliesTint;
[ShowInInspectorIf("AppliesTint", false)]
    public Color TintColor = new Color(1f, 1f, 1f, 0.5f);
    public bool AppliesDeathTint;
[ShowInInspectorIf("AppliesDeathTint", false)]
    public Color DeathTintColor = new Color(0.388f, 0.388f, 0.388f, 1f);
    public bool AppliesOutlineTint;
[ColorUsage(true, true, 0.0f, 1000f, 0.125f, 3f)]
    public Color OutlineTintColor = new Color(0.0f, 0.0f, 0.0f, 1f);
    public GameObject OverheadVFX;
    public bool PlaysVFXOnActor;

    public virtual bool ResistanceAffectsDuration => false;

    public virtual void OnEffectApplied(
      GameActor actor,
      RuntimeGameActorEffectData effectData,
      float partialAmount = 1f)
    {
    }

    public virtual void OnDarkSoulsAccumulate(
      GameActor actor,
      RuntimeGameActorEffectData effectData,
      float partialAmount = 1f,
      Projectile sourceProjectile = null)
    {
    }

    public virtual void EffectTick(GameActor actor, RuntimeGameActorEffectData effectData)
    {
    }

    public virtual void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
    {
    }

    public virtual void ApplyTint(GameActor actor)
    {
      if (this.AppliesTint)
        actor.RegisterOverrideColor(this.TintColor, this.effectIdentifier);
      if (!this.AppliesOutlineTint || !(actor is AIActor))
        return;
      (actor as AIActor).SetOverrideOutlineColor(this.OutlineTintColor);
    }

    public virtual bool IsFinished(
      GameActor actor,
      RuntimeGameActorEffectData effectData,
      float elapsedTime)
    {
      float a = this.duration;
      if (this is GameActorFireEffect && (bool) (UnityEngine.Object) actor.healthHaver && actor.healthHaver.IsBoss)
        a = Mathf.Min(a, 8f);
      if (this.ResistanceAffectsDuration)
      {
        float resistanceForEffectType = actor.GetResistanceForEffectType(this.resistanceType);
        a *= Mathf.Clamp01(1f - resistanceForEffectType);
      }
      return (double) elapsedTime >= (double) a;
    }

public enum EffectStackingMode
    {
      Refresh,
      Stack,
      Ignore,
      DarkSoulsAccumulate,
    }
  }

