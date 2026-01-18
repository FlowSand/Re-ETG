using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Gets the pressed state of the specified Mouse Button and stores it in a Bool Variable. See Unity Input Manager doc.")]
  [ActionCategory(ActionCategory.Input)]
  public class GetMouseButton : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The mouse button to test.")]
    [RequiredField]
    public MouseButton button;
    [HutongGames.PlayMaker.Tooltip("Store the pressed state in a Bool Variable.")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmBool storeResult;

    public override void Reset()
    {
      this.button = MouseButton.Left;
      this.storeResult = (FsmBool) null;
    }

    public override void OnEnter()
    {
      this.storeResult.Value = Input.GetMouseButton((int) this.button);
    }

    public override void OnUpdate()
    {
      this.storeResult.Value = Input.GetMouseButton((int) this.button);
    }
  }
}
