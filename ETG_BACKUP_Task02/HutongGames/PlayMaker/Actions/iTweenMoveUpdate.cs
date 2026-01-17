// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.iTweenMoveUpdate
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Similar to MoveTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a 'live' set of changing values. Does not utilize an EaseType.")]
[ActionCategory("iTween")]
public class iTweenMoveUpdate : FsmStateAction
{
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("Move From a transform rotation.")]
  public FsmGameObject transformPosition;
  [HutongGames.PlayMaker.Tooltip("The position the GameObject will animate from.  If transformPosition is set, this is used as an offset.")]
  public FsmVector3 vectorPosition;
  [HutongGames.PlayMaker.Tooltip("The time in seconds the animation will take to complete.")]
  public FsmFloat time;
  [HutongGames.PlayMaker.Tooltip("Whether to animate in local or world space.")]
  public Space space;
  [HutongGames.PlayMaker.Tooltip("Whether or not the GameObject will orient to its direction of travel. False by default.")]
  [ActionSection("LookAt")]
  public FsmBool orientToPath;
  [HutongGames.PlayMaker.Tooltip("A target object the GameObject will look at.")]
  public FsmGameObject lookAtObject;
  [HutongGames.PlayMaker.Tooltip("A target position the GameObject will look at.")]
  public FsmVector3 lookAtVector;
  [HutongGames.PlayMaker.Tooltip("The time in seconds the object will take to look at either the Look At Target or Orient To Path. 0 by default")]
  public FsmFloat lookTime;
  [HutongGames.PlayMaker.Tooltip("Restricts rotation to the supplied axis only.")]
  public iTweenFsmAction.AxisRestriction axis;
  private Hashtable hash;
  private GameObject go;

  public override void Reset()
  {
    FsmGameObject fsmGameObject1 = new FsmGameObject();
    fsmGameObject1.UseVariable = true;
    this.transformPosition = fsmGameObject1;
    FsmVector3 fsmVector3_1 = new FsmVector3();
    fsmVector3_1.UseVariable = true;
    this.vectorPosition = fsmVector3_1;
    this.time = (FsmFloat) 1f;
    this.space = Space.World;
    this.orientToPath = new FsmBool() { Value = true };
    FsmGameObject fsmGameObject2 = new FsmGameObject();
    fsmGameObject2.UseVariable = true;
    this.lookAtObject = fsmGameObject2;
    FsmVector3 fsmVector3_2 = new FsmVector3();
    fsmVector3_2.UseVariable = true;
    this.lookAtVector = fsmVector3_2;
    this.lookTime = (FsmFloat) 0.0f;
    this.axis = iTweenFsmAction.AxisRestriction.none;
  }

  public override void OnEnter()
  {
    this.hash = new Hashtable();
    this.go = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if ((UnityEngine.Object) this.go == (UnityEngine.Object) null)
    {
      this.Finish();
    }
    else
    {
      if (this.transformPosition.IsNone)
        this.hash.Add((object) "position", (object) (!this.vectorPosition.IsNone ? this.vectorPosition.Value : Vector3.zero));
      else if (this.vectorPosition.IsNone)
        this.hash.Add((object) "position", (object) this.transformPosition.Value.transform);
      else if (this.space == Space.World || (UnityEngine.Object) this.go.transform.parent == (UnityEngine.Object) null)
        this.hash.Add((object) "position", (object) (this.transformPosition.Value.transform.position + this.vectorPosition.Value));
      else
        this.hash.Add((object) "position", (object) (this.go.transform.parent.InverseTransformPoint(this.transformPosition.Value.transform.position) + this.vectorPosition.Value));
      this.hash.Add((object) "time", (object) (float) (!this.time.IsNone ? (double) this.time.Value : 1.0));
      this.hash.Add((object) "islocal", (object) (this.space == Space.Self));
      this.hash.Add((object) "axis", this.axis != iTweenFsmAction.AxisRestriction.none ? (object) Enum.GetName(typeof (iTweenFsmAction.AxisRestriction), (object) this.axis) : (object) string.Empty);
      if (!this.orientToPath.IsNone)
        this.hash.Add((object) "orienttopath", (object) this.orientToPath.Value);
      if (this.lookAtObject.IsNone)
      {
        if (!this.lookAtVector.IsNone)
          this.hash.Add((object) "looktarget", (object) this.lookAtVector.Value);
      }
      else
        this.hash.Add((object) "looktarget", (object) this.lookAtObject.Value.transform);
      if (!this.lookAtObject.IsNone || !this.lookAtVector.IsNone)
        this.hash.Add((object) "looktime", (object) (float) (!this.lookTime.IsNone ? (double) this.lookTime.Value : 0.0));
      this.DoiTween();
    }
  }

  public override void OnUpdate()
  {
    this.hash.Remove((object) "position");
    if (this.transformPosition.IsNone)
      this.hash.Add((object) "position", (object) (!this.vectorPosition.IsNone ? this.vectorPosition.Value : Vector3.zero));
    else if (this.vectorPosition.IsNone)
      this.hash.Add((object) "position", (object) this.transformPosition.Value.transform);
    else if (this.space == Space.World)
      this.hash.Add((object) "position", (object) (this.transformPosition.Value.transform.position + this.vectorPosition.Value));
    else
      this.hash.Add((object) "position", (object) (this.transformPosition.Value.transform.localPosition + this.vectorPosition.Value));
    this.DoiTween();
  }

  public override void OnExit()
  {
  }

  private void DoiTween() => iTween.MoveUpdate(this.go, this.hash);
}
