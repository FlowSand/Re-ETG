// Decompiled with JetBrains decompiler
// Type: ScareEnemiesModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class ScareEnemiesModifier : MonoBehaviour
{
  public FleePlayerData FleeData;
  public float ConeAngle = 45f;
  public bool OnlyFearDuringReload;
  private Gun m_gun;
  private List<AIActor> m_allEnemies = new List<AIActor>();

  private void Awake() => this.m_gun = this.GetComponent<Gun>();

  private void Update()
  {
    if (!(bool) (Object) this.m_gun || !(bool) (Object) this.m_gun.CurrentOwner || !(bool) (Object) this.m_gun.CurrentOwner.healthHaver || !(this.m_gun.CurrentOwner is PlayerController))
      return;
    PlayerController currentOwner = this.m_gun.CurrentOwner as PlayerController;
    if ((Object) currentOwner.CurrentGun != (Object) this.m_gun || currentOwner.CurrentRoom == null)
      return;
    this.FleeData.Player = currentOwner;
    currentOwner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All, ref this.m_allEnemies);
    if (this.m_allEnemies == null || this.m_allEnemies.Count == 0)
      return;
    float currentAngle = this.m_gun.CurrentAngle;
    Vector2 centerPosition = this.m_gun.CurrentOwner.CenterPosition;
    for (int index = 0; index < this.m_allEnemies.Count; ++index)
    {
      AIActor allEnemy = this.m_allEnemies[index];
      if ((bool) (Object) allEnemy && (bool) (Object) allEnemy.healthHaver && allEnemy.IsNormalEnemy && allEnemy.IsWorthShootingAt && !allEnemy.healthHaver.IsBoss && !allEnemy.healthHaver.IsDead && (bool) (Object) allEnemy.behaviorSpeculator)
      {
        if ((double) BraveMathCollege.AbsAngleBetween(currentAngle, BraveMathCollege.Atan2Degrees(allEnemy.CenterPosition - centerPosition)) < (double) this.ConeAngle)
          allEnemy.behaviorSpeculator.FleePlayerData = !this.OnlyFearDuringReload || this.m_gun.IsReloading ? this.FleeData : (FleePlayerData) null;
        else
          allEnemy.behaviorSpeculator.FleePlayerData = (FleePlayerData) null;
      }
    }
    this.m_allEnemies.Clear();
  }
}
