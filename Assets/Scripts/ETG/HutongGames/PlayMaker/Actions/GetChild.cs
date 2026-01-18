using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Finds the Child of a GameObject by Name and/or Tag. Use this to find attach points etc. NOTE: This action will search recursively through all children and return the first match; To find a specific child use Find Child.")]
    [ActionCategory(ActionCategory.GameObject)]
    public class GetChild : FsmStateAction
    {
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The GameObject to search.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The name of the child to search for.")]
        public FsmString childName;
        [UIHint(UIHint.Tag)]
        [HutongGames.PlayMaker.Tooltip("The Tag to search for. If Child Name is set, both name and Tag need to match.")]
        public FsmString withTag;
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [HutongGames.PlayMaker.Tooltip("Store the result in a GameObject variable.")]
        public FsmGameObject storeResult;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.childName = (FsmString) string.Empty;
            this.withTag = (FsmString) "Untagged";
            this.storeResult = (FsmGameObject) null;
        }

        public override void OnEnter()
        {
            this.storeResult.Value = GetChild.DoGetChildByName(this.Fsm.GetOwnerDefaultTarget(this.gameObject), this.childName.Value, this.withTag.Value);
            this.Finish();
        }

        private static GameObject DoGetChildByName(GameObject root, string name, string tag)
        {
            if ((Object) root == (Object) null)
                return (GameObject) null;
            foreach (Transform transform in root.transform)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    if (transform.name == name)
                    {
                        if (string.IsNullOrEmpty(tag))
                            return transform.gameObject;
                        if (transform.tag.Equals(tag))
                            return transform.gameObject;
                    }
                }
                else if (!string.IsNullOrEmpty(tag) && transform.tag == tag)
                    return transform.gameObject;
                GameObject childByName = GetChild.DoGetChildByName(transform.gameObject, name, tag);
                if ((Object) childByName != (Object) null)
                    return childByName;
            }
            return (GameObject) null;
        }

        public override string ErrorCheck()
        {
            return string.IsNullOrEmpty(this.childName.Value) && string.IsNullOrEmpty(this.withTag.Value) ? "Specify Child Name, Tag, or both." : (string) null;
        }
    }
}
