using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Destroys a Game Object.")]
    [ActionCategory(ActionCategory.GameObject)]
    public class DestroyObject : FsmStateAction
    {
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The GameObject to destroy.")]
        public FsmGameObject gameObject;
        [HutongGames.PlayMaker.Tooltip("Optional delay before destroying the Game Object.")]
        [HasFloatSlider(0.0f, 5f)]
        public FsmFloat delay;
        [HutongGames.PlayMaker.Tooltip("Detach children before destroying the Game Object.")]
        public FsmBool detachChildren;

        public override void Reset()
        {
            this.gameObject = (FsmGameObject) null;
            this.delay = (FsmFloat) 0.0f;
        }

        public override void OnEnter()
        {
            GameObject gameObject = this.gameObject.Value;
            if ((Object) gameObject != (Object) null)
            {
                if ((double) this.delay.Value <= 0.0)
                    Object.Destroy((Object) gameObject);
                else
                    Object.Destroy((Object) gameObject, this.delay.Value);
                if (this.detachChildren.Value)
                    gameObject.transform.DetachChildren();
            }
            this.Finish();
        }

        public override void OnUpdate()
        {
        }
    }
}
