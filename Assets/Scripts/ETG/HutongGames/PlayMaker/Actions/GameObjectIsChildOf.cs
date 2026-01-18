using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Tests if a GameObject is a Child of another GameObject.")]
  [ActionCategory(ActionCategory.Logic)]
  public class GameObjectIsChildOf : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("GameObject to test.")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("Is it a child of this GameObject?")]
    [RequiredField]
    public FsmGameObject isChildOf;
    [HutongGames.PlayMaker.Tooltip("Event to send if GameObject is a child.")]
    public FsmEvent trueEvent;
    [HutongGames.PlayMaker.Tooltip("Event to send if GameObject is NOT a child.")]
    public FsmEvent falseEvent;
    [HutongGames.PlayMaker.Tooltip("Store result in a bool variable")]
    [RequiredField]
    [UIHint(UIHint.Variable)]
    public FsmBool storeResult;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.isChildOf = (FsmGameObject) null;
      this.trueEvent = (FsmEvent) null;
      this.falseEvent = (FsmEvent) null;
      this.storeResult = (FsmBool) null;
    }

    public override void OnEnter()
    {
      this.DoIsChildOf(this.Fsm.GetOwnerDefaultTarget(this.gameObject));
      this.Finish();
    }

    private void DoIsChildOf(GameObject go)
    {
      if ((Object) go == (Object) null || this.isChildOf == null)
        return;
      bool flag = go.transform.IsChildOf(this.isChildOf.Value.transform);
      this.storeResult.Value = flag;
      this.Fsm.Event(!flag ? this.falseEvent : this.trueEvent);
    }
  }
}
