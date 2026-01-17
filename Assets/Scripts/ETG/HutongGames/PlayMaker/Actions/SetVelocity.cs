// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetVelocity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Sets the Velocity of a Game Object. To leave any axis unchanged, set variable to 'None'. NOTE: Game object must have a rigidbody.")]
  [ActionCategory(ActionCategory.Physics)]
  public class SetVelocity : ComponentAction<Rigidbody>
  {
    [RequiredField]
    [CheckForComponent(typeof (Rigidbody))]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.Variable)]
    public FsmVector3 vector;
    public FsmFloat x;
    public FsmFloat y;
    public FsmFloat z;
    public Space space;
    public bool everyFrame;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.vector = (FsmVector3) null;
      FsmFloat fsmFloat1 = new FsmFloat();
      fsmFloat1.UseVariable = true;
      this.x = fsmFloat1;
      FsmFloat fsmFloat2 = new FsmFloat();
      fsmFloat2.UseVariable = true;
      this.y = fsmFloat2;
      FsmFloat fsmFloat3 = new FsmFloat();
      fsmFloat3.UseVariable = true;
      this.z = fsmFloat3;
      this.space = Space.Self;
      this.everyFrame = false;
    }

    public override void OnPreprocess() => this.Fsm.HandleFixedUpdate = true;

    public override void OnEnter()
    {
      this.DoSetVelocity();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnFixedUpdate()
    {
      this.DoSetVelocity();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    private void DoSetVelocity()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if (!this.UpdateCache(ownerDefaultTarget))
        return;
      Vector3 direction = !this.vector.IsNone ? this.vector.Value : (this.space != Space.World ? ownerDefaultTarget.transform.InverseTransformDirection(this.rigidbody.velocity) : this.rigidbody.velocity);
      if (!this.x.IsNone)
        direction.x = this.x.Value;
      if (!this.y.IsNone)
        direction.y = this.y.Value;
      if (!this.z.IsNone)
        direction.z = this.z.Value;
      this.rigidbody.velocity = this.space != Space.World ? ownerDefaultTarget.transform.TransformDirection(direction) : direction;
    }
  }
}
