using System;
using System.Collections;

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Instantly changes a GameObject's position to a supplied destination then returns it to it's starting position over time.")]
    [ActionCategory("iTween")]
    public class iTweenMoveFrom : iTweenFsmAction
    {
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("iTween ID. If set you can use iTween Stop action to stop it by its id.")]
        public FsmString id;
        [HutongGames.PlayMaker.Tooltip("Move From a transform rotation.")]
        public FsmGameObject transformPosition;
        [HutongGames.PlayMaker.Tooltip("The position the GameObject will animate from. If Transform Position is defined this is used as a local offset.")]
        public FsmVector3 vectorPosition;
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
        [HutongGames.PlayMaker.Tooltip("Whether to animate in local or world space.")]
        public Space space;
        [ActionSection("LookAt")]
        [HutongGames.PlayMaker.Tooltip("Whether or not the GameObject will orient to its direction of travel. False by default.")]
        public FsmBool orientToPath;
        [HutongGames.PlayMaker.Tooltip("A target object the GameObject will look at.")]
        public FsmGameObject lookAtObject;
        [HutongGames.PlayMaker.Tooltip("A target position the GameObject will look at.")]
        public FsmVector3 lookAtVector;
        [HutongGames.PlayMaker.Tooltip("The time in seconds the object will take to look at either the Look At Target or Orient To Path. 0 by default")]
        public FsmFloat lookTime;
        [HutongGames.PlayMaker.Tooltip("Restricts rotation to the supplied axis only.")]
        public iTweenFsmAction.AxisRestriction axis;

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
            FsmFloat fsmFloat = new FsmFloat();
            fsmFloat.UseVariable = true;
            this.speed = fsmFloat;
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
            args.Add((object) "islocal", (object) (this.space == Space.Self));
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
            iTween.MoveFrom(ownerDefaultTarget, args);
        }
    }
}
