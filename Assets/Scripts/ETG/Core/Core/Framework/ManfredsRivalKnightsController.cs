// Decompiled with JetBrains decompiler
// Type: ManfredsRivalKnightsController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class ManfredsRivalKnightsController : BraveBehaviour
  {
    public float[] HealthThresholds = new float[2]
    {
      0.8f,
      0.6f
    };
    private List<AIActor> m_knights = new List<AIActor>();
    private int m_activeKnights;

    public void Start()
    {
      List<AIActor> activeEnemies = this.aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
      for (int index = 0; index < activeEnemies.Count; ++index)
      {
        if (!((Object) activeEnemies[index] == (Object) this.aiActor))
        {
          activeEnemies[index].behaviorSpeculator.enabled = false;
          this.m_knights.Add(activeEnemies[index]);
        }
      }
    }

    public void Update()
    {
      for (int index = 0; index < this.m_knights.Count; ++index)
      {
        if (!((Object) this.m_knights[index] == (Object) null))
        {
          if (!(bool) (Object) this.m_knights[index] || !(bool) (Object) this.m_knights[index].healthHaver || this.m_knights[index].healthHaver.IsDead)
          {
            ++this.m_activeKnights;
            this.m_knights[index] = (AIActor) null;
          }
          else if ((double) this.m_knights[index].healthHaver.GetCurrentHealthPercentage() < 1.0)
          {
            this.ActivateKnight(index);
          }
          else
          {
            this.m_knights[index].aiAnimator.LockFacingDirection = true;
            this.m_knights[index].aiAnimator.FacingDirection = -90f;
          }
        }
      }
      for (int index = 0; index < this.HealthThresholds.Length; ++index)
      {
        if ((double) this.healthHaver.GetCurrentHealthPercentage() < (double) this.HealthThresholds[index] && this.m_activeKnights <= index)
          this.ActivateKnight();
      }
    }

    protected override void OnDestroy() => base.OnDestroy();

    private void ActivateKnight(int index = -1)
    {
      if (index == -1)
      {
        index = 0;
        while (index < this.m_knights.Count && (Object) this.m_knights[index] == (Object) null)
          ++index;
      }
      if (index < 0 || index >= this.m_knights.Count)
        return;
      this.m_knights[index].behaviorSpeculator.enabled = true;
      this.m_knights[index].aiAnimator.LockFacingDirection = false;
      this.m_knights[index].aiActor.State = AIActor.ActorState.Normal;
      ++this.m_activeKnights;
      this.m_knights[index] = (AIActor) null;
    }
  }

