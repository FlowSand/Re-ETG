#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Sets how subsequent events sent in this state are handled.")]
  [ActionCategory(ActionCategory.StateMachine)]
  public class FsmEventOptions : FsmStateAction
  {
    public PlayMakerFSM sendToFsmComponent;
    public FsmGameObject sendToGameObject;
    public FsmString fsmName;
    public FsmBool sendToChildren;
    public FsmBool broadcastToAll;

    public override void Reset()
    {
      this.sendToFsmComponent = (PlayMakerFSM) null;
      this.sendToGameObject = (FsmGameObject) null;
      this.fsmName = (FsmString) string.Empty;
      this.sendToChildren = (FsmBool) false;
      this.broadcastToAll = (FsmBool) false;
    }

    public override void OnUpdate()
    {
    }
  }
}
