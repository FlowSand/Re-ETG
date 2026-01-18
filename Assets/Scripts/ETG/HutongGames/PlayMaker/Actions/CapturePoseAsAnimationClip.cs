using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animation)]
    [HutongGames.PlayMaker.Tooltip("Captures the current pose of a hierarchy as an animation clip.\n\nUseful to blend from an arbitrary pose (e.g. a ragdoll death) back to a known animation (e.g. idle).")]
    public class CapturePoseAsAnimationClip : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The GameObject root of the hierarchy to capture.")]
        [CheckForComponent(typeof (Animation))]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Capture position keys.")]
        public FsmBool position;
        [HutongGames.PlayMaker.Tooltip("Capture rotation keys.")]
        public FsmBool rotation;
        [HutongGames.PlayMaker.Tooltip("Capture scale keys.")]
        public FsmBool scale;
        [ObjectType(typeof (AnimationClip))]
        [HutongGames.PlayMaker.Tooltip("Store the result in an Object variable of type AnimationClip.")]
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmObject storeAnimationClip;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.position = (FsmBool) false;
            this.rotation = (FsmBool) true;
            this.scale = (FsmBool) false;
            this.storeAnimationClip = (FsmObject) null;
        }

        public override void OnEnter()
        {
            this.DoCaptureAnimationClip();
            this.Finish();
        }

        private void DoCaptureAnimationClip()
        {
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if ((Object) ownerDefaultTarget == (Object) null)
                return;
            AnimationClip clip = new AnimationClip();
            foreach (Transform transform in ownerDefaultTarget.transform)
                this.CaptureTransform(transform, string.Empty, clip);
            this.storeAnimationClip.Value = (Object) clip;
        }

        private void CaptureTransform(Transform transform, string path, AnimationClip clip)
        {
            path += transform.name;
            if (this.position.Value)
                this.CapturePosition(transform, path, clip);
            if (this.rotation.Value)
                this.CaptureRotation(transform, path, clip);
            if (this.scale.Value)
                this.CaptureScale(transform, path, clip);
            foreach (Transform transform1 in transform)
                this.CaptureTransform(transform1, path + "/", clip);
        }

        private void CapturePosition(Transform transform, string path, AnimationClip clip)
        {
            this.SetConstantCurve(clip, path, "localPosition.x", transform.localPosition.x);
            this.SetConstantCurve(clip, path, "localPosition.y", transform.localPosition.y);
            this.SetConstantCurve(clip, path, "localPosition.z", transform.localPosition.z);
        }

        private void CaptureRotation(Transform transform, string path, AnimationClip clip)
        {
            this.SetConstantCurve(clip, path, "localRotation.x", transform.localRotation.x);
            this.SetConstantCurve(clip, path, "localRotation.y", transform.localRotation.y);
            this.SetConstantCurve(clip, path, "localRotation.z", transform.localRotation.z);
            this.SetConstantCurve(clip, path, "localRotation.w", transform.localRotation.w);
        }

        private void CaptureScale(Transform transform, string path, AnimationClip clip)
        {
            this.SetConstantCurve(clip, path, "localScale.x", transform.localScale.x);
            this.SetConstantCurve(clip, path, "localScale.y", transform.localScale.y);
            this.SetConstantCurve(clip, path, "localScale.z", transform.localScale.z);
        }

        private void SetConstantCurve(
            AnimationClip clip,
            string childPath,
            string propertyPath,
            float value)
        {
            AnimationCurve curve = AnimationCurve.Linear(0.0f, value, 100f, value);
            curve.postWrapMode = WrapMode.Loop;
            clip.SetCurve(childPath, typeof (Transform), propertyPath, curve);
        }
    }
}
