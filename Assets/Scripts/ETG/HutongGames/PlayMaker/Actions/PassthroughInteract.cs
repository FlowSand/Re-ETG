#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(".NPCs")]
  public class PassthroughInteract : FsmStateAction
  {
    public TalkDoerLite TargetTalker;
    private TalkDoerLite m_talkDoer;

    public override void Reset()
    {
    }

    public override string ErrorCheck() => string.Empty;

    public override void OnEnter()
    {
      this.TargetTalker.Interact(GameManager.Instance.PrimaryPlayer);
      this.Finish();
    }
  }
}
