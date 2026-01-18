using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Application)]
  [HutongGames.PlayMaker.Tooltip("Gets the Width of the Screen in pixels.")]
  public class GetScreenWidth : FsmStateAction
  {
    [RequiredField]
    [UIHint(UIHint.Variable)]
    public FsmFloat storeScreenWidth;

    public override void Reset() => this.storeScreenWidth = (FsmFloat) null;

    public override void OnEnter()
    {
      this.storeScreenWidth.Value = (float) Screen.width;
      this.Finish();
    }
  }
}
