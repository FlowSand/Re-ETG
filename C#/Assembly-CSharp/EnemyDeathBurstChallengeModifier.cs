// Decompiled with JetBrains decompiler
// Type: EnemyDeathBurstChallengeModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;

#nullable disable
public class EnemyDeathBurstChallengeModifier : ChallengeModifier
{
  public BulletScriptSelector DeathBulletScript;
  public Projectile DefaultFallbackProjectile;

  private void Start()
  {
    for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      GameManager.Instance.AllPlayers[index].OnAnyEnemyReceivedDamage += new Action<float, bool, HealthHaver>(this.OnEnemyDamaged);
  }

  private void OnEnemyDamaged(float damage, bool fatal, HealthHaver enemyHealth)
  {
    if (!(bool) (UnityEngine.Object) enemyHealth || enemyHealth.IsBoss || !fatal || !(bool) (UnityEngine.Object) enemyHealth.aiActor || !enemyHealth.aiActor.IsNormalEnemy)
      return;
    string name = enemyHealth.name;
    if (name.StartsWith("Bashellisk") || name.StartsWith("Blobulin") || name.StartsWith("Poisbulin"))
      return;
    this.SetDeathBurst(enemyHealth);
  }

  private void OnDestroy()
  {
    for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      GameManager.Instance.AllPlayers[index].OnAnyEnemyReceivedDamage -= new Action<float, bool, HealthHaver>(this.OnEnemyDamaged);
  }

  public override bool IsValid(RoomHandler room)
  {
    return room.area.PrototypeRoomCategory != PrototypeDungeonRoom.RoomCategory.BOSS && base.IsValid(room);
  }

  private void SetDeathBurst(HealthHaver healthHaver)
  {
    AIActor aiActor = healthHaver.aiActor;
    if (!(bool) (UnityEngine.Object) aiActor || !aiActor.IsNormalEnemy || !(bool) (UnityEngine.Object) aiActor.healthHaver || aiActor.healthHaver.IsBoss)
      return;
    if (!healthHaver.spawnBulletScript)
    {
      if (!(bool) (UnityEngine.Object) healthHaver.bulletBank)
        return;
      AIBulletBank.Entry bullet = healthHaver.bulletBank.GetBullet();
      if (bullet == null)
      {
        AIBulletBank.Entry entry = new AIBulletBank.Entry()
        {
          Name = "default",
          BulletObject = this.DefaultFallbackProjectile.gameObject,
          ProjectileData = new ProjectileData()
        };
        entry.ProjectileData.onDestroyBulletScript = new BulletScriptSelector();
        healthHaver.bulletBank.Bullets.Add(entry);
      }
      else if ((UnityEngine.Object) bullet.BulletObject == (UnityEngine.Object) null)
        bullet.BulletObject = this.DefaultFallbackProjectile.gameObject;
      healthHaver.spawnBulletScript = true;
      healthHaver.chanceToSpawnBulletScript = 1f;
      healthHaver.bulletScriptType = HealthHaver.BulletScriptType.OnPreDeath;
      healthHaver.bulletScript = this.DeathBulletScript;
      if (string.IsNullOrEmpty(healthHaver.overrideDeathAnimBulletScript))
        return;
      string animBulletScript = healthHaver.overrideDeathAnimBulletScript;
      bool flag = false;
      if ((bool) (UnityEngine.Object) healthHaver.aiAnimator && healthHaver.aiAnimator.HasDirectionalAnimation(animBulletScript))
        flag = true;
      if ((bool) (UnityEngine.Object) healthHaver.spriteAnimator && healthHaver.spriteAnimator.GetClipByName(animBulletScript) != null)
        flag = true;
      if (flag)
        return;
      healthHaver.overrideDeathAnimBulletScript = string.Empty;
    }
    else
      healthHaver.chanceToSpawnBulletScript = 1f;
  }
}
