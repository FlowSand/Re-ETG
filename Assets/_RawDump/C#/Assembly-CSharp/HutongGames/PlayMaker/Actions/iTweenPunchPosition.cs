// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.iTweenPunchPosition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory("iTween")]
[HutongGames.PlayMaker.Tooltip("Applies a jolt of force to a GameObject's position and wobbles it back to its initial position.")]
public class iTweenPunchPosition : iTweenFsmAction
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
  [HutongGames.PlayMaker.Tooltip("Restricts rotation to the supplied axis only.")]
  public iTweenFsmAction.AxisRestriction axis;

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
    this.axis = iTweenFsmAction.AxisRestriction.none;
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
    if ((UnityEngine.Object) ownerDefaultTarget == (UnityEngine.Object) null)
      return;
    Vector3 zero = Vector3.zero;
    if (!this.vector.IsNone)
      zero = this.vector.Value;
    this.itweenType = "punch";
    iTween.PunchPosition(ownerDefaultTarget, iTween.Hash((object) "amount", (object) zero, (object) "name", !this.id.IsNone ? (object) this.id.Value : (object) string.Empty, (object) "time", (object) (float) (!this.time.IsNone ? (double) this.time.Value : 1.0), (object) "delay", (object) (float) (!this.delay.IsNone ? (double) this.delay.Value : 0.0), (object) "looptype", (object) this.loopType, (object) "oncomplete", (object) "iTweenOnComplete", (object) "oncompleteparams", (object) this.itweenID, (object) "onstart", (object) "iTweenOnStart", (object) "onstartparams", (object) this.itweenID, (object) "ignoretimescale", (object) (bool) (!this.realTime.IsNone ? (this.realTime.Value ? 1 : 0) : 0), (object) "space", (object) this.space, (object) "axis", this.axis != iTweenFsmAction.AxisRestriction.none ? (object) Enum.GetName(typeof (iTweenFsmAction.AxisRestriction), (object) this.axis) : (object) string.Empty));
  }
}
