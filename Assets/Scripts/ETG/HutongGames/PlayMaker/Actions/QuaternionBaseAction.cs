#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  public abstract class QuaternionBaseAction : FsmStateAction
  {
    [Tooltip("Repeat every frame. Useful if any of the values are changing.")]
    public bool everyFrame;
    [Tooltip("Defines how to perform the action when 'every Frame' is enabled.")]
    public QuaternionBaseAction.everyFrameOptions everyFrameOption;

    public override void Awake()
    {
      if (!this.everyFrame || this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.FixedUpdate)
        return;
      this.Fsm.HandleFixedUpdate = true;
    }

    public enum everyFrameOptions
    {
      Update,
      FixedUpdate,
      LateUpdate,
    }
  }
}
