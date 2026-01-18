#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(".Brave")]
  [Tooltip("Sends Events based on whether or not the player is in the foyer.")]
  public class TestInFoyer : FsmStateAction
  {
    [Tooltip("Event to send if the player is in the foyer.")]
    public FsmEvent isTrue;
    [Tooltip("Event to send if the player is not in the foyer.")]
    public FsmEvent isFalse;
    [Tooltip("Repeat every frame while the state is active.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.isTrue = (FsmEvent) null;
      this.isFalse = (FsmEvent) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.Fsm.Event(!GameManager.Instance.IsFoyer ? this.isFalse : this.isTrue);
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate()
    {
      this.Fsm.Event(!GameManager.Instance.IsFoyer ? this.isFalse : this.isTrue);
    }
  }
}
