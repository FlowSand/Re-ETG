// Decompiled with JetBrains decompiler
// Type: KnifeShieldEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using UnityEngine;

#nullable disable

public class KnifeShieldEffect : BraveBehaviour
  {
    public int numKnives = 5;
    public float knifeDamage = 5f;
    public float circleRadius = 3f;
    public float rotationDegreesPerSecond = 360f;
    public float throwSpeed = 3f;
    public float throwRange = 25f;
    public float throwRadius = 3f;
    public float radiusChangeDistance = 3f;
    public float remainingHealth;
    public GameObject deathVFX;
    private bool m_lightknife;
    protected PlayerController m_player;
    protected SpeculativeRigidbody[] m_knives;
    protected float m_elapsed;
    protected float m_traversedDistance;
    protected Vector3 m_currentShieldVelocity = Vector3.zero;
    protected Vector3 m_currentShieldCenterOffset = Vector3.zero;
    private Shader m_lightknivesShader;
    private Vector3 m_cachedOffsetBase;

    public bool IsActive
    {
      get
      {
        if (this.m_currentShieldVelocity != Vector3.zero)
          return false;
        for (int index = 0; index < this.m_knives.Length; ++index)
        {
          if ((UnityEngine.Object) this.m_knives[index] != (UnityEngine.Object) null)
            return true;
        }
        return false;
      }
    }

    public void Initialize(PlayerController player, GameObject knifePrefab)
    {
      this.m_player = player;
      this.m_knives = new SpeculativeRigidbody[this.numKnives];
      this.m_lightknife = player.HasActiveBonusSynergy(CustomSynergyType.LIGHTKNIVES);
      for (int index = 0; index < this.numKnives; ++index)
      {
        Vector3 position = player.LockedApproximateSpriteCenter + Quaternion.Euler(0.0f, 0.0f, 360f / (float) this.numKnives * (float) index) * Vector3.right * this.circleRadius;
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(knifePrefab, position, Quaternion.identity);
        tk2dSprite component1 = gameObject.GetComponent<tk2dSprite>();
        component1.HeightOffGround = 1.5f;
        tk2dSpriteAnimator component2 = gameObject.GetComponent<tk2dSpriteAnimator>();
        component2.PlayFromFrame(UnityEngine.Random.Range(0, component2.DefaultClip.frames.Length));
        if (this.m_lightknife)
          this.SetOverrideShader(component1);
        SpeculativeRigidbody component3 = gameObject.GetComponent<SpeculativeRigidbody>();
        component3.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollision);
        component3.OnTriggerCollision += new SpeculativeRigidbody.OnTriggerDelegate(this.HandleCollision);
        component3.OnTileCollision += new SpeculativeRigidbody.OnTileCollisionDelegate(this.HandleTileCollision);
        this.m_knives[index] = component3;
      }
    }

    private void SetOverrideShader(tk2dSprite spr)
    {
      spr.usesOverrideMaterial = true;
      Material material = spr.GetComponent<MeshRenderer>().material;
      if ((UnityEngine.Object) this.m_lightknivesShader == (UnityEngine.Object) null)
        this.m_lightknivesShader = Shader.Find("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
      material.shader = this.m_lightknivesShader;
      material.SetColor("_OverrideColor", new Color(0.392156869f, 0.8235294f, 0.470588237f));
      material.SetFloat("_EmissivePower", 130f);
    }

    private void HandleTileCollision(CollisionData tileCollision)
    {
      int index = Array.IndexOf<SpeculativeRigidbody>(this.m_knives, tileCollision.MyRigidbody);
      if (index != -1)
        this.m_knives[index] = (SpeculativeRigidbody) null;
      tileCollision.MyRigidbody.sprite.PlayEffectOnSprite(this.deathVFX, Vector3.zero, false);
      UnityEngine.Object.Destroy((UnityEngine.Object) tileCollision.MyRigidbody.gameObject);
    }

    public void ThrowShield()
    {
      int num = (int) AkSoundEngine.PostEvent("Play_OBJ_daggershield_shot_01", this.gameObject);
      if (!(this.m_currentShieldVelocity == Vector3.zero))
        return;
      this.m_currentShieldVelocity = (this.m_player.unadjustedAimPoint - (Vector3) this.m_player.CenterPosition).WithZ(0.0f).normalized * this.throwSpeed;
      for (int index = 0; index < this.m_knives.Length; ++index)
      {
        if ((UnityEngine.Object) this.m_knives[index] != (UnityEngine.Object) null && (bool) (UnityEngine.Object) this.m_knives[index])
          this.m_knives[index].specRigidbody.CollideWithTileMap = true;
      }
    }

    protected Vector3 GetTargetPositionForKniveID(Vector3 center, int i, float radiusToUse)
    {
      float num = (float) ((double) this.rotationDegreesPerSecond * (double) this.m_elapsed % 360.0);
      return center + Quaternion.Euler(0.0f, 0.0f, num + 360f / (float) this.numKnives * (float) i) * Vector3.right * radiusToUse;
    }

    private void OnPreCollision(
      SpeculativeRigidbody myRigidbody,
      PixelCollider myCollider,
      SpeculativeRigidbody other,
      PixelCollider otherCollider)
    {
      Projectile component1 = other.GetComponent<Projectile>();
      if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
      {
        if (component1.Owner is PlayerController)
          PhysicsEngine.SkipCollision = true;
        else if (this.m_lightknife)
        {
          PassiveReflectItem.ReflectBullet(component1, true, (GameActor) this.m_player, 10f);
          this.DestroyKnife(myRigidbody);
        }
      }
      GameActor component2 = other.GetComponent<GameActor>();
      if (component2 is PlayerController)
        PhysicsEngine.SkipCollision = true;
      if (!(component2 is AIActor) || (component2 as AIActor).IsNormalEnemy)
        return;
      PhysicsEngine.SkipCollision = true;
    }

    private void HandleCollision(
      SpeculativeRigidbody other,
      SpeculativeRigidbody source,
      CollisionData collisionData)
    {
      if ((UnityEngine.Object) other.GetComponent<AIActor>() != (UnityEngine.Object) null)
      {
        HealthHaver component = other.GetComponent<HealthHaver>();
        float knifeDamage = this.knifeDamage;
        if (this.m_lightknife)
          knifeDamage *= 3f;
        component.ApplyDamage(knifeDamage, Vector2.zero, "Knife Shield");
        int index = Array.IndexOf<SpeculativeRigidbody>(this.m_knives, source);
        if (index != -1)
          this.m_knives[index] = (SpeculativeRigidbody) null;
        source.sprite.PlayEffectOnSprite(this.deathVFX, Vector3.zero, false);
        UnityEngine.Object.Destroy((UnityEngine.Object) source.gameObject);
      }
      else
      {
        if (!((UnityEngine.Object) other.GetComponent<Projectile>() != (UnityEngine.Object) null))
          return;
        Projectile component = other.GetComponent<Projectile>();
        if (component.Owner is PlayerController)
          return;
        if (!this.m_lightknife)
          component.DieInAir();
        this.remainingHealth -= component.ModifiedDamage;
        if ((double) this.remainingHealth > 0.0)
          return;
        this.DestroyKnife(source);
      }
    }

    private void DestroyKnife(SpeculativeRigidbody source)
    {
      int index = Array.IndexOf<SpeculativeRigidbody>(this.m_knives, source);
      if (index != -1)
        this.m_knives[index] = (SpeculativeRigidbody) null;
      source.sprite.PlayEffectOnSprite(this.deathVFX, Vector3.zero, false);
      UnityEngine.Object.Destroy((UnityEngine.Object) source.gameObject);
    }

    private void Update()
    {
      if (GameManager.Instance.IsLoadingLevel || Dungeon.IsGenerating)
        return;
      this.m_elapsed += BraveTime.DeltaTime;
      bool flag = this.m_currentShieldVelocity != Vector3.zero;
      Vector3 vector3 = this.m_currentShieldVelocity * BraveTime.DeltaTime;
      this.m_currentShieldCenterOffset += vector3;
      if (!flag)
      {
        this.m_cachedOffsetBase = this.m_player.LockedApproximateSpriteCenter;
      }
      else
      {
        this.m_traversedDistance += vector3.magnitude;
        if ((double) this.m_traversedDistance > (double) this.throwRange)
        {
          for (int index = 0; index < this.m_knives.Length; ++index)
          {
            if ((UnityEngine.Object) this.m_knives[index] != (UnityEngine.Object) null && (bool) (UnityEngine.Object) this.m_knives[index])
            {
              this.m_knives[index].sprite.PlayEffectOnSprite(this.deathVFX, Vector3.zero, false);
              UnityEngine.Object.Destroy((UnityEngine.Object) this.m_knives[index].gameObject);
              this.m_knives[index] = (SpeculativeRigidbody) null;
            }
          }
        }
      }
      Vector3 center = this.m_cachedOffsetBase + this.m_currentShieldCenterOffset;
      float radiusToUse = this.circleRadius;
      if (flag)
        radiusToUse = Mathf.Lerp(this.circleRadius, this.throwRadius, this.m_traversedDistance / this.radiusChangeDistance);
      for (int i = 0; i < this.numKnives; ++i)
      {
        if ((UnityEngine.Object) this.m_knives[i] != (UnityEngine.Object) null && (bool) (UnityEngine.Object) this.m_knives[i])
        {
          Vector2 vector2 = (this.GetTargetPositionForKniveID(center, i, radiusToUse) - this.m_knives[i].transform.position).XY() / BraveTime.DeltaTime;
          this.m_knives[i].Velocity = vector2;
          this.m_knives[i].sprite.UpdateZDepth();
        }
      }
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

