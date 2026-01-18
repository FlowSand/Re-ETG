using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Adds torque (rotational force) to a Game Object.")]
  [ActionCategory(ActionCategory.Physics)]
  public class AddTorque : ComponentAction<Rigidbody>
  {
    [CheckForComponent(typeof (Rigidbody))]
    [HutongGames.PlayMaker.Tooltip("The GameObject to add torque to.")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("A Vector3 torque. Optionally override any axis with the X, Y, Z parameters.")]
    public FsmVector3 vector;
    [HutongGames.PlayMaker.Tooltip("Torque around the X axis. To leave unchanged, set to 'None'.")]
    public FsmFloat x;
    [HutongGames.PlayMaker.Tooltip("Torque around the Y axis. To leave unchanged, set to 'None'.")]
    public FsmFloat y;
    [HutongGames.PlayMaker.Tooltip("Torque around the Z axis. To leave unchanged, set to 'None'.")]
    public FsmFloat z;
    [HutongGames.PlayMaker.Tooltip("Apply the force in world or local space.")]
    public Space space;
    [HutongGames.PlayMaker.Tooltip("The type of force to apply. See Unity Physics docs.")]
    public ForceMode forceMode;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame while the state is active.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      FsmFloat fsmFloat1 = new FsmFloat();
      fsmFloat1.UseVariable = true;
      this.x = fsmFloat1;
      FsmFloat fsmFloat2 = new FsmFloat();
      fsmFloat2.UseVariable = true;
      this.y = fsmFloat2;
      FsmFloat fsmFloat3 = new FsmFloat();
      fsmFloat3.UseVariable = true;
      this.z = fsmFloat3;
      this.space = Space.World;
      this.forceMode = ForceMode.Force;
      this.everyFrame = false;
    }

    public override void OnPreprocess() => this.Fsm.HandleFixedUpdate = true;

    public override void OnEnter()
    {
      this.DoAddTorque();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnFixedUpdate() => this.DoAddTorque();

    private void DoAddTorque()
    {
      if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
        return;
      Vector3 torque = !this.vector.IsNone ? this.vector.Value : new Vector3(this.x.Value, this.y.Value, this.z.Value);
      if (!this.x.IsNone)
        torque.x = this.x.Value;
      if (!this.y.IsNone)
        torque.y = this.y.Value;
      if (!this.z.IsNone)
        torque.z = this.z.Value;
      if (this.space == Space.World)
        this.rigidbody.AddTorque(torque, this.forceMode);
      else
        this.rigidbody.AddRelativeTorque(torque, this.forceMode);
    }
  }
}
