using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Gets the top most parent of the Game Object.\nIf the game object has no parent, returns itself.")]
  [ActionCategory(ActionCategory.GameObject)]
  public class GetRoot : FsmStateAction
  {
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmGameObject storeRoot;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.storeRoot = (FsmGameObject) null;
    }

    public override void OnEnter()
    {
      this.DoGetRoot();
      this.Finish();
    }

    private void DoGetRoot()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget == (Object) null)
        return;
      this.storeRoot.Value = ownerDefaultTarget.transform.root.gameObject;
    }
  }
}
