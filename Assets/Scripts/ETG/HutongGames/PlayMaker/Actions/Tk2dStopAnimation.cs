using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("2D Toolkit/SpriteAnimator")]
    [HutongGames.PlayMaker.Tooltip("Stops a sprite animation. \nNOTE: The Game Object must have a tk2dSpriteAnimator attached.")]
    public class Tk2dStopAnimation : FsmStateAction
    {
        [CheckForComponent(typeof (tk2dSpriteAnimator))]
        [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        private tk2dSpriteAnimator _sprite;

        private void _getSprite()
        {
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if ((Object) ownerDefaultTarget == (Object) null)
                return;
            this._sprite = ownerDefaultTarget.GetComponent<tk2dSpriteAnimator>();
        }

        public override void Reset() => this.gameObject = (FsmOwnerDefault) null;

        public override void OnEnter()
        {
            this._getSprite();
            this.DoStopAnimation();
            this.Finish();
        }

        private void DoStopAnimation()
        {
            if ((Object) this._sprite == (Object) null)
                this.LogWarning("Missing tk2dSpriteAnimator component: " + this._sprite.gameObject.name);
            else
                this._sprite.Stop();
        }
    }
}
