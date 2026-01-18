using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Gets the Height of the Screen in pixels.")]
  [ActionCategory(ActionCategory.Application)]
  public class GetScreenHeight : FsmStateAction
  {
    [RequiredField]
    [UIHint(UIHint.Variable)]
    public FsmFloat storeScreenHeight;

    public override void Reset() => this.storeScreenHeight = (FsmFloat) null;

    public override void OnEnter()
    {
      this.storeScreenHeight.Value = (float) Screen.height;
      this.Finish();
    }
  }
}
