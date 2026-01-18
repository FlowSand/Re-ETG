using Dungeonator;
using UnityEngine;

#nullable disable

public class EnemyBulletsBecomeJammedModifier : MonoBehaviour
  {
    public float EffectRadius = 1f;
    private Projectile m_projectile;
    private tk2dBaseSprite m_sprite;

    private void Start()
    {
      this.m_projectile = this.GetComponent<Projectile>();
      this.m_sprite = this.m_projectile.sprite;
    }

    private void Update()
    {
      if (Dungeon.IsGenerating)
        return;
      Vector2 vector2 = !(bool) (Object) this.m_sprite ? this.m_projectile.transform.position.XY() : this.m_sprite.WorldCenter;
      for (int index = 0; index < StaticReferenceManager.AllProjectiles.Count; ++index)
      {
        Projectile allProjectile = StaticReferenceManager.AllProjectiles[index];
        if ((bool) (Object) allProjectile && allProjectile.Owner is AIActor && !allProjectile.IsBlackBullet && (double) (allProjectile.transform.position.XY() - vector2).sqrMagnitude < (double) this.EffectRadius)
          allProjectile.BecomeBlackBullet();
      }
    }
  }

