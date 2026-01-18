using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GameObject)]
    [HutongGames.PlayMaker.Tooltip("Gets a Game Object's Layer and stores it in an Int Variable.")]
    public class GetLayer : FsmStateAction
    {
        [RequiredField]
        public FsmGameObject gameObject;
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmInt storeResult;
        public bool everyFrame;

        public override void Reset()
        {
            this.gameObject = (FsmGameObject) null;
            this.storeResult = (FsmInt) null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoGetLayer();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoGetLayer();

        private void DoGetLayer()
        {
            if ((Object) this.gameObject.Value == (Object) null)
                return;
            this.storeResult.Value = this.gameObject.Value.layer;
        }
    }
}
