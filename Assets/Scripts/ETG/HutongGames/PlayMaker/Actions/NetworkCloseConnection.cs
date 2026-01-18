using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Network)]
  [HutongGames.PlayMaker.Tooltip("Close the connection to another system.\n\nConnection index defines which system to close the connection to (from the Network connections array).\nCan define connection to close via Guid if index is unknown. \nIf we are a client the only possible connection to close is the server connection, if we are a server the target player will be kicked off. \n\nSend Disconnection Notification enables or disables notifications being sent to the other end. If disabled the connection is dropped, if not a disconnect notification is reliably sent to the remote party and there after the connection is dropped.")]
  public class NetworkCloseConnection : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("Connection index to close")]
    [UIHint(UIHint.Variable)]
    public FsmInt connectionIndex;
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Connection GUID to close. Used If Index is not set.")]
    public FsmString connectionGUID;
    [HutongGames.PlayMaker.Tooltip("If True, send Disconnection Notification")]
    public bool sendDisconnectionNotification;

    public override void Reset()
    {
      this.connectionIndex = (FsmInt) 0;
      this.connectionGUID = (FsmString) null;
      this.sendDisconnectionNotification = true;
    }

    public override void OnEnter()
    {
      int index = 0;
      if (!this.connectionIndex.IsNone)
      {
        index = this.connectionIndex.Value;
      }
      else
      {
        int guidIndex;
        if (!this.connectionGUID.IsNone && this.getIndexFromGUID(this.connectionGUID.Value, out guidIndex))
          index = guidIndex;
      }
      if (index < 0 || index > Network.connections.Length)
        this.LogError("Connection index out of range: " + (object) index);
      else
        Network.CloseConnection(Network.connections[index], this.sendDisconnectionNotification);
      this.Finish();
    }

    private bool getIndexFromGUID(string guid, out int guidIndex)
    {
      for (int index = 0; index < Network.connections.Length; ++index)
      {
        if (guid.Equals(Network.connections[index].guid))
        {
          guidIndex = index;
          return true;
        }
      }
      guidIndex = 0;
      return false;
    }
  }
}
