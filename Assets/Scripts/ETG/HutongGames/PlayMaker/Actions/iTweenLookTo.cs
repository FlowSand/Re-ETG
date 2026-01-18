using System;

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("iTween")]
    [HutongGames.PlayMaker.Tooltip("Rotates a GameObject to look at a supplied Transform or Vector3 over time.")]
    public class iTweenLookTo : iTweenFsmAction
    {
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("iTween ID. If set you can use iTween Stop action to stop it by its id.")]
        public FsmString id;
        [HutongGames.PlayMaker.Tooltip("Look at a transform position.")]
        public FsmGameObject transformTarget;
        [HutongGames.PlayMaker.Tooltip("A target position the GameObject will look at. If Transform Target is defined this is used as a local offset.")]
        public FsmVector3 vectorTarget;
        [HutongGames.PlayMaker.Tooltip("The time in seconds the animation will take to complete.")]
        public FsmFloat time;
        [HutongGames.PlayMaker.Tooltip("The time in seconds the animation will wait before beginning.")]
        public FsmFloat delay;
        [HutongGames.PlayMaker.Tooltip("Can be used instead of time to allow animation based on speed. When you define speed the time variable is ignored.")]
        public FsmFloat speed;
        [HutongGames.PlayMaker.Tooltip("For the shape of the easing curve applied to the animation.")]
        public iTween.EaseType easeType = iTween.EaseType.linear;
        [HutongGames.PlayMaker.Tooltip("The type of loop to apply once the animation has completed.")]
        public iTween.LoopType loopType;
        [HutongGames.PlayMaker.Tooltip("Restricts rotation to the supplied axis only. Just put there strinc like 'x' or 'xz'")]
        public iTweenFsmAction.AxisRestriction axis;

        public override void Reset()
        {
            base.Reset();
            FsmString fsmString = new FsmString();
            fsmString.UseVariable = true;
            this.id = fsmString;
            FsmGameObject fsmGameObject = new FsmGameObject();
            fsmGameObject.UseVariable = true;
            this.transformTarget = fsmGameObject;
            FsmVector3 fsmVector3 = new FsmVector3();
            fsmVector3.UseVariable = true;
            this.vectorTarget = fsmVector3;
            this.time = (FsmFloat) 1f;
            this.delay = (FsmFloat) 0.0f;
            this.loopType = iTween.LoopType.none;
            FsmFloat fsmFloat = new FsmFloat();
            fsmFloat.UseVariable = true;
            this.speed = fsmFloat;
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
            Vector3 vector3 = !this.vectorTarget.IsNone ? this.vectorTarget.Value : Vector3.zero;
            if (!this.transformTarget.IsNone && (bool) (UnityEngine.Object) this.transformTarget.Value)
                vector3 = this.transformTarget.Value.transform.position + vector3;
            this.itweenType = "rotate";
            iTween.LookTo(ownerDefaultTarget, iTween.Hash((object) "looktarget", (object) vector3, (object) "name", !this.id.IsNone ? (object) this.id.Value : (object) string.Empty, !this.speed.IsNone ? (object) "speed" : (object) "time", (object) (float) (!this.speed.IsNone ? (double) this.speed.Value : (!this.time.IsNone ? (double) this.time.Value : 1.0)), (object) "delay", (object) (float) (!this.delay.IsNone ? (double) this.delay.Value : 0.0), (object) "easetype", (object) this.easeType, (object) "looptype", (object) this.loopType, (object) "oncomplete", (object) "iTweenOnComplete", (object) "oncompleteparams", (object) this.itweenID, (object) "onstart", (object) "iTweenOnStart", (object) "onstartparams", (object) this.itweenID, (object) "ignoretimescale", (object) (bool) (!this.realTime.IsNone ? (this.realTime.Value ? 1 : 0) : 0), (object) "axis", this.axis != iTweenFsmAction.AxisRestriction.none ? (object) Enum.GetName(typeof (iTweenFsmAction.AxisRestriction), (object) this.axis) : (object) string.Empty));
        }
    }
}
