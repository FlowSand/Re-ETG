// Decompiled with JetBrains decompiler
// Type: BombCompanionAppItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Active
{
    public class BombCompanionAppItem : PlayerItem
    {
      protected override void DoEffect(PlayerController user)
      {
        int num = (int) AkSoundEngine.PostEvent("Play_OBJ_computer_boop_01", this.gameObject);
        RoomHandler currentRoom = user.CurrentRoom;
        if (currentRoom == null)
          return;
        for (int index = 0; index < StaticReferenceManager.AllMinorBreakables.Count; ++index)
        {
          MinorBreakable allMinorBreakable = StaticReferenceManager.AllMinorBreakables[index];
          if (allMinorBreakable.transform.position.GetAbsoluteRoom() == currentRoom && !allMinorBreakable.IsBroken && allMinorBreakable.explodesOnBreak)
            allMinorBreakable.Break();
        }
        List<AIActor> activeEnemies = currentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
        if (activeEnemies != null)
        {
          for (int index = activeEnemies.Count - 1; index >= 0; --index)
          {
            AIActor aiActor = activeEnemies[index];
            if ((bool) (Object) aiActor && !aiActor.IsSignatureEnemy)
            {
              HealthHaver healthHaver = aiActor.healthHaver;
              if ((bool) (Object) healthHaver && !healthHaver.IsDead && !healthHaver.IsBoss)
              {
                ExplodeOnDeath component = aiActor.GetComponent<ExplodeOnDeath>();
                if ((bool) (Object) component && !component.immuneToIBombApp)
                  healthHaver.ApplyDamage((float) int.MaxValue, Vector2.zero, "iBomb", ignoreInvulnerabilityFrames: true);
              }
            }
          }
        }
        List<Projectile> projectileList = new List<Projectile>();
        for (int index = 0; index < StaticReferenceManager.AllProjectiles.Count; ++index)
        {
          if ((bool) (Object) StaticReferenceManager.AllProjectiles[index] && (Object) StaticReferenceManager.AllProjectiles[index].GetComponent<ExplosiveModifier>() != (Object) null)
            projectileList.Add(StaticReferenceManager.AllProjectiles[index]);
        }
        for (int index = 0; index < projectileList.Count; ++index)
          projectileList[index].DieInAir();
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
