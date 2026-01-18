using System;
using System.Collections;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Changes a GameObject's position over time to a supplied destination.")]
  [ActionCategory("iTween")]
  public class iTweenMoveTo : iTweenFsmAction
  {
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("iTween ID. If set you can use iTween Stop action to stop it by its id.")]
    public FsmString id;
    [HutongGames.PlayMaker.Tooltip("Move To a transform position.")]
    public FsmGameObject transformPosition;
    [HutongGames.PlayMaker.Tooltip("Position the GameObject will animate to. If Transform Position is defined this is used as a local offset.")]
    public FsmVector3 vectorPosition;
    [HutongGames.PlayMaker.Tooltip("The time in seconds the animation will take to complete.")]
    public FsmFloat time;
    [HutongGames.PlayMaker.Tooltip("The time in seconds the animation will wait before beginning.")]
    public FsmFloat delay;
    [HutongGames.PlayMaker.Tooltip("Can be used instead of time to allow animation based on speed. When you define speed the time variable is ignored.")]
    public FsmFloat speed;
    [HutongGames.PlayMaker.Tooltip("Whether to animate in local or world space.")]
    public Space space;
    [HutongGames.PlayMaker.Tooltip("The shape of the easing curve applied to the animation.")]
    public iTween.EaseType easeType = iTween.EaseType.linear;
    [HutongGames.PlayMaker.Tooltip("The type of loop to apply once the animation has completed.")]
    public iTween.LoopType loopType;
    [HutongGames.PlayMaker.Tooltip("Whether or not the GameObject will orient to its direction of travel. False by default.")]
    [ActionSection("LookAt")]
    public FsmBool orientToPath;
    [HutongGames.PlayMaker.Tooltip("A target object the GameObject will look at.")]
    public FsmGameObject lookAtObject;
    [HutongGames.PlayMaker.Tooltip("A target position the GameObject will look at.")]
    public FsmVector3 lookAtVector;
    [HutongGames.PlayMaker.Tooltip("The time in seconds the object will take to look at either the Look Target or Orient To Path. 0 by default")]
    public FsmFloat lookTime;
    [HutongGames.PlayMaker.Tooltip("Restricts rotation to the supplied axis only.")]
    public iTweenFsmAction.AxisRestriction axis;
    [HutongGames.PlayMaker.Tooltip("Whether to automatically generate a curve from the GameObject's current position to the beginning of the path. True by default.")]
    [ActionSection("Path")]
    public FsmBool moveToPath;
    [HutongGames.PlayMaker.Tooltip("How much of a percentage (from 0 to 1) to look ahead on a path to influence how strict Orient To Path is and how much the object will anticipate each curve.")]
    public FsmFloat lookAhead;
    [CompoundArray("Path Nodes", "Transform", "Vector")]
    [HutongGames.PlayMaker.Tooltip("A list of objects to draw a Catmull-Rom spline through for a curved animation path.")]
    public FsmGameObject[] transforms;
    [HutongGames.PlayMaker.Tooltip("A list of positions to draw a Catmull-Rom through for a curved animation path. If Transform is defined, this value is added as a local offset.")]
    public FsmVector3[] vectors;
    [HutongGames.PlayMaker.Tooltip("Reverse the path so object moves from End to Start node.")]
    public FsmBool reverse;
    private Vector3[] tempVct3;

    public override void OnDrawActionGizmos()
    {
      if (this.transforms.Length < 2)
        return;
      this.tempVct3 = new Vector3[this.transforms.Length];
      for (int index = 0; index < this.transforms.Length; ++index)
        this.tempVct3[index] = !this.transforms[index].IsNone ? (!((UnityEngine.Object) this.transforms[index].Value == (UnityEngine.Object) null) ? this.transforms[index].Value.transform.position + (!this.vectors[index].IsNone ? this.vectors[index].Value : Vector3.zero) : (!this.vectors[index].IsNone ? this.vectors[index].Value : Vector3.zero)) : (!this.vectors[index].IsNone ? this.vectors[index].Value : Vector3.zero);
      iTween.DrawPathGizmos(this.tempVct3, Color.yellow);
    }

    public override void Reset()
    {
      base.Reset();
      FsmString fsmString = new FsmString();
      fsmString.UseVariable = true;
      this.id = fsmString;
      FsmGameObject fsmGameObject1 = new FsmGameObject();
      fsmGameObject1.UseVariable = true;
      this.transformPosition = fsmGameObject1;
      FsmVector3 fsmVector3_1 = new FsmVector3();
      fsmVector3_1.UseVariable = true;
      this.vectorPosition = fsmVector3_1;
      this.time = (FsmFloat) 1f;
      this.delay = (FsmFloat) 0.0f;
      this.loopType = iTween.LoopType.none;
      FsmFloat fsmFloat1 = new FsmFloat();
      fsmFloat1.UseVariable = true;
      this.speed = fsmFloat1;
      this.space = Space.World;
      this.orientToPath = new FsmBool() { Value = true };
      FsmGameObject fsmGameObject2 = new FsmGameObject();
      fsmGameObject2.UseVariable = true;
      this.lookAtObject = fsmGameObject2;
      FsmVector3 fsmVector3_2 = new FsmVector3();
      fsmVector3_2.UseVariable = true;
      this.lookAtVector = fsmVector3_2;
      FsmFloat fsmFloat2 = new FsmFloat();
      fsmFloat2.UseVariable = true;
      this.lookTime = fsmFloat2;
      this.moveToPath = (FsmBool) true;
      FsmFloat fsmFloat3 = new FsmFloat();
      fsmFloat3.UseVariable = true;
      this.lookAhead = fsmFloat3;
      this.transforms = new FsmGameObject[0];
      this.vectors = new FsmVector3[0];
      this.tempVct3 = new Vector3[0];
      this.axis = iTweenFsmAction.AxisRestriction.none;
      this.reverse = (FsmBool) false;
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
      Vector3 vector3 = !this.vectorPosition.IsNone ? this.vectorPosition.Value : Vector3.zero;
      if (!this.transformPosition.IsNone && (bool) (UnityEngine.Object) this.transformPosition.Value)
        vector3 = this.space == Space.World || (UnityEngine.Object) ownerDefaultTarget.transform.parent == (UnityEngine.Object) null ? this.transformPosition.Value.transform.position + vector3 : ownerDefaultTarget.transform.parent.InverseTransformPoint(this.transformPosition.Value.transform.position) + vector3;
      Hashtable args = new Hashtable();
      args.Add((object) "position", (object) vector3);
      args.Add(!this.speed.IsNone ? (object) "speed" : (object) "time", (object) (float) (!this.speed.IsNone ? (double) this.speed.Value : (!this.time.IsNone ? (double) this.time.Value : 1.0)));
      args.Add((object) "delay", (object) (float) (!this.delay.IsNone ? (double) this.delay.Value : 0.0));
      args.Add((object) "easetype", (object) this.easeType);
      args.Add((object) "looptype", (object) this.loopType);
      args.Add((object) "oncomplete", (object) "iTweenOnComplete");
      args.Add((object) "oncompleteparams", (object) this.itweenID);
      args.Add((object) "onstart", (object) "iTweenOnStart");
      args.Add((object) "onstartparams", (object) this.itweenID);
      args.Add((object) "ignoretimescale", (object) (bool) (!this.realTime.IsNone ? (this.realTime.Value ? 1 : 0) : 0));
      args.Add((object) "name", !this.id.IsNone ? (object) this.id.Value : (object) string.Empty);
      args.Add((object) "islocal", (object) (this.space == Space.Self));
      args.Add((object) "axis", this.axis != iTweenFsmAction.AxisRestriction.none ? (object) Enum.GetName(typeof (iTweenFsmAction.AxisRestriction), (object) this.axis) : (object) string.Empty);
      if (!this.orientToPath.IsNone)
        args.Add((object) "orienttopath", (object) this.orientToPath.Value);
      if (!this.lookAtObject.IsNone)
        args.Add((object) "looktarget", (object) (!this.lookAtVector.IsNone ? this.lookAtObject.Value.transform.position + this.lookAtVector.Value : this.lookAtObject.Value.transform.position));
      else if (!this.lookAtVector.IsNone)
        args.Add((object) "looktarget", (object) this.lookAtVector.Value);
      if (!this.lookAtObject.IsNone || !this.lookAtVector.IsNone)
        args.Add((object) "looktime", (object) (float) (!this.lookTime.IsNone ? (double) this.lookTime.Value : 0.0));
      if (this.transforms.Length >= 2)
      {
        this.tempVct3 = new Vector3[this.transforms.Length];
        if ((!this.reverse.IsNone ? (this.reverse.Value ? 1 : 0) : 0) != 0)
        {
          for (int index = 0; index < this.transforms.Length; ++index)
            this.tempVct3[this.tempVct3.Length - 1 - index] = !this.transforms[index].IsNone ? (!((UnityEngine.Object) this.transforms[index].Value == (UnityEngine.Object) null) ? (this.space != Space.World ? this.transforms[index].Value.transform.localPosition : this.transforms[index].Value.transform.position) + (!this.vectors[index].IsNone ? this.vectors[index].Value : Vector3.zero) : (!this.vectors[index].IsNone ? this.vectors[index].Value : Vector3.zero)) : (!this.vectors[index].IsNone ? this.vectors[index].Value : Vector3.zero);
        }
        else
        {
          for (int index = 0; index < this.transforms.Length; ++index)
            this.tempVct3[index] = !this.transforms[index].IsNone ? (!((UnityEngine.Object) this.transforms[index].Value == (UnityEngine.Object) null) ? (this.space != Space.World ? ownerDefaultTarget.transform.parent.InverseTransformPoint(this.transforms[index].Value.transform.position) : this.transforms[index].Value.transform.position) + (!this.vectors[index].IsNone ? this.vectors[index].Value : Vector3.zero) : (!this.vectors[index].IsNone ? this.vectors[index].Value : Vector3.zero)) : (!this.vectors[index].IsNone ? this.vectors[index].Value : Vector3.zero);
        }
        args.Add((object) "path", (object) this.tempVct3);
        args.Add((object) "movetopath", (object) (bool) (!this.moveToPath.IsNone ? (this.moveToPath.Value ? 1 : 0) : 1));
        args.Add((object) "lookahead", (object) (float) (!this.lookAhead.IsNone ? (double) this.lookAhead.Value : 1.0));
      }
      this.itweenType = "move";
      iTween.MoveTo(ownerDefaultTarget, args);
    }
  }
}
