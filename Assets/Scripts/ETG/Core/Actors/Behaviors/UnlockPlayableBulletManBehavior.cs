// Decompiled with JetBrains decompiler
// Type: UnlockPlayableBulletManBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable

namespace ETG.Core.Actors.Behaviors
{
    public class UnlockPlayableBulletManBehavior : BehaviorBase
    {
      private float m_aloneElapsed;

      public override void Start()
      {
        base.Start();
        if (!(bool) (Object) this.m_aiActor || !(bool) (Object) this.m_aiActor.sprite)
          ;
      }

      public override void Upkeep() => base.Upkeep();

      public override BehaviorResult Update()
      {
        if (this.m_aiActor.ParentRoom.GetActiveEnemiesCount(RoomHandler.ActiveEnemyType.RoomClear) == 1)
        {
          this.m_aloneElapsed += BraveTime.DeltaTime;
          if ((double) this.m_aloneElapsed > 3.0)
          {
            Object.Instantiate<GameObject>((GameObject) ResourceCache.Acquire("Global VFX/VFX_Teleport_Beam")).GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor((Vector3) (this.m_aiActor.specRigidbody.UnitBottomCenter + new Vector2(0.0f, -0.5f)), tk2dBaseSprite.Anchor.LowerCenter);
            Debug.Log((object) "Setting a SEEN_SECRET_BULLETMAN flag!");
            GameStatsManager.Instance.SetNextFlag(GungeonFlags.SECRET_BULLETMAN_SEEN_01, GungeonFlags.SECRET_BULLETMAN_SEEN_02, GungeonFlags.SECRET_BULLETMAN_SEEN_03, GungeonFlags.SECRET_BULLETMAN_SEEN_04, GungeonFlags.SECRET_BULLETMAN_SEEN_05);
            Object.Destroy((Object) this.m_gameObject);
          }
        }
        return base.Update();
      }
    }

}
