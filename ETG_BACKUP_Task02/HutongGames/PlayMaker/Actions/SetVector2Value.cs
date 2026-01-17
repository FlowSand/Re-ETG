// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetVector2Value
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[Tooltip("Sets the value of a Vector2 Variable.")]
[ActionCategory(ActionCategory.Vector2)]
public class SetVector2Value : FsmStateAction
{
  [RequiredField]
  [UIHint(UIHint.Variable)]
  [Tooltip("The vector2 target")]
  public FsmVector2 vector2Variable;
  [Tooltip("The vector2 source")]
  [RequiredField]
  public FsmVector2 vector2Value;
  [Tooltip("Repeat every frame.")]
  public bool everyFrame;

  public override void Reset()
  {
    this.vector2Variable = (FsmVector2) null;
    this.vector2Value = (FsmVector2) null;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.vector2Variable.Value = this.vector2Value.Value;
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.vector2Variable.Value = this.vector2Value.Value;
}
