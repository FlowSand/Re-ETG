// Decompiled with JetBrains decompiler
// Type: WarpWingPortalController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class WarpWingPortalController : BraveBehaviour, IPlayerInteractable
  {
    public bool UsesTriggerZone;
    [NonSerialized]
    public WarpWingPortalController pairedPortal;
    [NonSerialized]
    public RoomHandler parentRoom;
    [NonSerialized]
    public RuntimeRoomExitData parentExit;
    [NonSerialized]
    private float FailChance;
    [NonSerialized]
    public WarpWingPortalController failPortal;
    private bool m_justUsed;

    [DebuggerHidden]
    private IEnumerator Start()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new WarpWingPortalController__Startc__Iterator0()
      {
        _this = this
      };
    }

    private void Update() => this.m_justUsed = false;

    private void HandleTriggerEntered(
      SpeculativeRigidbody specRigidbody,
      SpeculativeRigidbody sourceSpecRigidbody,
      CollisionData collisionData)
    {
      if (this.m_justUsed)
        return;
      PlayerController component = specRigidbody.GetComponent<PlayerController>();
      if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
        return;
      this.DoTeleport(component);
    }

    private void HandleResourcefulRatFlowSetup()
    {
      DungeonData.Direction[] resourcefulRatSolution = GameManager.GetResourcefulRatSolution();
      if (this.parentRoom.area.PrototypeRoomName == "ResourcefulRat_ChainRoom_01")
      {
        if (resourcefulRatSolution[0] == this.parentExit.referencedExit.exitDirection)
        {
          this.pairedPortal = this.GetDirectionalPortalFromRoom(2, DungeonData.Direction.WEST);
        }
        else
        {
          this.AttachResourcefulRatFailRoom();
          this.pairedPortal = this.GetRatFirstRoomEntrancePortal();
        }
      }
      else if (this.parentRoom.area.PrototypeRoomName == "ResourcefulRat_ChainRoom_02")
      {
        if (resourcefulRatSolution[1] == this.parentExit.referencedExit.exitDirection)
        {
          this.pairedPortal = this.GetDirectionalPortalFromRoom(3, DungeonData.Direction.WEST);
        }
        else
        {
          this.AttachResourcefulRatFailRoom();
          this.pairedPortal = this.GetRatFirstRoomEntrancePortal();
        }
      }
      else if (this.parentRoom.area.PrototypeRoomName == "ResourcefulRat_ChainRoom_03")
      {
        if (resourcefulRatSolution[2] == this.parentExit.referencedExit.exitDirection)
        {
          this.pairedPortal = this.GetDirectionalPortalFromRoom(4, DungeonData.Direction.WEST);
        }
        else
        {
          this.AttachResourcefulRatFailRoom();
          this.pairedPortal = this.GetRatFirstRoomEntrancePortal();
        }
      }
      else if (this.parentRoom.area.PrototypeRoomName == "ResourcefulRat_ChainRoom_04")
      {
        if (resourcefulRatSolution[3] == this.parentExit.referencedExit.exitDirection)
        {
          this.pairedPortal = this.GetDirectionalPortalFromRoom(5, DungeonData.Direction.WEST);
        }
        else
        {
          this.AttachResourcefulRatFailRoom();
          this.pairedPortal = this.GetRatFirstRoomEntrancePortal();
        }
      }
      else if (this.parentRoom.area.PrototypeRoomName == "ResourcefulRat_ChainRoom_05")
      {
        if (resourcefulRatSolution[4] == this.parentExit.referencedExit.exitDirection)
        {
          this.pairedPortal = this.GetDirectionalPortalFromRoom(6, DungeonData.Direction.WEST);
        }
        else
        {
          this.AttachResourcefulRatFailRoom();
          this.pairedPortal = this.GetRatFirstRoomEntrancePortal();
        }
      }
      else if (this.parentRoom.area.PrototypeRoomName == "ResourcefulRat_ChainRoom_06")
      {
        if (resourcefulRatSolution[5] == this.parentExit.referencedExit.exitDirection)
        {
          this.pairedPortal = this.GetDirectionalPortalFromRoom(7, DungeonData.Direction.WEST);
        }
        else
        {
          this.AttachResourcefulRatFailRoom();
          this.pairedPortal = this.GetRatFirstRoomEntrancePortal();
        }
      }
      else
      {
        if (!(this.parentRoom.area.PrototypeRoomName == "ResourcefulRat_FailRoom"))
          return;
        this.AttachResourcefulRatFailRoom();
        WarpWingPortalController roomEntrancePortal = this.GetRatFirstRoomEntrancePortal();
        if (!((UnityEngine.Object) roomEntrancePortal != (UnityEngine.Object) null))
          return;
        this.pairedPortal = roomEntrancePortal;
      }
    }

    private WarpWingPortalController GetDirectionalPortalFromRoom(
      int roomIndex,
      DungeonData.Direction dir)
    {
      WarpWingPortalController directionalPortalFromRoom = (WarpWingPortalController) null;
      for (int index1 = 0; index1 < GameManager.Instance.Dungeon.data.rooms.Count; ++index1)
      {
        string str = "ResourcefulRat_ChainRoom_0" + roomIndex.ToString();
        if (roomIndex > 6)
          str = "Boss Foyer";
        if (GameManager.Instance.Dungeon.data.rooms[index1].area.PrototypeRoomName == str)
        {
          RoomHandler room = GameManager.Instance.Dungeon.data.rooms[index1];
          for (int index2 = 0; index2 < room.area.instanceUsedExits.Count; ++index2)
          {
            if (room.area.instanceUsedExits[index2].exitDirection == dir)
              directionalPortalFromRoom = room.area.exitToLocalDataMap[room.area.instanceUsedExits[index2]].warpWingPortal;
          }
        }
      }
      return directionalPortalFromRoom;
    }

    private WarpWingPortalController GetRatFirstRoomEntrancePortal()
    {
      return this.GetDirectionalPortalFromRoom(2, DungeonData.Direction.WEST);
    }

    private void AttachResourcefulRatFailRoom()
    {
      for (int index1 = 0; index1 < GameManager.Instance.Dungeon.data.rooms.Count; ++index1)
      {
        if (GameManager.Instance.Dungeon.data.rooms[index1].area.PrototypeRoomName == "ResourcefulRat_FailExit")
        {
          RoomHandler room = GameManager.Instance.Dungeon.data.rooms[index1];
          for (int index2 = 0; index2 < room.area.instanceUsedExits.Count; ++index2)
          {
            WarpWingPortalController warpWingPortal = room.area.exitToLocalDataMap[room.area.instanceUsedExits[index2]].warpWingPortal;
            if ((UnityEngine.Object) warpWingPortal != (UnityEngine.Object) null)
            {
              warpWingPortal.pairedPortal = warpWingPortal;
              this.failPortal = warpWingPortal;
              this.FailChance = 0.25f;
              break;
            }
          }
        }
      }
    }

    public void OnEnteredRange(PlayerController interactor)
    {
      if (!(bool) (UnityEngine.Object) this)
        return;
      SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.white, 0.1f);
      this.sprite.UpdateZDepth();
    }

    public void OnExitRange(PlayerController interactor)
    {
      if (!(bool) (UnityEngine.Object) this)
        return;
      SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
      this.sprite.UpdateZDepth();
    }

    public float GetDistanceToPoint(Vector2 point)
    {
      if (this.UsesTriggerZone || (UnityEngine.Object) this.pairedPortal == (UnityEngine.Object) this || (UnityEngine.Object) this.pairedPortal == (UnityEngine.Object) null)
        return 1000f;
      Bounds bounds = this.sprite.GetBounds();
      bounds.SetMinMax(bounds.min + this.transform.position, bounds.max + this.transform.position);
      float num1 = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
      float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
      return Mathf.Sqrt((float) (((double) point.x - (double) num1) * ((double) point.x - (double) num1) + ((double) point.y - (double) num2) * ((double) point.y - (double) num2)));
    }

    public float GetOverrideMaxDistance() => -1f;

    [DebuggerHidden]
    private IEnumerator HandleDelayedAnimationTrigger(tk2dSpriteAnimator target)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new WarpWingPortalController__HandleDelayedAnimationTriggerc__Iterator1()
      {
        target = target
      };
    }

    public void Interact(PlayerController player) => this.DoTeleport(player);

    [DebuggerHidden]
    private IEnumerator MarkUsed(WarpWingPortalController targetPortal)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new WarpWingPortalController__MarkUsedc__Iterator2()
      {
        targetPortal = targetPortal
      };
    }

    private void DoTeleport(PlayerController player)
    {
      if ((UnityEngine.Object) this.pairedPortal == (UnityEngine.Object) this || (UnityEngine.Object) this.pairedPortal == (UnityEngine.Object) null)
        return;
      if ((UnityEngine.Object) this.failPortal != (UnityEngine.Object) null && (double) UnityEngine.Random.value < (double) this.FailChance)
      {
        this.spriteAnimator.Play("resourceful_rat_teleport_out");
        this.StartCoroutine(this.HandleDelayedAnimationTrigger(this.failPortal.spriteAnimator));
        this.StartCoroutine(this.MarkUsed(this));
        this.StartCoroutine(this.MarkUsed(this.pairedPortal));
        player.TeleportToPoint(this.failPortal.sprite.WorldCenter, true);
      }
      else
      {
        this.spriteAnimator.Play("resourceful_rat_teleport_out");
        this.StartCoroutine(this.HandleDelayedAnimationTrigger(this.pairedPortal.spriteAnimator));
        this.StartCoroutine(this.MarkUsed(this));
        this.StartCoroutine(this.MarkUsed(this.pairedPortal));
        player.TeleportToPoint(this.pairedPortal.sprite.WorldCenter, true);
      }
    }

    public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
    {
      shouldBeFlipped = false;
      return string.Empty;
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

