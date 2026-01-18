using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Logic)]
  [HutongGames.PlayMaker.Tooltip("Tests if a GameObject has children.")]
  public class GameObjectHasChildren : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject to test.")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("Event to send if the GameObject has children.")]
    public FsmEvent trueEvent;
    [HutongGames.PlayMaker.Tooltip("Event to send if the GameObject does not have children.")]
    public FsmEvent falseEvent;
    [HutongGames.PlayMaker.Tooltip("Store the result in a bool variable.")]
    [UIHint(UIHint.Variable)]
    public FsmBool storeResult;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.trueEvent = (FsmEvent) null;
      this.falseEvent = (FsmEvent) null;
      this.storeResult = (FsmBool) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoHasChildren();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoHasChildren();

    private void DoHasChildren()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget == (Object) null)
        return;
      bool flag = ownerDefaultTarget.transform.childCount > 0;
      this.storeResult.Value = flag;
      this.Fsm.Event(!flag ? this.falseEvent : this.trueEvent);
    }
  }
}
