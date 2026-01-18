using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Camera)]
    [HutongGames.PlayMaker.Tooltip("Gets the GameObject tagged MainCamera from the scene")]
    [ActionTarget(typeof (Camera), "storeGameObject", false)]
    public class GetMainCamera : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmGameObject storeGameObject;

        public override void Reset() => this.storeGameObject = (FsmGameObject) null;

        public override void OnEnter()
        {
            this.storeGameObject.Value = !((Object) Camera.main != (Object) null) ? (GameObject) null : Camera.main.gameObject;
            this.Finish();
        }
    }
}
