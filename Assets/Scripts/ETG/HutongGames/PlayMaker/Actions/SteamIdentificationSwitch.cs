// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SteamIdentificationSwitch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Steamworks;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Logic)]
  public class SteamIdentificationSwitch : FsmStateAction
  {
    [CompoundArray("Int Switches", "Compare Int", "Send Event")]
    public FsmString[] targetIDs;
    public FsmEvent[] sendEvent;
    public bool everyFrame;
    public FsmEvent defaultEvent;

    public override void Reset()
    {
      this.targetIDs = new FsmString[1];
      this.sendEvent = new FsmEvent[1];
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoIDSwitch();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoIDSwitch();

    private void DoIDSwitch()
    {
      bool flag = false;
      ulong num = 0;
      if (GameManager.Instance.platformInterface is PlatformInterfaceSteam && SteamManager.Initialized)
      {
        num = SteamUser.GetSteamID().m_SteamID;
        flag = true;
      }
      if (flag)
      {
        for (int index = 0; index < this.targetIDs.Length; ++index)
        {
          if (this.targetIDs[index].Value == num.ToString())
          {
            this.Fsm.Event(this.sendEvent[index]);
            return;
          }
        }
      }
      this.Fsm.Event(this.defaultEvent);
    }
  }
}
