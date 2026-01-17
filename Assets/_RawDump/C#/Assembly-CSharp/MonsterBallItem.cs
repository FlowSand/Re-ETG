// Decompiled with JetBrains decompiler
// Type: MonsterBallItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
public class MonsterBallItem : PlayerItem
{
  public GameActorCharmEffect CharmEffect;
  private bool m_containsEnemy;
  private string m_storedEnemyGuid;
  private int m_idleSpriteId = -1;

  private void Awake() => this.m_idleSpriteId = this.sprite.spriteId;

  public override void Pickup(PlayerController player)
  {
    base.Pickup(player);
    this.sprite.SetSprite(this.m_idleSpriteId);
    if (!this.m_containsEnemy)
      return;
    this.IsCurrentlyActive = true;
    this.ClearCooldowns();
  }

  protected override void DoEffect(PlayerController user)
  {
    if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES)
      return;
    DebrisObject debrisObject = user.DropActiveItem((PlayerItem) this, 10f);
    if (!(bool) (UnityEngine.Object) debrisObject)
      return;
    MonsterBallItem component = debrisObject.GetComponent<MonsterBallItem>();
    component.spriteAnimator.Play("monster_ball_throw");
    component.m_containsEnemy = this.m_containsEnemy;
    component.m_storedEnemyGuid = this.m_storedEnemyGuid;
    debrisObject.OnGrounded += new Action<DebrisObject>(this.HandleTossedBallGrounded);
  }

  private void HandleTossedBallGrounded(DebrisObject obj)
  {
    obj.OnGrounded -= new Action<DebrisObject>(this.HandleTossedBallGrounded);
    MonsterBallItem component = obj.GetComponent<MonsterBallItem>();
    component.spriteAnimator.Play("monster_ball_open");
    float nearestDistance = -1f;
    AIActor nearestEnemy = obj.transform.position.GetAbsoluteRoom().GetNearestEnemy(obj.sprite.WorldCenter, out nearestDistance, false);
    if (!(bool) (UnityEngine.Object) nearestEnemy || (double) nearestDistance >= 10.0)
      return;
    component.m_containsEnemy = true;
    component.m_storedEnemyGuid = nearestEnemy.EnemyGuid;
    LootEngine.DoDefaultItemPoof(nearestEnemy.CenterPosition);
    nearestEnemy.EraseFromExistence();
  }

  protected override void DoActiveEffect(PlayerController user)
  {
    if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES)
      return;
    DebrisObject debrisObject = user.DropActiveItem((PlayerItem) this, 10f);
    if (!(bool) (UnityEngine.Object) debrisObject)
      return;
    MonsterBallItem component = debrisObject.GetComponent<MonsterBallItem>();
    component.spriteAnimator.Play("monster_ball_throw");
    component.m_containsEnemy = this.m_containsEnemy;
    component.m_storedEnemyGuid = this.m_storedEnemyGuid;
    debrisObject.OnGrounded += new Action<DebrisObject>(this.HandleActiveTossedBallGrounded);
  }

  private void HandleActiveTossedBallGrounded(DebrisObject obj)
  {
    obj.OnGrounded -= new Action<DebrisObject>(this.HandleActiveTossedBallGrounded);
    MonsterBallItem component = obj.GetComponent<MonsterBallItem>();
    component.spriteAnimator.Play("monster_ball_open");
    AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(this.m_storedEnemyGuid);
    IntVector2 bestRewardLocation = obj.transform.position.GetAbsoluteRoom().GetBestRewardLocation(orLoadByGuid.Clearance, obj.sprite.WorldCenter);
    AIActor.Spawn(orLoadByGuid, bestRewardLocation, obj.transform.position.GetAbsoluteRoom(), true).ApplyEffect((GameActorEffect) this.CharmEffect);
    component.m_containsEnemy = false;
    component.m_storedEnemyGuid = string.Empty;
    component.IsCurrentlyActive = false;
    component.ApplyCooldown(this.LastOwner);
  }

  protected override void OnDestroy() => base.OnDestroy();
}
