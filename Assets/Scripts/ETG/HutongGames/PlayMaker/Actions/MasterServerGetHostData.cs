using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Get host data from the master server.")]
  [ActionCategory(ActionCategory.Network)]
  public class MasterServerGetHostData : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The index into the MasterServer Host List")]
    [RequiredField]
    public FsmInt hostIndex;
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Does this server require NAT punchthrough?")]
    public FsmBool useNat;
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("The type of the game (e.g., 'MyUniqueGameType')")]
    public FsmString gameType;
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("The name of the game (e.g., 'John Does's Game')")]
    public FsmString gameName;
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Currently connected players")]
    public FsmInt connectedPlayers;
    [HutongGames.PlayMaker.Tooltip("Maximum players limit")]
    [UIHint(UIHint.Variable)]
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

    public override void Reset()
    {
      this.hostIndex = (FsmInt) null;
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
      this.GetHostData();
      this.Finish();
    }

    private void GetHostData()
    {
      int length = MasterServer.PollHostList().Length;
      int index = this.hostIndex.Value;
      if (index < 0 || index >= length)
      {
        this.LogError("MasterServer Host index out of range!");
      }
      else
      {
        HostData pollHost = MasterServer.PollHostList()[index];
        if (pollHost == null)
        {
          this.LogError("MasterServer HostData could not found at index " + (object) index);
        }
        else
        {
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
        }
      }
    }
  }
}
