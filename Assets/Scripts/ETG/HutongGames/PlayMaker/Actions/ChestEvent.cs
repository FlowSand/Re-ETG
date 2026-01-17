// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ChestEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Responds to chest events.")]
  [ActionCategory(".NPCs")]
  public class ChestEvent : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("Event to play when the chest has been unlocked.")]
    public FsmEvent unlocked;
    [HutongGames.PlayMaker.Tooltip("Event to play when the chest has been locked.")]
    public FsmEvent locked;
    [HutongGames.PlayMaker.Tooltip("Event to play when the chest has been unsealed.")]
    public FsmEvent unsealed;
    [HutongGames.PlayMaker.Tooltip("Event to play when the chest has been sealed.")]
    public FsmEvent Sealed;
    [HutongGames.PlayMaker.Tooltip("Event to play when the chest has been opened.")]
    public FsmEvent opened;
    [HutongGames.PlayMaker.Tooltip("Event to play when the chest has been destroyed.")]
    public FsmEvent destroyed;
    private Chest m_chest;
    private bool m_wasLocked;
    private bool m_wasSealed;
    private bool m_wasOpen;
    private bool m_wasDestroyed;

    public override void Reset()
    {
      this.unlocked = (FsmEvent) null;
      this.locked = (FsmEvent) null;
      this.unsealed = (FsmEvent) null;
      this.Sealed = (FsmEvent) null;
      this.opened = (FsmEvent) null;
      this.destroyed = (FsmEvent) null;
    }

    public override void OnEnter()
    {
      List<Chest> componentsInRoom = GameManager.Instance.Dungeon.GetRoomFromPosition(this.Owner.transform.position.IntXY(VectorConversions.Floor)).GetComponentsInRoom<Chest>();
      if (componentsInRoom != null && componentsInRoom.Count > 0)
      {
        this.m_chest = componentsInRoom[0];
        if (componentsInRoom.Count > 1)
          Debug.LogError((object) "Too many chests!");
        this.m_wasLocked = this.m_chest.IsLocked;
        this.m_wasSealed = this.m_chest.IsSealed;
        this.m_wasOpen = this.m_chest.IsOpen;
        this.m_wasDestroyed = this.m_chest.IsBroken;
      }
      else
      {
        Debug.LogError((object) "No chests found!");
        this.Finish();
      }
    }

    public override void OnUpdate()
    {
      if (!(bool) (Object) this.m_chest)
      {
        this.Finish();
      }
      else
      {
        if (this.unlocked != null & this.m_wasLocked && !this.m_chest.IsLocked)
          this.Fsm.Event(this.unlocked);
        if (this.locked != null & !this.m_wasLocked && this.m_chest.IsLocked)
          this.Fsm.Event(this.locked);
        if (this.unsealed != null & this.m_wasSealed && !this.m_chest.IsSealed)
          this.Fsm.Event(this.unsealed);
        if (this.Sealed != null & !this.m_wasSealed && this.m_chest.IsSealed)
          this.Fsm.Event(this.Sealed);
        if (this.opened != null & !this.m_wasOpen && this.m_chest.IsOpen)
          this.Fsm.Event(this.opened);
        if (this.destroyed != null & !this.m_wasDestroyed && this.m_chest.IsBroken)
          this.Fsm.Event(this.destroyed);
        this.m_wasLocked = this.m_chest.IsLocked;
        this.m_wasSealed = this.m_chest.IsSealed;
        this.m_wasOpen = this.m_chest.IsOpen;
        this.m_wasDestroyed = this.m_chest.IsBroken;
      }
    }
  }
}
