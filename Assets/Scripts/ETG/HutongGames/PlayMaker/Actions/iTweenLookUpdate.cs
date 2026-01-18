using System;
using System.Collections;

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Rotates a GameObject to look at a supplied Transform or Vector3 over time.")]
    [ActionCategory("iTween")]
    public class iTweenLookUpdate : FsmStateAction
    {
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Look at a transform position.")]
        public FsmGameObject transformTarget;
        [HutongGames.PlayMaker.Tooltip("A target position the GameObject will look at. If Transform Target is defined this is used as a look offset.")]
        public FsmVector3 vectorTarget;
        [HutongGames.PlayMaker.Tooltip("The time in seconds the animation will take to complete.")]
        public FsmFloat time;
        [HutongGames.PlayMaker.Tooltip("Restricts rotation to the supplied axis only. Just put there strinc like 'x' or 'xz'")]
        public iTweenFsmAction.AxisRestriction axis;
        private Hashtable hash;
        private GameObject go;

        public override void Reset()
        {
            FsmGameObject fsmGameObject = new FsmGameObject();
            fsmGameObject.UseVariable = true;
            this.transformTarget = fsmGameObject;
            FsmVector3 fsmVector3 = new FsmVector3();
            fsmVector3.UseVariable = true;
            this.vectorTarget = fsmVector3;
            this.time = (FsmFloat) 1f;
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
                if (this.transformTarget.IsNone)
                    this.hash.Add((object) "looktarget", (object) (!this.vectorTarget.IsNone ? this.vectorTarget.Value : Vector3.zero));
                else if (this.vectorTarget.IsNone)
                    this.hash.Add((object) "looktarget", (object) this.transformTarget.Value.transform);
                else
                    this.hash.Add((object) "looktarget", (object) (this.transformTarget.Value.transform.position + this.vectorTarget.Value));
                this.hash.Add((object) "time", (object) (float) (!this.time.IsNone ? (double) this.time.Value : 1.0));
                this.hash.Add((object) "axis", this.axis != iTweenFsmAction.AxisRestriction.none ? (object) Enum.GetName(typeof (iTweenFsmAction.AxisRestriction), (object) this.axis) : (object) string.Empty);
                this.DoiTween();
            }
        }

        public override void OnExit()
        {
        }

        public override void OnUpdate()
        {
            this.hash.Remove((object) "looktarget");
            if (this.transformTarget.IsNone)
                this.hash.Add((object) "looktarget", (object) (!this.vectorTarget.IsNone ? this.vectorTarget.Value : Vector3.zero));
            else if (this.vectorTarget.IsNone)
                this.hash.Add((object) "looktarget", (object) this.transformTarget.Value.transform);
            else
                this.hash.Add((object) "looktarget", (object) (this.transformTarget.Value.transform.position + this.vectorTarget.Value));
            this.DoiTween();
        }

        private void DoiTween() => iTween.LookUpdate(this.go, this.hash);
    }
}
