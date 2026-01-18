using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GameObject)]
    [HutongGames.PlayMaker.Tooltip("Gets a Game Object's Tag and stores it in a String Variable.")]
    public class GetTag : FsmStateAction
    {
        [RequiredField]
        public FsmGameObject gameObject;
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmString storeResult;
        public bool everyFrame;

        public override void Reset()
        {
            this.gameObject = (FsmGameObject) null;
            this.storeResult = (FsmString) null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoGetTag();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoGetTag();

        private void DoGetTag()
        {
            if ((Object) this.gameObject.Value == (Object) null)
                return;
            this.storeResult.Value = this.gameObject.Value.tag;
        }
    }
}
