// Decompiled with JetBrains decompiler
// Type: ChainLightningModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class ChainLightningModifier : BraveBehaviour
{
  public GameObject LinkVFXPrefab;
  public CoreDamageTypes damageTypes;
  public bool RequiresSameProjectileClass;
  public float maximumLinkDistance = 8f;
  public float damagePerHit = 5f;
  public float damageCooldown = 1f;
  [NonSerialized]
  public bool CanChainToAnyProjectile;
  [NonSerialized]
  public bool UseForcedLinkProjectile;
  [NonSerialized]
  public Projectile ForcedLinkProjectile;
  [NonSerialized]
  public Projectile BackLinkProjectile;
  [NonSerialized]
  public bool DamagesPlayers;
  [NonSerialized]
  public bool DamagesEnemies = true;
  [Header("Dispersal")]
  public bool UsesDispersalParticles;
  [ShowInInspectorIf("UsesDispersalParticles", false)]
  public float DispersalDensity = 3f;
  [ShowInInspectorIf("UsesDispersalParticles", false)]
  public float DispersalMinCoherency = 0.2f;
  [ShowInInspectorIf("UsesDispersalParticles", false)]
  public float DispersalMaxCoherency = 1f;
  [ShowInInspectorIf("UsesDispersalParticles", false)]
  public GameObject DispersalParticleSystemPrefab;
  private Projectile m_frameLinkProjectile;
  private tk2dTiledSprite m_extantLink;
  private bool m_hasSetBlackBullet;
  private ParticleSystem m_dispersalParticles;
  private HashSet<AIActor> m_damagedEnemies = new HashSet<AIActor>();

  private void Start()
  {
    PhysicsEngine.Instance.OnPostRigidbodyMovement += new System.Action(this.PostRigidbodyUpdate);
  }

  protected override void OnDestroy()
  {
    if ((UnityEngine.Object) PhysicsEngine.Instance != (UnityEngine.Object) null)
      PhysicsEngine.Instance.OnPostRigidbodyMovement -= new System.Action(this.PostRigidbodyUpdate);
    this.ClearLink();
    if ((bool) (UnityEngine.Object) this.BackLinkProjectile && (bool) (UnityEngine.Object) this.BackLinkProjectile.GetComponent<ChainLightningModifier>())
    {
      ChainLightningModifier component = this.BackLinkProjectile.GetComponent<ChainLightningModifier>();
      component.ClearLink();
      component.ForcedLinkProjectile = (Projectile) null;
    }
    base.OnDestroy();
  }

  private void Update() => this.m_frameLinkProjectile = (Projectile) null;

  private void UpdateLinkToProjectile(Projectile targetProjectile)
  {
    if ((UnityEngine.Object) this.m_extantLink == (UnityEngine.Object) null)
    {
      this.m_extantLink = SpawnManager.SpawnVFX(this.LinkVFXPrefab).GetComponent<tk2dTiledSprite>();
      int count = -1;
      if (this.DamagesPlayers && !this.m_hasSetBlackBullet)
      {
        this.m_hasSetBlackBullet = true;
        Material material = this.m_extantLink.GetComponent<Renderer>().material;
        material.SetFloat("_BlackBullet", 0.995f);
        material.SetFloat("_EmissiveColorPower", 4.9f);
      }
      else if (!this.DamagesPlayers && PlayerController.AnyoneHasActiveBonusSynergy(CustomSynergyType.TESLA_UNBOUND, out count))
      {
        Material material = this.m_extantLink.GetComponent<Renderer>().material;
        material.SetFloat("_BlackBullet", 0.15f);
        material.SetFloat("_EmissiveColorPower", 0.1f);
      }
    }
    this.m_frameLinkProjectile = targetProjectile;
    Vector2 unitCenter1 = this.projectile.specRigidbody.UnitCenter;
    Vector2 unitCenter2 = targetProjectile.specRigidbody.UnitCenter;
    this.m_extantLink.transform.position = (Vector3) unitCenter1;
    Vector2 vector2 = unitCenter2 - unitCenter1;
    float z = BraveMathCollege.Atan2Degrees(vector2.normalized);
    this.m_extantLink.dimensions = new Vector2((float) Mathf.RoundToInt(vector2.magnitude / (1f / 16f)), this.m_extantLink.dimensions.y);
    this.m_extantLink.transform.rotation = Quaternion.Euler(0.0f, 0.0f, z);
    this.m_extantLink.UpdateZDepth();
    if (!this.ApplyLinearDamage(unitCenter1, unitCenter2) || !this.UsesDispersalParticles)
      return;
    this.DoDispersalParticles(unitCenter2, unitCenter1);
  }

  private void DoDispersalParticles(Vector2 posStart, Vector2 posEnd)
  {
    if (GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.LOW || GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.VERY_LOW)
      return;
    if (!(bool) (UnityEngine.Object) this.m_dispersalParticles)
      this.m_dispersalParticles = GlobalDispersalParticleManager.GetSystemForPrefab(this.DispersalParticleSystemPrefab);
    int num = Mathf.Max(Mathf.CeilToInt(Vector2.Distance(posStart, posEnd) * this.DispersalDensity), 1);
    for (int index = 0; index < num; ++index)
    {
      float t = (float) index / (float) num;
      Vector3 vector3_1 = Vector3.Lerp((Vector3) posStart, (Vector3) posEnd, t) + Vector3.back;
      Vector3 vector3_2 = Vector3.Lerp(Quaternion.Euler(0.0f, 0.0f, Mathf.PerlinNoise(vector3_1.x / 3f, vector3_1.y / 3f) * 360f) * Vector3.right, UnityEngine.Random.insideUnitSphere, UnityEngine.Random.Range(this.DispersalMinCoherency, this.DispersalMaxCoherency));
      this.m_dispersalParticles.Emit(new ParticleSystem.EmitParams()
      {
        position = vector3_1,
        velocity = vector3_2 * this.m_dispersalParticles.startSpeed,
        startSize = this.m_dispersalParticles.startSize,
        startLifetime = this.m_dispersalParticles.startLifetime,
        startColor = (Color32) this.m_dispersalParticles.startColor
      }, 1);
    }
  }

  [DebuggerHidden]
  private IEnumerator HandleDamageCooldown(AIActor damagedTarget)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ChainLightningModifier.\u003CHandleDamageCooldown\u003Ec__Iterator0()
    {
      damagedTarget = damagedTarget,
      \u0024this = this
    };
  }

  private bool ApplyLinearDamage(Vector2 p1, Vector2 p2)
  {
    bool flag = false;
    if (this.DamagesEnemies)
    {
      for (int index = 0; index < StaticReferenceManager.AllEnemies.Count; ++index)
      {
        AIActor allEnemy = StaticReferenceManager.AllEnemies[index];
        if (!this.m_damagedEnemies.Contains(allEnemy) && (bool) (UnityEngine.Object) allEnemy && allEnemy.HasBeenEngaged && allEnemy.IsNormalEnemy && (bool) (UnityEngine.Object) allEnemy.specRigidbody)
        {
          Vector2 intersection = Vector2.zero;
          if (BraveUtility.LineIntersectsAABB(p1, p2, allEnemy.specRigidbody.HitboxPixelCollider.UnitBottomLeft, allEnemy.specRigidbody.HitboxPixelCollider.UnitDimensions, out intersection))
          {
            allEnemy.healthHaver.ApplyDamage(this.damagePerHit, Vector2.zero, "Chain Lightning", this.damageTypes);
            flag = true;
            GameManager.Instance.StartCoroutine(this.HandleDamageCooldown(allEnemy));
          }
        }
      }
    }
    if (this.DamagesPlayers)
    {
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      {
        PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
        if ((bool) (UnityEngine.Object) allPlayer && !allPlayer.IsGhost && (bool) (UnityEngine.Object) allPlayer.healthHaver && allPlayer.healthHaver.IsAlive && allPlayer.healthHaver.IsVulnerable)
        {
          Vector2 intersection = Vector2.zero;
          if (BraveUtility.LineIntersectsAABB(p1, p2, allPlayer.specRigidbody.HitboxPixelCollider.UnitBottomLeft, allPlayer.specRigidbody.HitboxPixelCollider.UnitDimensions, out intersection))
          {
            allPlayer.healthHaver.ApplyDamage(0.5f, Vector2.zero, this.projectile.OwnerName, this.damageTypes);
            flag = true;
          }
        }
      }
    }
    return flag;
  }

  private void ClearLink()
  {
    if (!((UnityEngine.Object) this.m_extantLink != (UnityEngine.Object) null))
      return;
    SpawnManager.Despawn(this.m_extantLink.gameObject);
    this.m_extantLink = (tk2dTiledSprite) null;
  }

  private Projectile GetLinkProjectile()
  {
    Projectile projectile = (Projectile) null;
    float num1 = float.MaxValue;
    float num2 = this.maximumLinkDistance * this.maximumLinkDistance;
    for (int index = 0; index < StaticReferenceManager.AllProjectiles.Count; ++index)
    {
      Projectile allProjectile = StaticReferenceManager.AllProjectiles[index];
      if ((bool) (UnityEngine.Object) allProjectile && (UnityEngine.Object) allProjectile != (UnityEngine.Object) this.projectile && (this.CanChainToAnyProjectile || (UnityEngine.Object) allProjectile.Owner == (UnityEngine.Object) this.projectile.Owner))
      {
        if (this.RequiresSameProjectileClass && !this.CanChainToAnyProjectile)
        {
          if ((bool) (UnityEngine.Object) this.projectile.spriteAnimator && (bool) (UnityEngine.Object) allProjectile.spriteAnimator)
          {
            if (this.projectile.spriteAnimator.CurrentClip != allProjectile.spriteAnimator.CurrentClip)
              continue;
          }
          else if ((bool) (UnityEngine.Object) this.projectile.spriteAnimator || (bool) (UnityEngine.Object) allProjectile.spriteAnimator)
            continue;
          if ((bool) (UnityEngine.Object) this.projectile.sprite && (bool) (UnityEngine.Object) allProjectile.sprite)
          {
            if (allProjectile.sprite.spriteId != this.projectile.sprite.spriteId || (UnityEngine.Object) allProjectile.sprite.Collection != (UnityEngine.Object) this.projectile.sprite.Collection)
              continue;
          }
          else if ((bool) (UnityEngine.Object) this.projectile.sprite || (bool) (UnityEngine.Object) allProjectile.sprite)
            continue;
        }
        ChainLightningModifier component = allProjectile.GetComponent<ChainLightningModifier>();
        if ((bool) (UnityEngine.Object) component && (UnityEngine.Object) component.m_frameLinkProjectile == (UnityEngine.Object) null)
        {
          float sqrMagnitude = (component.specRigidbody.UnitCenter - this.specRigidbody.UnitCenter).sqrMagnitude;
          if ((double) sqrMagnitude < (double) num1 && (double) sqrMagnitude < (double) num2)
          {
            projectile = allProjectile;
            num1 = sqrMagnitude;
          }
        }
        else if (this.CanChainToAnyProjectile && (bool) (UnityEngine.Object) allProjectile && (bool) (UnityEngine.Object) allProjectile.specRigidbody && (bool) (UnityEngine.Object) this && (bool) (UnityEngine.Object) this.specRigidbody)
        {
          float sqrMagnitude = (allProjectile.specRigidbody.UnitCenter - this.specRigidbody.UnitCenter).sqrMagnitude;
          if ((double) sqrMagnitude < (double) num1 && (double) sqrMagnitude < (double) num2)
          {
            projectile = allProjectile;
            num1 = sqrMagnitude;
          }
        }
      }
    }
    return (UnityEngine.Object) projectile == (UnityEngine.Object) null ? (Projectile) null : projectile;
  }

  private void PostRigidbodyUpdate()
  {
    if ((bool) (UnityEngine.Object) this.projectile)
    {
      Projectile targetProjectile = !this.UseForcedLinkProjectile ? this.GetLinkProjectile() : this.ForcedLinkProjectile;
      if ((bool) (UnityEngine.Object) targetProjectile)
        this.UpdateLinkToProjectile(targetProjectile);
      else
        this.ClearLink();
    }
    else
      this.ClearLink();
  }
}
