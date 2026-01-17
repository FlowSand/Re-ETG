// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.CheckRoomVisited
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  public class CheckRoomVisited : FsmStateAction
  {
    [Tooltip("Event sent if there are.")]
    public FsmEvent HasVisited;
    [Tooltip("Event sent if there aren't.")]
    public FsmEvent HasNotVisited;
    private RoomHandler m_targetRoom;

    public RoomHandler targetRoom
    {
      get => this.m_targetRoom;
      set => this.m_targetRoom = value;
    }

    public override void Awake() => base.Awake();

    public override void OnEnter()
    {
      if (this.targetRoom != null)
      {
        if (this.targetRoom.visibility == RoomHandler.VisibilityStatus.OBSCURED)
          this.Fsm.Event(this.HasNotVisited);
        else
          this.Fsm.Event(this.HasVisited);
      }
      this.Finish();
    }
  }
}
