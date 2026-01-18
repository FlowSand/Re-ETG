using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Destroys GameObjects in an array.")]
    [ActionCategory(ActionCategory.GameObject)]
    public class DestroyObjects : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The GameObjects to destroy.")]
        [ArrayEditor(VariableType.GameObject, "", 0, 0, 65536 /*0x010000*/)]
        [RequiredField]
        public FsmArray gameObjects;
        [HutongGames.PlayMaker.Tooltip("Optional delay before destroying the Game Objects.")]
        [HasFloatSlider(0.0f, 5f)]
        public FsmFloat delay;
        [HutongGames.PlayMaker.Tooltip("Detach children before destroying the Game Objects.")]
        public FsmBool detachChildren;

        public override void Reset()
        {
            this.gameObjects = (FsmArray) null;
            this.delay = (FsmFloat) 0.0f;
        }

        public override void OnEnter()
        {
            if (this.gameObjects.Values != null)
            {
                foreach (GameObject gameObject in this.gameObjects.Values)
                {
                    if ((Object) gameObject != (Object) null)
                    {
                        if ((double) this.delay.Value <= 0.0)
                            Object.Destroy((Object) gameObject);
                        else
                            Object.Destroy((Object) gameObject, this.delay.Value);
                        if (this.detachChildren.Value)
                            gameObject.transform.DetachChildren();
                    }
                }
            }
            this.Finish();
        }
    }
}
