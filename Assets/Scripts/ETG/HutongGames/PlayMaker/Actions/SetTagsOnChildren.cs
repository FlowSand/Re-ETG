using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Set the Tag on all children of a GameObject. Optionally filter by component.")]
    [ActionCategory(ActionCategory.GameObject)]
    public class SetTagsOnChildren : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("GameObject Parent")]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.Tag)]
        [HutongGames.PlayMaker.Tooltip("Set Tag To...")]
        [RequiredField]
        public FsmString tag;
        [HutongGames.PlayMaker.Tooltip("Only set the Tag on children with this component.")]
        [UIHint(UIHint.ScriptComponent)]
        public FsmString filterByComponent;
        private System.Type componentFilter;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.tag = (FsmString) null;
            this.filterByComponent = (FsmString) null;
        }

        public override void OnEnter()
        {
            this.SetTag(this.Fsm.GetOwnerDefaultTarget(this.gameObject));
            this.Finish();
        }

        private void SetTag(GameObject parent)
        {
            if ((UnityEngine.Object) parent == (UnityEngine.Object) null)
                return;
            if (string.IsNullOrEmpty(this.filterByComponent.Value))
            {
                foreach (Component component in parent.transform)
                    component.gameObject.tag = this.tag.Value;
            }
            else
            {
                this.UpdateComponentFilter();
                if (this.componentFilter != null)
                {
                    foreach (Component componentsInChild in parent.GetComponentsInChildren(this.componentFilter))
                        componentsInChild.gameObject.tag = this.tag.Value;
                }
            }
            this.Finish();
        }

        private void UpdateComponentFilter()
        {
            this.componentFilter = ReflectionUtils.GetGlobalType(this.filterByComponent.Value);
            if (this.componentFilter == null)
                this.componentFilter = ReflectionUtils.GetGlobalType("UnityEngine." + this.filterByComponent.Value);
            if (this.componentFilter != null)
                return;
            Debug.LogWarning((object) ("Couldn't get type: " + this.filterByComponent.Value));
        }
    }
}
