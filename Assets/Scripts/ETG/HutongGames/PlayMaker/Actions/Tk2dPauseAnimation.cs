using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("2D Toolkit/SpriteAnimator")]
    [HutongGames.PlayMaker.Tooltip("Pause a sprite animation. Can work everyframe to pause resume animation on the fly. \nNOTE: The Game Object must have a tk2dSpriteAnimator attached.")]
    [HelpUrl("https://hutonggames.fogbugz.com/default.asp?W720")]
    public class Tk2dPauseAnimation : FsmStateAction
    {
        [CheckForComponent(typeof (tk2dSpriteAnimator))]
        [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Pause flag")]
        public FsmBool pause;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        [ActionSection("")]
        public bool everyframe;
        private tk2dSpriteAnimator _sprite;

        private void _getSprite()
        {
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if ((Object) ownerDefaultTarget == (Object) null)
                return;
            this._sprite = ownerDefaultTarget.GetComponent<tk2dSpriteAnimator>();
        }

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.pause = (FsmBool) true;
            this.everyframe = false;
        }

        public override void OnEnter()
        {
            this._getSprite();
            this.DoPauseAnimation();
            if (this.everyframe)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoPauseAnimation();

        private void DoPauseAnimation()
        {
            if ((Object) this._sprite == (Object) null)
            {
                this.LogWarning("Missing tk2dSpriteAnimator component: " + this._sprite.gameObject.name);
            }
            else
            {
                if (this._sprite.Paused == this.pause.Value)
                    return;
                if (this.pause.Value)
                    this._sprite.Pause();
                else
                    this._sprite.Resume();
            }
        }
    }
}
