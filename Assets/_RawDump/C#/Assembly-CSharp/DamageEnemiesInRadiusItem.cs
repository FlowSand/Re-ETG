// Decompiled with JetBrains decompiler
// Type: DamageEnemiesInRadiusItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class DamageEnemiesInRadiusItem : AffectEnemiesInRadiusItem
{
  public float Damage = 10f;
  public bool PreventsReinforcements;

  protected override void DoEffect(PlayerController user)
  {
    if (this.PreventsReinforcements && user.CurrentRoom.area.PrototypeRoomCategory != PrototypeDungeonRoom.RoomCategory.BOSS)
      user.CurrentRoom.ClearReinforcementLayers();
    base.DoEffect(user);
  }

  protected override void AffectEnemy(AIActor target)
  {
    if (!(bool) (Object) target.healthHaver)
      return;
    target.healthHaver.ApplyDamage(this.Damage, Vector2.zero, string.Empty);
  }
}
