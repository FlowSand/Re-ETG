using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Gets the number of children that a GameObject has.")]
  [ActionCategory(ActionCategory.GameObject)]
  public class GetChildCount : FsmStateAction
  {
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The GameObject to test.")]
    public FsmOwnerDefault gameObject;
    [RequiredField]
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Store the number of children in an int variable.")]
    public FsmInt storeResult;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.storeResult = (FsmInt) null;
    }

    public override void OnEnter()
    {
      this.DoGetChildCount();
      this.Finish();
    }

    private void DoGetChildCount()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget == (Object) null)
        return;
      this.storeResult.Value = ownerDefaultTarget.transform.childCount;
    }
  }
}
