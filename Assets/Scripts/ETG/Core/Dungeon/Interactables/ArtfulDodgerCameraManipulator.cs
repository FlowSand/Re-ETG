using Dungeonator;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

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
      return (IEnumerator) new ArtfulDodgerCameraManipulator__Startc__Iterator0()
      {
        _this = this
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

