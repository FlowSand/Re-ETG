using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Network)]
  [HutongGames.PlayMaker.Tooltip("Test if the Network View is controlled by a GameObject.")]
  public class NetworkViewIsMine : FsmStateAction
  {
    [CheckForComponent(typeof (NetworkView))]
    [HutongGames.PlayMaker.Tooltip("The Game Object with the NetworkView attached.")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("True if the network view is controlled by this object.")]
    public FsmBool isMine;
    [HutongGames.PlayMaker.Tooltip("Send this event if the network view controlled by this object.")]
    public FsmEvent isMineEvent;
    [HutongGames.PlayMaker.Tooltip("Send this event if the network view is NOT controlled by this object.")]
    public FsmEvent isNotMineEvent;
    private NetworkView _networkView;

    private void _getNetworkView()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget == (Object) null)
        return;
      this._networkView = ownerDefaultTarget.GetComponent<NetworkView>();
    }

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.isMine = (FsmBool) null;
      this.isMineEvent = (FsmEvent) null;
      this.isNotMineEvent = (FsmEvent) null;
    }

    public override void OnEnter()
    {
      this._getNetworkView();
      this.checkIsMine();
      this.Finish();
    }

    private void checkIsMine()
    {
      if ((Object) this._networkView == (Object) null)
        return;
      bool isMine = this._networkView.isMine;
      this.isMine.Value = isMine;
      if (isMine)
      {
        if (this.isMineEvent == null)
          return;
        this.Fsm.Event(this.isMineEvent);
      }
      else
      {
        if (this.isNotMineEvent == null)
          return;
        this.Fsm.Event(this.isNotMineEvent);
      }
    }
  }
}
