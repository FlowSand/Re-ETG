// Decompiled with JetBrains decompiler
// Type: TrapSpawnerController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class TrapSpawnerController : BraveBehaviour
    {
      [Header("Spawn Info")]
      public GameObject Trap;
      public GameObject PoofVfx;
      public List<Vector2> RoomPositionOffsets;
      public List<float> SpawnDelays;
      public Vector2 VfxOffset;
      public float VfxLeadTime;
      public float AdditionalTriggerDelayTime;
      [Header("Spawn Triggers")]
      public bool SpawnOnIntroFinished;
      [Header("Destroy Triggers")]
      public bool DestroyOnDeath;
      private RoomHandler m_room;
      private List<GameObject> m_traps = new List<GameObject>();

      public void Start()
      {
        this.m_room = this.GetComponent<AIActor>().ParentRoom;
        if (this.SpawnOnIntroFinished)
          this.GetComponent<GenericIntroDoer>().OnIntroFinished += new System.Action(this.OnIntroFinished);
        if (!this.DestroyOnDeath)
          return;
        this.healthHaver.OnDeath += new Action<Vector2>(this.OnDeath);
      }

      protected override void OnDestroy() => base.OnDestroy();

      private void OnIntroFinished()
      {
        if (!this.SpawnOnIntroFinished)
          return;
        this.StartCoroutine(this.SpawnTraps());
      }

      private void OnDeath(Vector2 vector2)
      {
        if (!this.DestroyOnDeath)
          return;
        this.DestroyTraps();
      }

      [DebuggerHidden]
      private IEnumerator SpawnTraps()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TrapSpawnerController.\u003CSpawnTraps\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator SpawnTrap(Vector2 pos)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TrapSpawnerController.\u003CSpawnTrap\u003Ec__Iterator1()
        {
          pos = pos,
          \u0024this = this
        };
      }

      private void DestroyTraps()
      {
        if (!GameManager.HasInstance || GameManager.Instance.IsLoadingLevel || GameManager.IsReturningToBreach)
          return;
        for (int index = 0; index < this.m_traps.Count; ++index)
          GameManager.Instance.StartCoroutine(this.DestroyTrap(this.m_traps[index]));
      }

      [DebuggerHidden]
      private IEnumerator DestroyTrap(GameObject trap)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TrapSpawnerController.\u003CDestroyTrap\u003Ec__Iterator2()
        {
          trap = trap,
          \u0024this = this
        };
      }
    }

}
