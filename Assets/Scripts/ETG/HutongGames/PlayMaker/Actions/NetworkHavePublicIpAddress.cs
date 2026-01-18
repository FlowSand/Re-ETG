using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Check if this machine has a public IP address.")]
  [ActionCategory(ActionCategory.Network)]
  public class NetworkHavePublicIpAddress : FsmStateAction
  {
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("True if this machine has a public IP address")]
    public FsmBool havePublicIpAddress;
    [HutongGames.PlayMaker.Tooltip("Event to send if this machine has a public IP address")]
    public FsmEvent publicIpAddressFoundEvent;
    [HutongGames.PlayMaker.Tooltip("Event to send if this machine has no public IP address")]
    public FsmEvent publicIpAddressNotFoundEvent;

    public override void Reset()
    {
      this.havePublicIpAddress = (FsmBool) null;
      this.publicIpAddressFoundEvent = (FsmEvent) null;
      this.publicIpAddressNotFoundEvent = (FsmEvent) null;
    }

    public override void OnEnter()
    {
      bool flag = Network.HavePublicAddress();
      this.havePublicIpAddress.Value = flag;
      if (flag && this.publicIpAddressFoundEvent != null)
        this.Fsm.Event(this.publicIpAddressFoundEvent);
      else if (!flag && this.publicIpAddressNotFoundEvent != null)
        this.Fsm.Event(this.publicIpAddressNotFoundEvent);
      this.Finish();
    }
  }
}
