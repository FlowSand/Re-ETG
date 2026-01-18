using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GameObject)]
    [HutongGames.PlayMaker.Tooltip("Gets a Random Child of a Game Object.")]
    public class GetRandomChild : FsmStateAction
    {
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmGameObject storeResult;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.storeResult = (FsmGameObject) null;
        }

        public override void OnEnter()
        {
            this.DoGetRandomChild();
            this.Finish();
        }

        private void DoGetRandomChild()
        {
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if ((Object) ownerDefaultTarget == (Object) null)
                return;
            int childCount = ownerDefaultTarget.transform.childCount;
            if (childCount == 0)
                return;
            this.storeResult.Value = ownerDefaultTarget.transform.GetChild(Random.Range(0, childCount)).gameObject;
        }
    }
}
