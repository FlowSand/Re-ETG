// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.MasterServerGetNextHostData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Get the next host data from the master server. \nEach time this action is called it gets the next connected host.This lets you quickly loop through all the connected hosts to get information on each one.")]
[ActionCategory(ActionCategory.Network)]
public class MasterServerGetNextHostData : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("Event to send for looping.")]
  [ActionSection("Set up")]
  public FsmEvent loopEvent;
  [HutongGames.PlayMaker.Tooltip("Event to send when there are no more hosts.")]
  public FsmEvent finishedEvent;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("The index into the MasterServer Host List")]
  [ActionSection("Result")]
  public FsmInt index;
  [HutongGames.PlayMaker.Tooltip("Does this server require NAT punchthrough?")]
  [UIHint(UIHint.Variable)]
  public FsmBool useNat;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("The type of the game (e.g., 'MyUniqueGameType')")]
  public FsmString gameType;
  [HutongGames.PlayMaker.Tooltip("The name of the game (e.g., 'John Does's Game')")]
  [UIHint(UIHint.Variable)]
  public FsmString gameName;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("Currently connected players")]
  public FsmInt connectedPlayers;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("Maximum players limit")]
  public FsmInt playerLimit;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("Server IP address.")]
  public FsmString ipAddress;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("Server port")]
  public FsmInt port;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("Does the server require a password?")]
  public FsmBool passwordProtected;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("A miscellaneous comment (can hold data)")]
  public FsmString comment;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("The GUID of the host, needed when connecting with NAT punchthrough.")]
  public FsmString guid;
  private int nextItemIndex;
  private bool noMoreItems;

  public override void Reset()
  {
    this.finishedEvent = (FsmEvent) null;
    this.loopEvent = (FsmEvent) null;
    this.index = (FsmInt) null;
    this.useNat = (FsmBool) null;
    this.gameType = (FsmString) null;
    this.gameName = (FsmString) null;
    this.connectedPlayers = (FsmInt) null;
    this.playerLimit = (FsmInt) null;
    this.ipAddress = (FsmString) null;
    this.port = (FsmInt) null;
    this.passwordProtected = (FsmBool) null;
    this.comment = (FsmString) null;
    this.guid = (FsmString) null;
  }

  public override void OnEnter()
  {
    this.DoGetNextHostData();
    this.Finish();
  }

  private void DoGetNextHostData()
  {
    if (this.nextItemIndex >= MasterServer.PollHostList().Length)
    {
      this.nextItemIndex = 0;
      this.Fsm.Event(this.finishedEvent);
    }
    else
    {
      HostData pollHost = MasterServer.PollHostList()[this.nextItemIndex];
      this.index.Value = this.nextItemIndex;
      this.useNat.Value = pollHost.useNat;
      this.gameType.Value = pollHost.gameType;
      this.gameName.Value = pollHost.gameName;
      this.connectedPlayers.Value = pollHost.connectedPlayers;
      this.playerLimit.Value = pollHost.playerLimit;
      this.ipAddress.Value = pollHost.ip[0];
      this.port.Value = pollHost.port;
      this.passwordProtected.Value = pollHost.passwordProtected;
      this.comment.Value = pollHost.comment;
      this.guid.Value = pollHost.guid;
      if (this.nextItemIndex >= MasterServer.PollHostList().Length)
      {
        this.Fsm.Event(this.finishedEvent);
        this.nextItemIndex = 0;
      }
      else
      {
        ++this.nextItemIndex;
        if (this.loopEvent == null)
          return;
        this.Fsm.Event(this.loopEvent);
      }
    }
  }
}
