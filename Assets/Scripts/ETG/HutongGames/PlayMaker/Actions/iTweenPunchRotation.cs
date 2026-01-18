using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Applies a jolt of force to a GameObject's rotation and wobbles it back to its initial rotation. NOTE: Due to the way iTween utilizes the Transform.Rotate method, PunchRotation works best with single axis usage rather than punching with a Vector3.")]
  [ActionCategory("iTween")]
  public class iTweenPunchRotation : iTweenFsmAction
  {
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("iTween ID. If set you can use iTween Stop action to stop it by its id.")]
    public FsmString id;
    [HutongGames.PlayMaker.Tooltip("A vector punch range.")]
    [RequiredField]
    public FsmVector3 vector;
    [HutongGames.PlayMaker.Tooltip("The time in seconds the animation will take to complete.")]
    public FsmFloat time;
    [HutongGames.PlayMaker.Tooltip("The time in seconds the animation will wait before beginning.")]
    public FsmFloat delay;
    [HutongGames.PlayMaker.Tooltip("The type of loop to apply once the animation has completed.")]
    public iTween.LoopType loopType;
    public Space space;

    public override void Reset()
    {
      base.Reset();
      FsmString fsmString = new FsmString();
      fsmString.UseVariable = true;
      this.id = fsmString;
      this.time = (FsmFloat) 1f;
      this.delay = (FsmFloat) 0.0f;
      this.loopType = iTween.LoopType.none;
      FsmVector3 fsmVector3 = new FsmVector3();
      fsmVector3.UseVariable = true;
      this.vector = fsmVector3;
      this.space = Space.World;
    }

    public override void OnEnter()
    {
      this.OnEnteriTween(this.gameObject);
      if (this.loopType != iTween.LoopType.none)
        this.IsLoop(true);
      this.DoiTween();
    }

    public override void OnExit() => this.OnExitiTween(this.gameObject);

    private void DoiTween()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget == (Object) null)
        return;
      Vector3 zero = Vector3.zero;
      if (!this.vector.IsNone)
        zero = this.vector.Value;
      this.itweenType = "punch";
      iTween.PunchRotation(ownerDefaultTarget, iTween.Hash((object) "amount", (object) zero, (object) "name", !this.id.IsNone ? (object) this.id.Value : (object) string.Empty, (object) "time", (object) (float) (!this.time.IsNone ? (double) this.time.Value : 1.0), (object) "delay", (object) (float) (!this.delay.IsNone ? (double) this.delay.Value : 0.0), (object) "looptype", (object) this.loopType, (object) "oncomplete", (object) "iTweenOnComplete", (object) "oncompleteparams", (object) this.itweenID, (object) "onstart", (object) "iTweenOnStart", (object) "onstartparams", (object) this.itweenID, (object) "ignoretimescale", (object) (bool) (!this.realTime.IsNone ? (this.realTime.Value ? 1 : 0) : 0), (object) "space", (object) this.space));
    }
  }
}
