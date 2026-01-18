using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Sets if the Application should play in the background. Useful for servers or testing network games on one machine.")]
  [ActionCategory(ActionCategory.Application)]
  public class ApplicationRunInBackground : FsmStateAction
  {
    public FsmBool runInBackground;

    public override void Reset() => this.runInBackground = (FsmBool) true;

    public override void OnEnter()
    {
      Application.runInBackground = this.runInBackground.Value;
      this.Finish();
    }
  }
}
