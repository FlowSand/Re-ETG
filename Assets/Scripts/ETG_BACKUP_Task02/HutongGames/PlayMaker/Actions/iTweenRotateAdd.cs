// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.iTweenRotateAdd
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Adds supplied Euler angles in degrees to a GameObject's rotation over time.")]
[ActionCategory("iTween")]
public class iTweenRotateAdd : iTweenFsmAction
{
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("iTween ID. If set you can use iTween Stop action to stop it by its id.")]
  public FsmString id;
  [HutongGames.PlayMaker.Tooltip("A vector that will be added to a GameObjects rotation.")]
  [RequiredField]
  public FsmVector3 vector;
  [HutongGames.PlayMaker.Tooltip("The time in seconds the animation will take to complete.")]
  public FsmFloat time;
  [HutongGames.PlayMaker.Tooltip("The time in seconds the animation will wait before beginning.")]
  public FsmFloat delay;
  [HutongGames.PlayMaker.Tooltip("Can be used instead of time to allow animation based on speed. When you define speed the time variable is ignored.")]
  public FsmFloat speed;
  [HutongGames.PlayMaker.Tooltip("The shape of the easing curve applied to the animation.")]
  public iTween.EaseType easeType = iTween.EaseType.linear;
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
    FsmFloat fsmFloat = new FsmFloat();
    fsmFloat.UseVariable = true;
    this.speed = fsmFloat;
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
    this.itweenType = "rotate";
    iTween.RotateAdd(ownerDefaultTarget, iTween.Hash((object) "amount", (object) zero, (object) "name", !this.id.IsNone ? (object) this.id.Value : (object) string.Empty, !this.speed.IsNone ? (object) "speed" : (object) "time", (object) (float) (!this.speed.IsNone ? (double) this.speed.Value : (!this.time.IsNone ? (double) this.time.Value : 1.0)), (object) "delay", (object) (float) (!this.delay.IsNone ? (double) this.delay.Value : 0.0), (object) "easetype", (object) this.easeType, (object) "looptype", (object) this.loopType, (object) "oncomplete", (object) "iTweenOnComplete", (object) "oncompleteparams", (object) this.itweenID, (object) "onstart", (object) "iTweenOnStart", (object) "onstartparams", (object) this.itweenID, (object) "ignoretimescale", (object) (bool) (!this.realTime.IsNone ? (this.realTime.Value ? 1 : 0) : 0), (object) "space", (object) this.space));
  }
}
