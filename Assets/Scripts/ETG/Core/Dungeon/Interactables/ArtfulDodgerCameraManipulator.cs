// Decompiled with JetBrains decompiler
// Type: ArtfulDodgerCameraManipulator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    public class ArtfulDodgerCameraManipulator : 
      DungeonPlaceableBehaviour,
      IEventTriggerable,
      IPlaceConfigurable
    {
      [DwarfConfigurable]
      public float OverrideZoomScale = 0.75f;
      [NonSerialized]
      public bool Active;
      private bool m_triggered;
      private bool m_triggeredFrame;
      private ArtfulDodgerRoomController m_dodgerRoom;
      protected RoomHandler m_room;

      [DebuggerHidden]
      private IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ArtfulDodgerCameraManipulator.\u003CStart\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      public void Trigger(int index)
      {
        if (this.m_dodgerRoom.Completed || !this.Active)
          return;
        this.m_triggeredFrame = true;
      }

      public void LateUpdate()
      {
        if (!this.m_triggeredFrame)
        {
          if (this.m_triggered)
          {
            this.m_triggered = false;
            Minimap.Instance.TemporarilyPreventMinimap = false;
            GameManager.Instance.MainCameraController.SetManualControl(false);
            GameManager.Instance.MainCameraController.OverrideZoomScale = 1f;
          }
        }
        else if (!this.m_triggered)
        {
          this.m_triggered = true;
          Minimap.Instance.TemporarilyPreventMinimap = true;
          GameManager.Instance.MainCameraController.OverridePosition = (Vector3) this.transform.position.XY();
          GameManager.Instance.MainCameraController.SetManualControl(true);
          GameManager.Instance.MainCameraController.OverrideZoomScale = this.OverrideZoomScale;
        }
        this.m_triggeredFrame = false;
      }

      public void ConfigureOnPlacement(RoomHandler room) => this.m_room = room;

      protected override void OnDestroy() => base.OnDestroy();
    }

}
