using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Gets a Game Object's Transform and stores it in an Object Variable.")]
    [ActionCategory(ActionCategory.GameObject)]
    public class GetTransform : FsmStateAction
    {
        [RequiredField]
        public FsmGameObject gameObject;
        [ObjectType(typeof (Transform))]
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmObject storeTransform;
        public bool everyFrame;

        public override void Reset()
        {
            FsmGameObject fsmGameObject = new FsmGameObject();
            fsmGameObject.UseVariable = true;
            this.gameObject = fsmGameObject;
            this.storeTransform = (FsmObject) null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoGetGameObjectName();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoGetGameObjectName();

        private void DoGetGameObjectName()
        {
            GameObject gameObject = this.gameObject.Value;
            this.storeTransform.Value = !((Object) gameObject != (Object) null) ? (Object) null : (Object) gameObject.transform;
        }
    }
}
