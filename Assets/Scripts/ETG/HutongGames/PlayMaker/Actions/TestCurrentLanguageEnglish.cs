#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Logic)]
  public class TestCurrentLanguageEnglish : FsmStateAction
  {
    public FsmEvent EnglishEvent;
    public FsmEvent OtherEvent;

    public override void Reset()
    {
    }

    public override void OnEnter()
    {
      this.DoIDSwitch();
      this.Finish();
    }

    public override void OnUpdate() => this.DoIDSwitch();

    private void DoIDSwitch()
    {
      if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.ENGLISH)
        this.Fsm.Event(this.EnglishEvent);
      else
        this.Fsm.Event(this.OtherEvent);
    }
  }
}
