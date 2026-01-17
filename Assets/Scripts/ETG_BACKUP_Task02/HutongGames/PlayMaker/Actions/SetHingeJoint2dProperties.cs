// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetHingeJoint2dProperties
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Physics2D)]
[HutongGames.PlayMaker.Tooltip("Sets the various properties of a HingeJoint2d component")]
public class SetHingeJoint2dProperties : FsmStateAction
{
  [CheckForComponent(typeof (HingeJoint2D))]
  [HutongGames.PlayMaker.Tooltip("The HingeJoint2d target")]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("Should limits be placed on the range of rotation?")]
  [ActionSection("Limits")]
  public FsmBool useLimits;
  [HutongGames.PlayMaker.Tooltip("Lower angular limit of rotation.")]
  public FsmFloat min;
  [HutongGames.PlayMaker.Tooltip("Upper angular limit of rotation")]
  public FsmFloat max;
  [HutongGames.PlayMaker.Tooltip("Should a motor force be applied automatically to the Rigidbody2D?")]
  [ActionSection("Motor")]
  public FsmBool useMotor;
  [HutongGames.PlayMaker.Tooltip("The desired speed for the Rigidbody2D to reach as it moves with the joint.")]
  public FsmFloat motorSpeed;
  [HutongGames.PlayMaker.Tooltip("The maximum force that can be applied to the Rigidbody2D at the joint to attain the target speed.")]
  public FsmFloat maxMotorTorque;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame while the state is active.")]
  public bool everyFrame;
  private HingeJoint2D _joint;
  private JointMotor2D _motor;
  private JointAngleLimits2D _limits;

  public override void Reset()
  {
    FsmBool fsmBool1 = new FsmBool();
    fsmBool1.UseVariable = true;
    this.useLimits = fsmBool1;
    FsmFloat fsmFloat1 = new FsmFloat();
    fsmFloat1.UseVariable = true;
    this.min = fsmFloat1;
    FsmFloat fsmFloat2 = new FsmFloat();
    fsmFloat2.UseVariable = true;
    this.max = fsmFloat2;
    FsmBool fsmBool2 = new FsmBool();
    fsmBool2.UseVariable = true;
    this.useMotor = fsmBool2;
    FsmFloat fsmFloat3 = new FsmFloat();
    fsmFloat3.UseVariable = true;
    this.motorSpeed = fsmFloat3;
    FsmFloat fsmFloat4 = new FsmFloat();
    fsmFloat4.UseVariable = true;
    this.maxMotorTorque = fsmFloat4;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if ((Object) ownerDefaultTarget != (Object) null)
    {
      this._joint = ownerDefaultTarget.GetComponent<HingeJoint2D>();
      if ((Object) this._joint != (Object) null)
      {
        this._motor = this._joint.motor;
        this._limits = this._joint.limits;
      }
    }
    this.SetProperties();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.SetProperties();

  private void SetProperties()
  {
    if ((Object) this._joint == (Object) null)
      return;
    if (!this.useMotor.IsNone)
      this._joint.useMotor = this.useMotor.Value;
    if (!this.motorSpeed.IsNone)
    {
      this._motor.motorSpeed = this.motorSpeed.Value;
      this._joint.motor = this._motor;
    }
    if (!this.maxMotorTorque.IsNone)
    {
      this._motor.maxMotorTorque = this.maxMotorTorque.Value;
      this._joint.motor = this._motor;
    }
    if (!this.useLimits.IsNone)
      this._joint.useLimits = this.useLimits.Value;
    if (!this.min.IsNone)
    {
      this._limits.min = this.min.Value;
      this._joint.limits = this._limits;
    }
    if (this.max.IsNone)
      return;
    this._limits.max = this.max.Value;
    this._joint.limits = this._limits;
  }
}
