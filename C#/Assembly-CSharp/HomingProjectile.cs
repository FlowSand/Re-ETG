// Decompiled with JetBrains decompiler
// Type: HomingProjectile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable
public class HomingProjectile : Projectile
{
  public float detectionRange = 5f;
  public float trackingSpeed = 5f;
  public bool stopTrackingIfLeaveRadius;
  private AIActor nearestEnemy;

  protected override void Move()
  {
    if (this.stopTrackingIfLeaveRadius)
      this.nearestEnemy = (AIActor) null;
    if ((Object) this.nearestEnemy == (Object) null)
      this.nearestEnemy = BraveUtility.GetClosestToPosition<AIActor>(GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.transform.position.IntXY()).GetActiveEnemies(RoomHandler.ActiveEnemyType.All), this.transform.position.XY());
    if ((Object) this.nearestEnemy != (Object) null)
    {
      Vector3 vector3 = this.nearestEnemy.transform.position - this.transform.position;
      float f = (float) (((double) Mathf.Atan2(vector3.y, vector3.x) - (double) Mathf.Atan2(this.specRigidbody.Velocity.y, this.specRigidbody.Velocity.x)) * 57.295780181884766);
      this.transform.Rotate(0.0f, 0.0f, Mathf.Min(Mathf.Abs(f), this.trackingSpeed * BraveTime.DeltaTime) * Mathf.Sign(f));
    }
    this.specRigidbody.Velocity = (Vector2) (this.transform.right * this.baseData.speed);
    this.LastVelocity = this.specRigidbody.Velocity;
  }

  protected override void OnDestroy() => base.OnDestroy();
}
