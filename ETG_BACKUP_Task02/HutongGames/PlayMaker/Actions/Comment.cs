// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.Comment
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Debug)]
[Tooltip("Adds a text area to the action list. NOTE: Doesn't do anything, just for notes...")]
public class Comment : FsmStateAction
{
  [UIHint(UIHint.Comment)]
  public string comment;

  public override void Reset() => this.comment = string.Empty;

  public override void OnEnter() => this.Finish();
}
