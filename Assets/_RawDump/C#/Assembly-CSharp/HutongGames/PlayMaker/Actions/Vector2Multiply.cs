// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.Vector2Multiply
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Vector2)]
[Tooltip("Multiplies a Vector2 variable by a Float.")]
public class Vector2Multiply : FsmStateAction
{
  [Tooltip("The vector to Multiply")]
  [UIHint(UIHint.Variable)]
  [RequiredField]
  public FsmVector2 vector2Variable;
  [Tooltip("The multiplication factor")]
  [RequiredField]
  public FsmFloat multiplyBy;
  [Tooltip("Repeat every frame")]
  public bool everyFrame;

  public override void Reset()
  {
    this.vector2Variable = (FsmVector2) null;
    this.multiplyBy = (FsmFloat) 1f;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.vector2Variable.Value *= this.multiplyBy.Value;
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.vector2Variable.Value *= this.multiplyBy.Value;
}
