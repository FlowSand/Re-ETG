// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetGameObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.GameObject)]
[Tooltip("Sets the value of a Game Object Variable.")]
public class SetGameObject : FsmStateAction
{
  [UIHint(UIHint.Variable)]
  [RequiredField]
  public FsmGameObject variable;
  public FsmGameObject gameObject;
  public bool everyFrame;

  public override void Reset()
  {
    this.variable = (FsmGameObject) null;
    this.gameObject = (FsmGameObject) null;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.variable.Value = this.gameObject.Value;
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.variable.Value = this.gameObject.Value;
}
