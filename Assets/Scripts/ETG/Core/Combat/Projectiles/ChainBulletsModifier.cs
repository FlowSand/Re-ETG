// Decompiled with JetBrains decompiler
// Type: ChainBulletsModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
    public class ChainBulletsModifier : MonoBehaviour
    {
      public int GuaranteedNumberOfChains = 1;
      public float ChanceToContinueChaining;
      public bool BounceRandomlyOnNoTarget = true;
      public float EnemyDetectRadius = -1f;
      private int m_numBounces;

      private void Start()
      {
        Projectile component = this.GetComponent<Projectile>();
        if (!(bool) (UnityEngine.Object) component)
          return;
        component.OnHitEnemy += new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHitEnemy);
      }

      private void HandleHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
      {
        this.DoChain(arg1, arg2);
      }

      private void DoChain(Projectile proj, SpeculativeRigidbody enemy)
      {
        if (this.m_numBounces >= this.GuaranteedNumberOfChains && (double) UnityEngine.Random.value >= (double) this.ChanceToContinueChaining)
          return;
        ++this.m_numBounces;
        if (this.BounceRandomlyOnNoTarget)
        {
          PierceProjModifier orAddComponent = proj.gameObject.GetOrAddComponent<PierceProjModifier>();
          orAddComponent.penetratesBreakables = true;
          ++orAddComponent.penetration;
        }
        Vector2 dirVec = UnityEngine.Random.insideUnitCircle;
        if ((bool) (UnityEngine.Object) enemy.aiActor)
        {
          AIActor closestToPosition = BraveUtility.GetClosestToPosition<AIActor>(enemy.aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All), enemy.UnitCenter, (Func<AIActor, bool>) null, this.EnemyDetectRadius, enemy.aiActor);
          if ((bool) (UnityEngine.Object) closestToPosition)
          {
            if (!this.BounceRandomlyOnNoTarget)
            {
              PierceProjModifier orAddComponent = proj.gameObject.GetOrAddComponent<PierceProjModifier>();
              orAddComponent.penetratesBreakables = true;
              ++orAddComponent.penetration;
            }
            dirVec = closestToPosition.CenterPosition - proj.transform.position.XY();
            if (!this.BounceRandomlyOnNoTarget)
              proj.SendInDirection(dirVec, false);
          }
        }
        if (!this.BounceRandomlyOnNoTarget)
          return;
        proj.SendInDirection(dirVec, false);
      }
    }

}
