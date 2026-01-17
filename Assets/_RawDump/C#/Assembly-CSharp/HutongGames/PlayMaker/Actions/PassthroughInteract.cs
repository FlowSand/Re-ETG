// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.PassthroughInteract
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

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
