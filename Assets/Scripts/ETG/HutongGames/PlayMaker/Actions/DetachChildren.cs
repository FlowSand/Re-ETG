using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Unparents all children from the Game Object.")]
  [ActionCategory(ActionCategory.GameObject)]
  public class DetachChildren : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("GameObject to unparent children from.")]
    [RequiredField]
    public FsmOwnerDefault gameObject;

    public override void Reset() => this.gameObject = (FsmOwnerDefault) null;

    public override void OnEnter()
    {
      DetachChildren.DoDetachChildren(this.Fsm.GetOwnerDefaultTarget(this.gameObject));
      this.Finish();
    }

    private static void DoDetachChildren(GameObject go)
    {
      if (!((Object) go != (Object) null))
        return;
      go.transform.DetachChildren();
    }
  }
}
