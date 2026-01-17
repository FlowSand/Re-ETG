// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.iTweenMoveBy
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Adds the supplied vector to a GameObject's position.")]
[ActionCategory("iTween")]
public class iTweenMoveBy : iTweenFsmAction
{
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("iTween ID. If set you can use iTween Stop action to stop it by its id.")]
  public FsmString id;
  [HutongGames.PlayMaker.Tooltip("The vector to add to the GameObject's position.")]
  [RequiredField]
  public FsmVector3 vector;
  [HutongGames.PlayMaker.Tooltip("For the time in seconds the animation will take to complete.")]
  public FsmFloat time;
  [HutongGames.PlayMaker.Tooltip("For the time in seconds the animation will wait before beginning.")]
  public FsmFloat delay;
  [HutongGames.PlayMaker.Tooltip("Can be used instead of time to allow animation based on speed. When you define speed the time variable is ignored.")]
  public FsmFloat speed;
  [HutongGames.PlayMaker.Tooltip("For the shape of the easing curve applied to the animation.")]
  public iTween.EaseType easeType = iTween.EaseType.linear;
  [HutongGames.PlayMaker.Tooltip("For the type of loop to apply once the animation has completed.")]
  public iTween.LoopType loopType;
  public Space space;
  [HutongGames.PlayMaker.Tooltip("For whether or not the GameObject will orient to its direction of travel. False by default.")]
  [ActionSection("LookAt")]
  public FsmBool orientToPath;
  [HutongGames.PlayMaker.Tooltip("For a target the GameObject will look at.")]
  public FsmGameObject lookAtObject;
  [HutongGames.PlayMaker.Tooltip("For a target the GameObject will look at.")]
  public FsmVector3 lookAtVector;
  [HutongGames.PlayMaker.Tooltip("For the time in seconds the object will take to look at either the 'looktarget' or 'orienttopath'. 0 by default")]
  public FsmFloat lookTime;
  [HutongGames.PlayMaker.Tooltip("Restricts rotation to the supplied axis only. Just put there strinc like 'x' or 'xz'")]
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
    FsmVector3 fsmVector3_1 = new FsmVector3();
    fsmVector3_1.UseVariable = true;
    this.vector = fsmVector3_1;
    FsmFloat fsmFloat = new FsmFloat();
    fsmFloat.UseVariable = true;
    this.speed = fsmFloat;
    this.space = Space.World;
    this.orientToPath = (FsmBool) false;
    FsmGameObject fsmGameObject = new FsmGameObject();
    fsmGameObject.UseVariable = true;
    this.lookAtObject = fsmGameObject;
    FsmVector3 fsmVector3_2 = new FsmVector3();
    fsmVector3_2.UseVariable = true;
    this.lookAtVector = fsmVector3_2;
    this.lookTime = (FsmFloat) 0.0f;
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
    Hashtable args = new Hashtable();
    args.Add((object) "amount", (object) (!this.vector.IsNone ? this.vector.Value : Vector3.zero));
    args.Add(!this.speed.IsNone ? (object) "speed" : (object) "time", (object) (float) (!this.speed.IsNone ? (double) this.speed.Value : (!this.time.IsNone ? (double) this.time.Value : 1.0)));
    args.Add((object) "delay", (object) (float) (!this.delay.IsNone ? (double) this.delay.Value : 0.0));
    args.Add((object) "easetype", (object) this.easeType);
    args.Add((object) "looptype", (object) this.loopType);
    args.Add((object) "oncomplete", (object) "iTweenOnComplete");
    args.Add((object) "oncompleteparams", (object) this.itweenID);
    args.Add((object) "onstart", (object) "iTweenOnStart");
    args.Add((object) "onstartparams", (object) this.itweenID);
    args.Add((object) "ignoretimescale", (object) (bool) (!this.realTime.IsNone ? (this.realTime.Value ? 1 : 0) : 0));
    args.Add((object) "space", (object) this.space);
    args.Add((object) "name", !this.id.IsNone ? (object) this.id.Value : (object) string.Empty);
    args.Add((object) "axis", this.axis != iTweenFsmAction.AxisRestriction.none ? (object) Enum.GetName(typeof (iTweenFsmAction.AxisRestriction), (object) this.axis) : (object) string.Empty);
    if (!this.orientToPath.IsNone)
      args.Add((object) "orienttopath", (object) this.orientToPath.Value);
    if (!this.lookAtObject.IsNone)
      args.Add((object) "looktarget", (object) (!this.lookAtVector.IsNone ? this.lookAtObject.Value.transform.position + this.lookAtVector.Value : this.lookAtObject.Value.transform.position));
    else if (!this.lookAtVector.IsNone)
      args.Add((object) "looktarget", (object) this.lookAtVector.Value);
    if (!this.lookAtObject.IsNone || !this.lookAtVector.IsNone)
      args.Add((object) "looktime", (object) (float) (!this.lookTime.IsNone ? (double) this.lookTime.Value : 0.0));
    this.itweenType = "move";
    iTween.MoveBy(ownerDefaultTarget, args);
  }
}
