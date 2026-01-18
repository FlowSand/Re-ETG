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

