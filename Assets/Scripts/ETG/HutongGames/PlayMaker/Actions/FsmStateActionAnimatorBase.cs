#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  public abstract class FsmStateActionAnimatorBase : FsmStateAction
  {
    [Tooltip("Repeat every frame.")]
    public bool everyFrame;
    [Tooltip("Select when to perform the action, during OnUpdate, OnAnimatorMove, OnAnimatorIK")]
    public FsmStateActionAnimatorBase.AnimatorFrameUpdateSelector everyFrameOption;
    protected int IklayerIndex;

    public abstract void OnActionUpdate();

    public override void Reset()
    {
      this.everyFrame = false;
      this.everyFrameOption = FsmStateActionAnimatorBase.AnimatorFrameUpdateSelector.OnUpdate;
    }

    public override void OnPreprocess()
    {
      if (this.everyFrameOption == FsmStateActionAnimatorBase.AnimatorFrameUpdateSelector.OnAnimatorMove)
        this.Fsm.HandleAnimatorMove = true;
      if (this.everyFrameOption != FsmStateActionAnimatorBase.AnimatorFrameUpdateSelector.OnAnimatorIK)
        return;
      this.Fsm.HandleAnimatorIK = true;
    }

    public override void OnUpdate()
    {
      if (this.everyFrameOption == FsmStateActionAnimatorBase.AnimatorFrameUpdateSelector.OnUpdate)
        this.OnActionUpdate();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void DoAnimatorMove()
    {
      if (this.everyFrameOption == FsmStateActionAnimatorBase.AnimatorFrameUpdateSelector.OnAnimatorMove)
        this.OnActionUpdate();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void DoAnimatorIK(int layerIndex)
    {
      this.IklayerIndex = layerIndex;
      if (this.everyFrameOption == FsmStateActionAnimatorBase.AnimatorFrameUpdateSelector.OnAnimatorIK)
        this.OnActionUpdate();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public enum AnimatorFrameUpdateSelector
    {
      OnUpdate,
      OnAnimatorMove,
      OnAnimatorIK,
    }
  }
}
