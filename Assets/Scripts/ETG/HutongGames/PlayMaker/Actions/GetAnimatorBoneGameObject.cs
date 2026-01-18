using System;

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [HutongGames.PlayMaker.Tooltip("Gets the GameObject mapped to this human bone id")]
    public class GetAnimatorBoneGameObject : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
        [CheckForComponent(typeof (Animator))]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [ObjectType(typeof (HumanBodyBones))]
        [HutongGames.PlayMaker.Tooltip("The bone reference")]
        public FsmEnum bone;
        [HutongGames.PlayMaker.Tooltip("The Bone's GameObject")]
        [UIHint(UIHint.Variable)]
        [ActionSection("Results")]
        public FsmGameObject boneGameObject;
        private Animator _animator;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.bone = (FsmEnum) (Enum) HumanBodyBones.Hips;
            this.boneGameObject = (FsmGameObject) null;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if ((UnityEngine.Object) ownerDefaultTarget == (UnityEngine.Object) null)
            {
                this.Finish();
            }
            else
            {
                this._animator = ownerDefaultTarget.GetComponent<Animator>();
                if ((UnityEngine.Object) this._animator == (UnityEngine.Object) null)
                {
                    this.Finish();
                }
                else
                {
                    this.GetBoneTransform();
                    this.Finish();
                }
            }
        }

        private void GetBoneTransform()
        {
            this.boneGameObject.Value = this._animator.GetBoneTransform((HumanBodyBones) this.bone.Value).gameObject;
        }
    }
}
