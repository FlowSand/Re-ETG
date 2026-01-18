using System;
using UnityEngine;

#nullable disable

public class ChaosBulletsItem : PassiveItem
  {
    [Header("Nonstatus Effects")]
    public float ChanceToAddBounce;
    public float ChanceToAddPierce;
    public float ChanceToFat = 0.1f;
    public float MinFatScale = 1.25f;
    public float MaxFatScale = 1.75f;
    public bool UsesVelocityModificationCurve;
    public AnimationCurve VelocityModificationCurve;
    [Header("Status Effects")]
    public float ChanceOfActivatingStatusEffect = 1f;
    public float ChanceOfStatusEffectFromBeamPerSecond = 1f;
    public float SpeedModifierWeight;
    public GameActorSpeedEffect SpeedModifierEffect;
    public Color SpeedTintColor;
    public float PoisonModifierWeight;
    public GameActorHealthEffect HealthModifierEffect;
    public Color PoisonTintColor;
    public float CharmModifierWeight;
    public GameActorCharmEffect CharmModifierEffect;
    public Color CharmTintColor;
    public float FreezeModifierWeight;
    public GameActorFreezeEffect FreezeModifierEffect;
    public bool FreezeScalesWithDamage;
    public float FreezeAmountPerDamage = 1f;
    public Color FreezeTintColor;
    public float BurnModifierWeight;
    public GameActorFireEffect FireModifierEffect;
    public Color FireTintColor;
    public float TransmogrifyModifierWeight;
    [EnemyIdentifier]
    public string TransmogTargetGuid;
    public Color TransmogrifyTintColor;
    public bool TintBullets;
    public bool TintBeams;
    public int TintPriority = 6;
    private PlayerController m_player;

    public override void Pickup(PlayerController player)
    {
      if (this.m_pickedUp)
        return;
      this.m_player = player;
      base.Pickup(player);
      player.PostProcessProjectile += new Action<Projectile, float>(this.PostProcessProjectile);
      player.PostProcessBeam += new Action<BeamController>(this.PostProcessBeam);
      player.PostProcessBeamTick += new Action<BeamController, SpeculativeRigidbody, float>(this.PostProcessBeamTick);
    }

    private void PostProcessProjectile(Projectile obj, float effectChanceScalar)
    {
      if (this.UsesVelocityModificationCurve)
        obj.baseData.speed *= this.VelocityModificationCurve.Evaluate(UnityEngine.Random.value);
      int num1 = 0;
      while ((double) UnityEngine.Random.value < (double) this.ChanceToAddBounce && num1 < 10)
      {
        ++num1;
        BounceProjModifier component = obj.GetComponent<BounceProjModifier>();
        if ((UnityEngine.Object) component == (UnityEngine.Object) null)
          obj.gameObject.AddComponent<BounceProjModifier>().numberOfBounces = 1;
        else
          ++component.numberOfBounces;
      }
      int num2 = 0;
      while ((double) UnityEngine.Random.value < (double) this.ChanceToAddPierce && num2 < 10)
      {
        ++num2;
        PierceProjModifier component = obj.GetComponent<PierceProjModifier>();
        if ((UnityEngine.Object) component == (UnityEngine.Object) null)
        {
          PierceProjModifier pierceProjModifier = obj.gameObject.AddComponent<PierceProjModifier>();
          pierceProjModifier.penetration = 2;
          pierceProjModifier.penetratesBreakables = true;
          pierceProjModifier.BeastModeLevel = PierceProjModifier.BeastModeStatus.NOT_BEAST_MODE;
        }
        else
          component.penetration += 2;
      }
      if ((double) UnityEngine.Random.value < (double) this.ChanceToFat)
      {
        float num3 = UnityEngine.Random.Range(this.MinFatScale, this.MaxFatScale);
        obj.AdditionalScaleMultiplier *= num3;
      }
      float num4 = this.ChanceOfActivatingStatusEffect;
      if ((double) this.ChanceOfActivatingStatusEffect < 1.0)
        num4 = this.ChanceOfActivatingStatusEffect * effectChanceScalar;
      if ((double) UnityEngine.Random.value >= (double) num4)
        return;
      Color targetTintColor = Color.white;
      float num5 = (this.SpeedModifierWeight + this.PoisonModifierWeight + this.FreezeModifierWeight + this.CharmModifierWeight + this.BurnModifierWeight + this.TransmogrifyModifierWeight) * UnityEngine.Random.value;
      if ((double) num5 < (double) this.SpeedModifierWeight)
      {
        targetTintColor = this.SpeedTintColor;
        obj.statusEffectsToApply.Add((GameActorEffect) this.SpeedModifierEffect);
      }
      else if ((double) num5 < (double) this.SpeedModifierWeight + (double) this.PoisonModifierWeight)
      {
        targetTintColor = this.PoisonTintColor;
        obj.statusEffectsToApply.Add((GameActorEffect) this.HealthModifierEffect);
      }
      else if ((double) num5 < (double) this.SpeedModifierWeight + (double) this.PoisonModifierWeight + (double) this.FreezeModifierWeight)
      {
        targetTintColor = this.FreezeTintColor;
        GameActorFreezeEffect freezeModifierEffect = this.FreezeModifierEffect;
        if (this.FreezeScalesWithDamage)
          freezeModifierEffect.FreezeAmount = obj.ModifiedDamage * this.FreezeAmountPerDamage;
        obj.statusEffectsToApply.Add((GameActorEffect) freezeModifierEffect);
      }
      else if ((double) num5 < (double) this.SpeedModifierWeight + (double) this.PoisonModifierWeight + (double) this.FreezeModifierWeight + (double) this.CharmModifierWeight)
      {
        targetTintColor = this.CharmTintColor;
        obj.statusEffectsToApply.Add((GameActorEffect) this.CharmModifierEffect);
      }
      else if ((double) num5 < (double) this.SpeedModifierWeight + (double) this.PoisonModifierWeight + (double) this.FreezeModifierWeight + (double) this.CharmModifierWeight + (double) this.BurnModifierWeight)
      {
        targetTintColor = this.FireTintColor;
        obj.statusEffectsToApply.Add((GameActorEffect) this.FireModifierEffect);
      }
      else if ((double) num5 < (double) this.SpeedModifierWeight + (double) this.PoisonModifierWeight + (double) this.FreezeModifierWeight + (double) this.CharmModifierWeight + (double) this.BurnModifierWeight + (double) this.TransmogrifyModifierWeight)
      {
        targetTintColor = this.TransmogrifyTintColor;
        obj.CanTransmogrify = true;
        obj.ChanceToTransmogrify = 1f;
        obj.TransmogrifyTargetGuids = new string[1];
        obj.TransmogrifyTargetGuids[0] = this.TransmogTargetGuid;
      }
      if (!this.TintBullets)
        return;
      obj.AdjustPlayerProjectileTint(targetTintColor, this.TintPriority);
    }

    private void PostProcessBeam(BeamController beam)
    {
      if (!this.TintBeams)
        ;
    }

    private void PostProcessBeamTick(
      BeamController beam,
      SpeculativeRigidbody hitRigidbody,
      float tickRate)
    {
      GameActor gameActor = hitRigidbody.gameActor;
      if (!(bool) (UnityEngine.Object) gameActor || (double) UnityEngine.Random.value >= (double) BraveMathCollege.SliceProbability(this.ChanceOfStatusEffectFromBeamPerSecond, tickRate))
        return;
      float num = (this.SpeedModifierWeight + this.PoisonModifierWeight + this.FreezeModifierWeight + this.CharmModifierWeight + this.BurnModifierWeight + this.TransmogrifyModifierWeight) * UnityEngine.Random.value;
      if ((double) num < (double) this.SpeedModifierWeight)
        gameActor.ApplyEffect((GameActorEffect) this.SpeedModifierEffect);
      else if ((double) num < (double) this.SpeedModifierWeight + (double) this.PoisonModifierWeight)
        gameActor.ApplyEffect((GameActorEffect) this.HealthModifierEffect);
      else if ((double) num < (double) this.SpeedModifierWeight + (double) this.PoisonModifierWeight + (double) this.FreezeModifierWeight)
        gameActor.ApplyEffect((GameActorEffect) this.FreezeModifierEffect);
      else if ((double) num < (double) this.SpeedModifierWeight + (double) this.PoisonModifierWeight + (double) this.FreezeModifierWeight + (double) this.CharmModifierWeight)
        gameActor.ApplyEffect((GameActorEffect) this.CharmModifierEffect);
      else if ((double) num < (double) this.SpeedModifierWeight + (double) this.PoisonModifierWeight + (double) this.FreezeModifierWeight + (double) this.CharmModifierWeight + (double) this.BurnModifierWeight)
      {
        gameActor.ApplyEffect((GameActorEffect) this.FireModifierEffect);
      }
      else
      {
        if ((double) num >= (double) this.SpeedModifierWeight + (double) this.PoisonModifierWeight + (double) this.FreezeModifierWeight + (double) this.CharmModifierWeight + (double) this.BurnModifierWeight + (double) this.TransmogrifyModifierWeight || !(gameActor is AIActor))
          return;
        (gameActor as AIActor).Transmogrify(EnemyDatabase.GetOrLoadByGuid(this.TransmogTargetGuid), (GameObject) ResourceCache.Acquire("Global VFX/VFX_Item_Spawn_Poof"));
      }
    }

    public override DebrisObject Drop(PlayerController player)
    {
      DebrisObject debrisObject = base.Drop(player);
      this.m_player = (PlayerController) null;
      debrisObject.GetComponent<ChaosBulletsItem>().m_pickedUpThisRun = true;
      player.PostProcessProjectile -= new Action<Projectile, float>(this.PostProcessProjectile);
      player.PostProcessBeam -= new Action<BeamController>(this.PostProcessBeam);
      player.PostProcessBeamTick -= new Action<BeamController, SpeculativeRigidbody, float>(this.PostProcessBeamTick);
      return debrisObject;
    }

    protected override void OnDestroy()
    {
      base.OnDestroy();
      if (!(bool) (UnityEngine.Object) this.m_player)
        return;
      this.m_player.PostProcessProjectile -= new Action<Projectile, float>(this.PostProcessProjectile);
      this.m_player.PostProcessBeam -= new Action<BeamController>(this.PostProcessBeam);
      this.m_player.PostProcessBeamTick -= new Action<BeamController, SpeculativeRigidbody, float>(this.PostProcessBeamTick);
    }
  }

