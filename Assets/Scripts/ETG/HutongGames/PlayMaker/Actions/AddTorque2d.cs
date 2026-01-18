using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Adds a 2d torque (rotational force) to a Game Object.")]
  [ActionCategory(ActionCategory.Physics2D)]
  public class AddTorque2d : ComponentAction<Rigidbody2D>
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject to add torque to.")]
    [RequiredField]
    [CheckForComponent(typeof (Rigidbody2D))]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("Option for applying the force")]
    public ForceMode2D forceMode;
    [HutongGames.PlayMaker.Tooltip("Torque")]
    public FsmFloat torque;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame while the state is active.")]
    public bool everyFrame;

    public override void OnPreprocess() => this.Fsm.HandleFixedUpdate = true;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.torque = (FsmFloat) null;
      this.everyFrame = false;
    }

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
      this.rigidbody2d.AddTorque(this.torque.Value, this.forceMode);
    }
  }
}
