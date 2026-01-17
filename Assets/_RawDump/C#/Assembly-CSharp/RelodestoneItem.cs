// Decompiled with JetBrains decompiler
// Type: RelodestoneItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable
public class RelodestoneItem : PlayerItem
{
  public float EffectRadius = 10f;
  public float duration = 3f;
  public float GravitationalForce = 500f;
  public GameObject ContinuousVFX;
  public RadialBurstInterface RelodestarBurst;
  private PlayerController m_owner;
  private GameObject m_instanceVFX;
  private int m_totalAbsorbedDuringCycle;

  protected override void DoEffect(PlayerController user)
  {
    int num = (int) AkSoundEngine.PostEvent("Play_OBJ_ammo_suck_01", this.gameObject);
    if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES)
      return;
    this.m_owner = user;
    this.m_totalAbsorbedDuringCycle = 0;
    this.IsCurrentlyActive = true;
    this.m_activeElapsed = 0.0f;
    this.m_activeDuration = this.duration;
    this.m_instanceVFX = SpawnManager.SpawnVFX(this.ContinuousVFX);
    this.m_instanceVFX.transform.parent = user.transform;
    this.m_instanceVFX.transform.position = user.CenterPosition.ToVector3ZisY();
  }

  public override void OnItemSwitched(PlayerController user)
  {
    this.BecomeInactive();
    base.OnItemSwitched(user);
  }

  protected override void OnPreDrop(PlayerController user)
  {
    this.BecomeInactive();
    base.OnPreDrop(user);
  }

  private void BecomeInactive()
  {
    this.IsCurrentlyActive = false;
    if (this.m_totalAbsorbedDuringCycle > 0 && (bool) (Object) this.m_owner && this.m_owner.HasActiveBonusSynergy(CustomSynergyType.RELODESTAR))
    {
      int num1 = Mathf.CeilToInt((float) this.m_totalAbsorbedDuringCycle / 20f);
      int num2 = Mathf.CeilToInt((float) this.m_totalAbsorbedDuringCycle / (float) num1);
      this.RelodestarBurst.MinToSpawnPerWave = num2;
      this.RelodestarBurst.MaxToSpawnPerWave = num2;
      this.RelodestarBurst.NumberWaves = num1;
      this.RelodestarBurst.TimeBetweenWaves = 1f;
      this.RelodestarBurst.DoBurst(this.m_owner);
    }
    this.m_totalAbsorbedDuringCycle = 0;
    if (!(bool) (Object) this.m_instanceVFX)
      return;
    SpawnManager.Despawn(this.m_instanceVFX);
    this.m_instanceVFX = (GameObject) null;
  }

  private void LateUpdate()
  {
    if (Dungeon.IsGenerating || !this.IsActive || !(bool) (Object) this.m_owner)
      return;
    Vector2 centerPosition = this.m_owner.CenterPosition;
    int amt = 0;
    for (int index = 0; index < StaticReferenceManager.AllProjectiles.Count; ++index)
    {
      Projectile allProjectile = StaticReferenceManager.AllProjectiles[index];
      if ((bool) (Object) allProjectile && (bool) (Object) allProjectile.specRigidbody && this.AdjustRigidbodyVelocity(allProjectile.specRigidbody, centerPosition))
        ++amt;
    }
    this.m_totalAbsorbedDuringCycle += amt;
    if (amt > 0 && (bool) (Object) this.m_owner.CurrentGun && this.m_owner.CurrentGun.CanGainAmmo)
      this.m_owner.CurrentGun.GainAmmo(amt);
    if ((double) this.m_activeElapsed < (double) this.m_activeDuration)
      return;
    this.BecomeInactive();
  }

  private Vector2 GetFrameAccelerationForRigidbody(
    Vector2 unitCenter,
    Vector2 myCenter,
    float currentDistance,
    float g)
  {
    Vector2 zero = Vector2.zero;
    float num1 = Mathf.Clamp01((float) (1.0 - (double) currentDistance / (double) this.EffectRadius));
    float num2 = g * num1 * num1;
    return (myCenter - unitCenter).normalized * num2;
  }

  private bool AdjustRigidbodyVelocity(SpeculativeRigidbody other, Vector2 myCenter)
  {
    bool flag = false;
    Vector2 a = other.UnitCenter - myCenter;
    float effectRadius = this.EffectRadius;
    float f = Vector2.SqrMagnitude(a);
    if ((double) f >= (double) effectRadius)
      return flag;
    float gravitationalForce = this.GravitationalForce;
    Vector2 vector2_1 = other.Velocity;
    Projectile projectile = other.projectile;
    if (!(bool) (Object) projectile || projectile.Owner is PlayerController)
      return false;
    if ((bool) (Object) projectile.GetComponent<ChainLightningModifier>())
      Object.Destroy((Object) projectile.GetComponent<ChainLightningModifier>());
    projectile.collidesWithPlayer = false;
    if ((Object) other.GetComponent<BlackHoleDoer>() != (Object) null)
      return false;
    if (vector2_1 == Vector2.zero)
    {
      if (!projectile.IsBulletScript)
        return false;
      Vector2 vector2_2 = myCenter - other.UnitCenter;
      if (vector2_2 == Vector2.zero)
        vector2_2 = new Vector2(Random.value - 0.5f, Random.value - 0.5f);
      vector2_1 = vector2_2.normalized * 3f;
    }
    if ((double) f < 2.0)
    {
      projectile.DieInAir();
      flag = true;
    }
    Vector2 vector2_3 = this.GetFrameAccelerationForRigidbody(other.UnitCenter, myCenter, Mathf.Sqrt(f), gravitationalForce) * Mathf.Clamp(BraveTime.DeltaTime, 0.0f, 0.02f);
    Vector2 vector2_4 = vector2_1 + vector2_3;
    if ((double) BraveTime.DeltaTime > 0.019999999552965164)
      vector2_4 *= 0.02f / BraveTime.DeltaTime;
    other.Velocity = vector2_4;
    if ((Object) projectile != (Object) null)
    {
      projectile.collidesWithPlayer = false;
      if (projectile.IsBulletScript)
        projectile.RemoveBulletScriptControl();
      if (vector2_4 != Vector2.zero)
      {
        projectile.Direction = vector2_4.normalized;
        projectile.Speed = Mathf.Max(3f, vector2_4.magnitude);
        other.Velocity = projectile.Direction * projectile.Speed;
        if (projectile.shouldRotate && ((double) vector2_4.x != 0.0 || (double) vector2_4.y != 0.0))
        {
          float num = BraveMathCollege.Atan2Degrees(projectile.Direction);
          if (!float.IsNaN(num) && !float.IsInfinity(num))
          {
            Quaternion quaternion = Quaternion.Euler(0.0f, 0.0f, num);
            if (!float.IsNaN(quaternion.x) && !float.IsNaN(quaternion.y))
              projectile.transform.rotation = quaternion;
          }
        }
      }
    }
    return flag;
  }
}
