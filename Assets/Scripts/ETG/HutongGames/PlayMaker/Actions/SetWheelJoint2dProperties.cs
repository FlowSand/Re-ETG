// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetWheelJoint2dProperties
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Physics2D)]
  [HutongGames.PlayMaker.Tooltip("Sets the various properties of a WheelJoint2d component")]
  public class SetWheelJoint2dProperties : FsmStateAction
  {
    [CheckForComponent(typeof (WheelJoint2D))]
    [HutongGames.PlayMaker.Tooltip("The WheelJoint2d target")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("Should a motor force be applied automatically to the Rigidbody2D?")]
    [ActionSection("Motor")]
    public FsmBool useMotor;
    [HutongGames.PlayMaker.Tooltip("The desired speed for the Rigidbody2D to reach as it moves with the joint.")]
    public FsmFloat motorSpeed;
    [HutongGames.PlayMaker.Tooltip("The maximum force that can be applied to the Rigidbody2D at the joint to attain the target speed.")]
    public FsmFloat maxMotorTorque;
    [ActionSection("Suspension")]
    [HutongGames.PlayMaker.Tooltip("The world angle along which the suspension will move. This provides 2D constrained motion similar to a SliderJoint2D. This is typically how suspension works in the real world.")]
    public FsmFloat angle;
    [HutongGames.PlayMaker.Tooltip("The amount by which the suspension spring force is reduced in proportion to the movement speed.")]
    public FsmFloat dampingRatio;
    [HutongGames.PlayMaker.Tooltip("The frequency at which the suspension spring oscillates.")]
    public FsmFloat frequency;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame while the state is active.")]
    public bool everyFrame;
    private WheelJoint2D _wj2d;
    private JointMotor2D _motor;
    private JointSuspension2D _suspension;

    public override void Reset()
    {
      FsmBool fsmBool = new FsmBool();
      fsmBool.UseVariable = true;
      this.useMotor = fsmBool;
      FsmFloat fsmFloat1 = new FsmFloat();
      fsmFloat1.UseVariable = true;
      this.motorSpeed = fsmFloat1;
      FsmFloat fsmFloat2 = new FsmFloat();
      fsmFloat2.UseVariable = true;
      this.maxMotorTorque = fsmFloat2;
      FsmFloat fsmFloat3 = new FsmFloat();
      fsmFloat3.UseVariable = true;
      this.angle = fsmFloat3;
      FsmFloat fsmFloat4 = new FsmFloat();
      fsmFloat4.UseVariable = true;
      this.dampingRatio = fsmFloat4;
      FsmFloat fsmFloat5 = new FsmFloat();
      fsmFloat5.UseVariable = true;
      this.frequency = fsmFloat5;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget != (Object) null)
      {
        this._wj2d = ownerDefaultTarget.GetComponent<WheelJoint2D>();
        if ((Object) this._wj2d != (Object) null)
        {
          this._motor = this._wj2d.motor;
          this._suspension = this._wj2d.suspension;
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
      if ((Object) this._wj2d == (Object) null)
        return;
      if (!this.useMotor.IsNone)
        this._wj2d.useMotor = this.useMotor.Value;
      if (!this.motorSpeed.IsNone)
      {
        this._motor.motorSpeed = this.motorSpeed.Value;
        this._wj2d.motor = this._motor;
      }
      if (!this.maxMotorTorque.IsNone)
      {
        this._motor.maxMotorTorque = this.maxMotorTorque.Value;
        this._wj2d.motor = this._motor;
      }
      if (!this.angle.IsNone)
      {
        this._suspension.angle = this.angle.Value;
        this._wj2d.suspension = this._suspension;
      }
      if (!this.dampingRatio.IsNone)
      {
        this._suspension.dampingRatio = this.dampingRatio.Value;
        this._wj2d.suspension = this._suspension;
      }
      if (this.frequency.IsNone)
        return;
      this._suspension.frequency = this.frequency.Value;
      this._wj2d.suspension = this._suspension;
    }
  }
}
