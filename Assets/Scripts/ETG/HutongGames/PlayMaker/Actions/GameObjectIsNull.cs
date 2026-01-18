using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Tests if a GameObject Variable has a null value. E.g., If the FindGameObject action failed to find an object.")]
    [ActionCategory(ActionCategory.Logic)]
    public class GameObjectIsNull : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The GameObject variable to test.")]
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmGameObject gameObject;
        [HutongGames.PlayMaker.Tooltip("Event to send if the GamObject is null.")]
        public FsmEvent isNull;
        [HutongGames.PlayMaker.Tooltip("Event to send if the GamObject is NOT null.")]
        public FsmEvent isNotNull;
        [HutongGames.PlayMaker.Tooltip("Store the result in a bool variable.")]
        [UIHint(UIHint.Variable)]
        public FsmBool storeResult;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.gameObject = (FsmGameObject) null;
            this.isNull = (FsmEvent) null;
            this.isNotNull = (FsmEvent) null;
            this.storeResult = (FsmBool) null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoIsGameObjectNull();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoIsGameObjectNull();

        private void DoIsGameObjectNull()
        {
            bool flag = (Object) this.gameObject.Value == (Object) null;
            if (this.storeResult != null)
                this.storeResult.Value = flag;
            this.Fsm.Event(!flag ? this.isNotNull : this.isNull);
        }
    }
}
