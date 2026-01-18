// Decompiled with JetBrains decompiler
// Type: StickyGrenadeBuff
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

public class StickyGrenadeBuff : AppliedEffectBase
  {
    public bool IsSynergyContingent;
    [LongNumericEnum]
    public CustomSynergyType RequiredSynergy;
    public ExplosionData explosionData;
    public GameObject vfx;
    private GameObject instantiatedVFX;
    private PlayerController m_player;
    private Gun m_attachedGun;
    private HealthHaver hh;
    private Vector2 m_cachedSourceVector = Vector2.zero;

    private void InitializeSelf(StickyGrenadeBuff source)
    {
      if (!(bool) (UnityEngine.Object) source)
        return;
      this.explosionData = source.explosionData;
      this.hh = this.GetComponent<HealthHaver>();
      if ((UnityEngine.Object) this.hh != (UnityEngine.Object) null)
      {
        Projectile component = source.GetComponent<Projectile>();
        if ((UnityEngine.Object) component.PossibleSourceGun != (UnityEngine.Object) null)
        {
          this.m_attachedGun = component.PossibleSourceGun;
          this.m_player = component.PossibleSourceGun.CurrentOwner as PlayerController;
          component.PossibleSourceGun.OnReloadPressed += new Action<PlayerController, Gun, bool>(this.ExplodeOnReload);
          if (!(bool) (UnityEngine.Object) this.m_player)
            return;
          this.m_player.GunChanged += new Action<Gun, Gun, bool>(this.GunChanged);
        }
        else
        {
          if (!(bool) (UnityEngine.Object) component || !(bool) (UnityEngine.Object) component.Owner || !(bool) (UnityEngine.Object) component.Owner.CurrentGun)
            return;
          this.m_attachedGun = component.Owner.CurrentGun;
          this.m_player = component.Owner as PlayerController;
          component.Owner.CurrentGun.OnReloadPressed += new Action<PlayerController, Gun, bool>(this.ExplodeOnReload);
          if (!(bool) (UnityEngine.Object) this.m_player)
            return;
          this.m_player.GunChanged += new Action<Gun, Gun, bool>(this.GunChanged);
        }
      }
      else
        UnityEngine.Object.Destroy((UnityEngine.Object) this);
    }

    private void Disconnect()
    {
      if ((bool) (UnityEngine.Object) this.m_player)
        this.m_player.GunChanged -= new Action<Gun, Gun, bool>(this.GunChanged);
      if (!(bool) (UnityEngine.Object) this.m_attachedGun)
        return;
      this.m_attachedGun.OnReloadPressed -= new Action<PlayerController, Gun, bool>(this.ExplodeOnReload);
    }

    private void GunChanged(Gun arg1, Gun arg2, bool newGun)
    {
      this.Disconnect();
      this.DoEffect();
    }

    private void ExplodeOnReload(PlayerController arg1, Gun arg2, bool actual)
    {
      this.Disconnect();
      this.DoEffect();
    }

    public override void Initialize(AppliedEffectBase source)
    {
      if (source is StickyGrenadeBuff)
      {
        StickyGrenadeBuff source1 = source as StickyGrenadeBuff;
        this.InitializeSelf(source1);
        if (!((UnityEngine.Object) source1.vfx != (UnityEngine.Object) null))
          return;
        this.instantiatedVFX = SpawnManager.SpawnVFX(source1.vfx, this.transform.position, Quaternion.identity, true);
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
        Projectile component4 = source.GetComponent<Projectile>();
        if ((bool) (UnityEngine.Object) component4 && component4.LastVelocity != Vector2.zero)
        {
          this.m_cachedSourceVector = component4.LastVelocity;
          component3.InitializePierce(this.GetComponent<GameActor>(), component4.LastVelocity);
        }
        else
          component3.Initialize(this.GetComponent<GameActor>());
      }
      else
        UnityEngine.Object.Destroy((UnityEngine.Object) this);
    }

    public override void AddSelfToTarget(GameObject target)
    {
      if ((UnityEngine.Object) target.GetComponent<HealthHaver>() == (UnityEngine.Object) null)
        return;
      if (this.IsSynergyContingent)
      {
        Projectile component = this.GetComponent<Projectile>();
        if (!(bool) (UnityEngine.Object) component || !(component.Owner is PlayerController) || !(component.Owner as PlayerController).HasActiveBonusSynergy(this.RequiredSynergy))
          return;
      }
      target.AddComponent<StickyGrenadeBuff>().Initialize((AppliedEffectBase) this);
    }

    private void DoEffect()
    {
      if ((bool) (UnityEngine.Object) this.hh)
      {
        float force = this.explosionData.force / 4f;
        this.explosionData.force = 0.0f;
        if ((UnityEngine.Object) this.instantiatedVFX != (UnityEngine.Object) null)
          Exploder.Explode((Vector3) (this.instantiatedVFX.GetComponent<tk2dBaseSprite>().WorldCenter + this.m_cachedSourceVector.normalized * -0.5f), this.explosionData, Vector2.zero, ignoreQueues: true);
        else
          Exploder.Explode((Vector3) this.hh.aiActor.CenterPosition, this.explosionData, Vector2.zero, ignoreQueues: true);
        if ((bool) (UnityEngine.Object) this.hh.knockbackDoer && this.m_cachedSourceVector != Vector2.zero)
          this.hh.knockbackDoer.ApplyKnockback(this.m_cachedSourceVector.normalized, force);
      }
      if ((bool) (UnityEngine.Object) this.instantiatedVFX)
        UnityEngine.Object.Destroy((UnityEngine.Object) this.instantiatedVFX);
      UnityEngine.Object.Destroy((UnityEngine.Object) this);
    }

    private void OnDestroy() => this.Disconnect();
  }

