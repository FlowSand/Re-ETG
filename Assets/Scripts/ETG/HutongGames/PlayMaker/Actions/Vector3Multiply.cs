// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.Vector3Multiply
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Multiplies a Vector3 variable by a Float.")]
  [ActionCategory(ActionCategory.Vector3)]
  public class Vector3Multiply : FsmStateAction
  {
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmVector3 vector3Variable;
    [RequiredField]
    public FsmFloat multiplyBy;
    public bool everyFrame;

    public override void Reset()
    {
      this.vector3Variable = (FsmVector3) null;
      this.multiplyBy = (FsmFloat) 1f;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.vector3Variable.Value *= this.multiplyBy.Value;
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.vector3Variable.Value *= this.multiplyBy.Value;
  }
}
