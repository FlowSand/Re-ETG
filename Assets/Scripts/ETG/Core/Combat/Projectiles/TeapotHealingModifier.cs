// Decompiled with JetBrains decompiler
// Type: TeapotHealingModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class TeapotHealingModifier : MonoBehaviour
  {
    public int AmmoCost = 24;
    private Projectile m_projectile;

    private void Awake()
    {
      this.m_projectile = this.GetComponent<Projectile>();
      this.m_projectile.allowSelfShooting = true;
      this.m_projectile.collidesWithEnemies = true;
      this.m_projectile.collidesWithPlayer = true;
      this.m_projectile.UpdateCollisionMask();
      this.m_projectile.specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandlePreCollision);
    }

    private void HandlePreCollision(
      SpeculativeRigidbody myRigidbody,
      PixelCollider myPixelCollider,
      SpeculativeRigidbody otherRigidbody,
      PixelCollider otherPixelCollider)
    {
      if (!(bool) (Object) otherRigidbody)
        return;
      PlayerController component = otherRigidbody.GetComponent<PlayerController>();
      if ((bool) (Object) component && (Object) component != (Object) this.m_projectile.Owner && !component.IsGhost)
      {
        if ((bool) (Object) this.m_projectile.PossibleSourceGun)
        {
          component.healthHaver.ApplyHealing(0.5f);
          int num = (int) AkSoundEngine.PostEvent("Play_OBJ_heart_heal_01", this.gameObject);
          GameObject effect = BraveResources.Load<GameObject>("Global VFX/VFX_Healing_Sparkles_001");
          if ((Object) effect != (Object) null)
            component.PlayEffectOnActor(effect, Vector3.zero);
          this.m_projectile.PossibleSourceGun.LoseAmmo(this.AmmoCost);
          this.m_projectile.DieInAir();
        }
        PhysicsEngine.SkipCollision = true;
      }
      else
      {
        if (!(bool) (Object) component)
          return;
        PhysicsEngine.SkipCollision = true;
      }
    }
  }

