// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetVector3XYZ
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Sets the XYZ channels of a Vector3 Variable. To leave any channel unchanged, set variable to 'None'.")]
[ActionCategory(ActionCategory.Vector3)]
public class SetVector3XYZ : FsmStateAction
{
  [RequiredField]
  [UIHint(UIHint.Variable)]
  public FsmVector3 vector3Variable;
  [UIHint(UIHint.Variable)]
  public FsmVector3 vector3Value;
  public FsmFloat x;
  public FsmFloat y;
  public FsmFloat z;
  public bool everyFrame;

  public override void Reset()
  {
    this.vector3Variable = (FsmVector3) null;
    this.vector3Value = (FsmVector3) null;
    FsmFloat fsmFloat1 = new FsmFloat();
    fsmFloat1.UseVariable = true;
    this.x = fsmFloat1;
    FsmFloat fsmFloat2 = new FsmFloat();
    fsmFloat2.UseVariable = true;
    this.y = fsmFloat2;
    FsmFloat fsmFloat3 = new FsmFloat();
    fsmFloat3.UseVariable = true;
    this.z = fsmFloat3;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoSetVector3XYZ();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoSetVector3XYZ();

  private void DoSetVector3XYZ()
  {
    if (this.vector3Variable == null)
      return;
    Vector3 vector3 = this.vector3Variable.Value;
    if (!this.vector3Value.IsNone)
      vector3 = this.vector3Value.Value;
    if (!this.x.IsNone)
      vector3.x = this.x.Value;
    if (!this.y.IsNone)
      vector3.y = this.y.Value;
    if (!this.z.IsNone)
      vector3.z = this.z.Value;
    this.vector3Variable.Value = vector3;
  }
}
