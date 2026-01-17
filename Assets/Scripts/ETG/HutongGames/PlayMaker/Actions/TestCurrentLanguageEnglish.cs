// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.TestCurrentLanguageEnglish
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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
