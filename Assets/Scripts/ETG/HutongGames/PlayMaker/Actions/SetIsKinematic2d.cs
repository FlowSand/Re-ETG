using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Controls whether 2D physics affects the Game Object.")]
  [ActionCategory(ActionCategory.Physics2D)]
  public class SetIsKinematic2d : ComponentAction<Rigidbody2D>
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject with the Rigidbody2D attached")]
    [CheckForComponent(typeof (Rigidbody2D))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("The isKinematic value")]
    [RequiredField]
    public FsmBool isKinematic;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.isKinematic = (FsmBool) false;
    }

    public override void OnEnter()
    {
      this.DoSetIsKinematic();
      this.Finish();
    }

    private void DoSetIsKinematic()
    {
      if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
        return;
      this.rigidbody2d.isKinematic = this.isKinematic.Value;
    }
  }
}
