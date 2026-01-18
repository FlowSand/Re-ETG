using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Gets the Parent of a Game Object.")]
  [ActionCategory(ActionCategory.GameObject)]
  public class GetParent : FsmStateAction
  {
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.Variable)]
    public FsmGameObject storeResult;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.storeResult = (FsmGameObject) null;
    }

    public override void OnEnter()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      this.storeResult.Value = !((Object) ownerDefaultTarget != (Object) null) ? (GameObject) null : (!((Object) ownerDefaultTarget.transform.parent == (Object) null) ? ownerDefaultTarget.transform.parent.gameObject : (GameObject) null);
      this.Finish();
    }
  }
}
