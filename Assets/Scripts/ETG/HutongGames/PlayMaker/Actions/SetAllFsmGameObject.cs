#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Set the value of a Game Object Variable in another All FSM. Accept null reference")]
  [ActionCategory(ActionCategory.StateMachine)]
  public class SetAllFsmGameObject : FsmStateAction
  {
    [RequiredField]
    public FsmOwnerDefault gameObject;
    public bool everyFrame;

    public override void Reset()
    {
    }

    public override void OnEnter()
    {
      if (this.everyFrame)
        return;
      this.Finish();
    }

    private void DoSetFsmGameObject()
    {
    }
  }
}
