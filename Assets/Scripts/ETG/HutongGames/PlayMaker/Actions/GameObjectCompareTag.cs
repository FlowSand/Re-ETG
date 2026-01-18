using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Tests if a Game Object has a tag.")]
  [ActionCategory(ActionCategory.Logic)]
  public class GameObjectCompareTag : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject to test.")]
    [RequiredField]
    public FsmGameObject gameObject;
    [HutongGames.PlayMaker.Tooltip("The Tag to check for.")]
    [UIHint(UIHint.Tag)]
    [RequiredField]
    public FsmString tag;
    [HutongGames.PlayMaker.Tooltip("Event to send if the GameObject has the Tag.")]
    public FsmEvent trueEvent;
    [HutongGames.PlayMaker.Tooltip("Event to send if the GameObject does not have the Tag.")]
    public FsmEvent falseEvent;
    [HutongGames.PlayMaker.Tooltip("Store the result in a Bool variable.")]
    [UIHint(UIHint.Variable)]
    public FsmBool storeResult;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.gameObject = (FsmGameObject) null;
      this.tag = (FsmString) "Untagged";
      this.trueEvent = (FsmEvent) null;
      this.falseEvent = (FsmEvent) null;
      this.storeResult = (FsmBool) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoCompareTag();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoCompareTag();

    private void DoCompareTag()
    {
      bool flag = false;
      if ((Object) this.gameObject.Value != (Object) null)
        flag = this.gameObject.Value.CompareTag(this.tag.Value);
      this.storeResult.Value = flag;
      this.Fsm.Event(!flag ? this.falseEvent : this.trueEvent);
    }
  }
}
