// Decompiled with JetBrains decompiler
// Type: BossStatuesChallengeModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Data
{
    public class BossStatuesChallengeModifier : ChallengeModifier
    {
      public GoopDefinition Goop;
      public int chessSquaresX = 4;
      public int chessSquaresY = 4;
      public float InitialDelayTime = 4f;
      public float AdditionalDelayTime = 1.5f;
      private RoomHandler m_room;
      private DeadlyDeadlyGoopManager m_manager;
      private bool m_firstQuadrant;
      private float m_timer;

      private void Start()
      {
        this.m_room = GameManager.Instance.PrimaryPlayer.CurrentRoom;
        this.m_timer = this.InitialDelayTime;
        this.m_manager = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.Goop);
      }

      private void Update()
      {
        this.m_timer -= BraveTime.DeltaTime;
        if ((double) this.m_timer > 0.0)
          return;
        this.m_timer = this.Goop.lifespan + this.AdditionalDelayTime;
        Vector2 vector2 = new Vector2((float) this.m_room.area.dimensions.x / (1f * (float) this.chessSquaresX), (float) this.m_room.area.dimensions.y / (1f * (float) this.chessSquaresY));
        for (int index1 = 0; index1 < this.chessSquaresX; ++index1)
        {
          for (int index2 = 0; index2 < this.chessSquaresY; ++index2)
          {
            Vector2 min = this.m_room.area.basePosition.ToVector2() + new Vector2(vector2.x * (float) index1, vector2.y * (float) index2);
            Vector2 max = min + vector2;
            int num = (index1 + index2) % 2;
            if (num == 1 && this.m_firstQuadrant || num == 0 && !this.m_firstQuadrant)
              this.m_manager.TimedAddGoopRect(min, max, 0.5f);
          }
        }
        this.m_firstQuadrant = !this.m_firstQuadrant;
      }
    }

}
