using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Finds the Child of a GameObject by Name.\nNote, you can specify a path to the child, e.g., LeftShoulder/Arm/Hand/Finger. If you need to specify a tag, use GetChild.")]
    [ActionCategory(ActionCategory.GameObject)]
    public class FindChild : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The GameObject to search.")]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The name of the child. Note, you can specify a path to the child, e.g., LeftShoulder/Arm/Hand/Finger")]
        [RequiredField]
        public FsmString childName;
        [HutongGames.PlayMaker.Tooltip("Store the child in a GameObject variable.")]
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmGameObject storeResult;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.childName = (FsmString) string.Empty;
            this.storeResult = (FsmGameObject) null;
        }

        public override void OnEnter()
        {
            this.DoFindChild();
            this.Finish();
        }

        private void DoFindChild()
        {
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if ((Object) ownerDefaultTarget == (Object) null)
                return;
            Transform transform = ownerDefaultTarget.transform.Find(this.childName.Value);
            this.storeResult.Value = !((Object) transform != (Object) null) ? (GameObject) null : transform.gameObject;
        }
    }
}
