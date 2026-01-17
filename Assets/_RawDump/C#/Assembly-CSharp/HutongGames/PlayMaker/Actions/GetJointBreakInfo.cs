// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetJointBreakInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[Tooltip("Gets info on the last joint break event.")]
[ActionCategory(ActionCategory.Physics)]
public class GetJointBreakInfo : FsmStateAction
{
  [Tooltip("Get the force that broke the joint.")]
  [UIHint(UIHint.Variable)]
  public FsmFloat breakForce;

  public override void Reset() => this.breakForce = (FsmFloat) null;

  public override void OnEnter()
  {
    this.breakForce.Value = this.Fsm.JointBreakForce;
    this.Finish();
  }
}
