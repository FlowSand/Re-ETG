// Decompiled with JetBrains decompiler
// Type: TowerBossController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    public class TowerBossController : DungeonPlaceableBehaviour
    {
      public HealthHaver cockpitHealthHaver;
      public tk2dSprite towerSprite;
      public TowerBossIrisController irisLeft;
      public TowerBossIrisController irisRight;
      public TowerBossEmitterController[] laserEmitters;
      public Vector2 ellipseCenter;
      public Vector2 ellipseAxes;
      public Projectile beamProjectile;
      public float spinSpeed = 60f;
      private TowerBossBatteryController m_batteryLeft;
      private TowerBossBatteryController m_batteryRight;
      private bool m_alive = true;
      public TowerBossController.TowerBossPhase currentPhase;

      private void Start()
      {
        TowerBossBatteryController[] objectsOfType = (TowerBossBatteryController[]) UnityEngine.Object.FindObjectsOfType(typeof (TowerBossBatteryController));
        BraveUtility.Assert(objectsOfType.Length != 2, "Trying to initialize TowerBoss with more or less than 2 batteries in world.");
        if ((double) objectsOfType[0].transform.position.x < (double) objectsOfType[1].transform.position.x)
        {
          this.m_batteryLeft = objectsOfType[0];
          this.m_batteryRight = objectsOfType[1];
        }
        else
        {
          this.m_batteryLeft = objectsOfType[1];
          this.m_batteryRight = objectsOfType[0];
        }
        this.m_batteryLeft.tower = this;
        this.m_batteryRight.tower = this;
        this.m_batteryLeft.linkedIris = this.irisLeft;
        this.m_batteryRight.linkedIris = this.irisRight;
        for (int index = 0; index < this.transform.childCount; ++index)
        {
          Transform child = this.transform.GetChild(index);
          MeshRenderer componentInChildren = child.GetComponentInChildren<MeshRenderer>();
          if ((UnityEngine.Object) child.GetComponent<Renderer>() != (UnityEngine.Object) null)
            DepthLookupManager.PinRendererToRenderer((Renderer) componentInChildren, (Renderer) this.towerSprite.GetComponent<MeshRenderer>());
        }
        this.towerSprite.IsPerpendicular = false;
        this.cockpitHealthHaver.IsVulnerable = false;
        this.cockpitHealthHaver.OnDeath += new Action<Vector2>(this.Die);
        this.StartCoroutine(this.HandleBatteryCycle());
        this.StartCoroutine(this.HandleBeamCycle());
      }

      private void Die(Vector2 lastDirection) => this.m_alive = false;

      private void Update()
      {
        if (!this.m_alive)
          return;
        for (int index = 0; index < this.laserEmitters.Length; ++index)
        {
          float f = this.laserEmitters[index].currentAngle + (float) ((double) this.spinSpeed * (double) BraveTime.DeltaTime * (Math.PI / 180.0));
          Vector3 vector3 = (Vector3) (this.transform.position.XY() + this.ellipseCenter);
          float x = vector3.x + this.ellipseAxes.x * Mathf.Cos(f);
          float y = vector3.y + this.ellipseAxes.y * Mathf.Sin(f);
          this.laserEmitters[index].transform.position = BraveUtility.QuantizeVector(new Vector3(x, y, this.laserEmitters[index].transform.position.z));
          this.laserEmitters[index].UpdateAngle(f % 6.28318548f);
        }
      }

      protected override void OnDestroy() => base.OnDestroy();

      [DebuggerHidden]
      private IEnumerator HandleBeamCycle()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TowerBossController.\u003CHandleBeamCycle\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleBatteryCycle()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TowerBossController.\u003CHandleBatteryCycle\u003Ec__Iterator1()
        {
          \u0024this = this
        };
      }

      private void PhaseTransition()
      {
        this.m_batteryLeft.linkedIris = this.irisRight;
        this.m_batteryRight.linkedIris = this.irisLeft;
        this.irisLeft.fuseAlive = true;
        this.irisRight.fuseAlive = true;
        this.currentPhase = TowerBossController.TowerBossPhase.PHASE_TWO;
      }

      public void NotifyFuseDestruction(TowerBossIrisController source)
      {
        if (this.irisLeft.fuseAlive || this.irisRight.fuseAlive)
          return;
        this.cockpitHealthHaver.IsVulnerable = true;
        GameManager.Instance.Dungeon.GetRoomFromPosition(this.transform.position.IntXY(VectorConversions.Floor)).HandleRoomAction(RoomEventTriggerAction.UNSEAL_ROOM);
      }

      public enum TowerBossPhase
      {
        PHASE_ONE,
        PHASE_TWO,
      }
    }

}
