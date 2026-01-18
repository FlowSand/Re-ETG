using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Gets the X Position of the mouse and stores it in a Float Variable.")]
  [ActionCategory(ActionCategory.Input)]
  public class GetMouseX : FsmStateAction
  {
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmFloat storeResult;
    public bool normalize;

    public override void Reset()
    {
      this.storeResult = (FsmFloat) null;
      this.normalize = true;
    }

    public override void OnEnter() => this.DoGetMouseX();

    public override void OnUpdate() => this.DoGetMouseX();

    private void DoGetMouseX()
    {
      if (this.storeResult == null)
        return;
      float x = Input.mousePosition.x;
      if (this.normalize)
        x /= (float) Screen.width;
      this.storeResult.Value = x;
    }
  }
}
