// Decompiled with JetBrains decompiler
// Type: ThrownGoopItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

public class ThrownGoopItem : MonoBehaviour
  {
    public GoopDefinition goop;
    public float goopRadius = 3f;
    public bool CreatesProjectiles;
    public int NumProjectiles;
    public Projectile SourceProjectile;
    public bool UsesSynergyOverrideProjectile;
    public CustomSynergyType SynergyToCheck;
    public Projectile SynergyProjectile;
    public string burstAnim;
    public VFXPool burstVFX;

    private void Start()
    {
      int num = (int) AkSoundEngine.PostEvent("Play_OBJ_item_throw_01", this.gameObject);
      DebrisObject component = this.GetComponent<DebrisObject>();
      component.killTranslationOnBounce = false;
      if (!(bool) (UnityEngine.Object) component)
        return;
      component.OnBounced += new Action<DebrisObject>(this.OnBounced);
      component.OnGrounded += new Action<DebrisObject>(this.OnHitGround);
    }

    private void OnBounced(DebrisObject obj)
    {
      if ((UnityEngine.Object) this.goop != (UnityEngine.Object) null)
        DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.goop).TimedAddGoopCircle(obj.sprite.WorldCenter, this.goopRadius);
      if (!this.CreatesProjectiles)
        return;
      float max = 360f / (float) this.NumProjectiles;
      float num = UnityEngine.Random.Range(0.0f, max);
      Projectile projectile = this.SourceProjectile;
      if (this.UsesSynergyOverrideProjectile && GameManager.Instance.PrimaryPlayer.HasActiveBonusSynergy(this.SynergyToCheck))
        projectile = this.SynergyProjectile;
      for (int index = 0; index < this.NumProjectiles; ++index)
      {
        float z = num + max * (float) index;
        Projectile component = SpawnManager.SpawnProjectile(projectile.gameObject, (Vector3) obj.sprite.WorldCenter, Quaternion.Euler(0.0f, 0.0f, z)).GetComponent<Projectile>();
        component.Owner = (GameActor) GameManager.Instance.PrimaryPlayer;
        component.Shooter = GameManager.Instance.PrimaryPlayer.specRigidbody;
        component.collidesWithPlayer = false;
        component.collidesWithEnemies = true;
      }
    }

    private void OnHitGround(DebrisObject obj)
    {
      int num = (int) AkSoundEngine.PostEvent("Play_WPN_molotov_impact_01", this.gameObject);
      this.OnBounced(obj);
      this.burstVFX.SpawnAtPosition((Vector3) this.GetComponent<tk2dSprite>().WorldCenter);
      if (!string.IsNullOrEmpty(this.burstAnim))
        this.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject(this.burstAnim);
      else
        UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }
  }

