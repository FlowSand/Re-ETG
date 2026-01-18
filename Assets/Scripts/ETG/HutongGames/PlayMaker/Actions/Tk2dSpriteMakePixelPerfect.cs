using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Make a sprite pixelPerfect. \nNOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite)")]
    [ActionCategory("2D Toolkit/Sprite")]
    public class Tk2dSpriteMakePixelPerfect : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof (tk2dBaseSprite))]
        [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite)")]
        public FsmOwnerDefault gameObject;
        private tk2dBaseSprite _sprite;

        private void _getSprite()
        {
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if ((Object) ownerDefaultTarget == (Object) null)
                return;
            this._sprite = ownerDefaultTarget.GetComponent<tk2dBaseSprite>();
        }

        public override void Reset() => this.gameObject = (FsmOwnerDefault) null;

        public override void OnEnter()
        {
            this._getSprite();
            this.MakePixelPerfect();
            this.Finish();
        }

        private void MakePixelPerfect()
        {
            if ((Object) this._sprite == (Object) null)
                this.LogWarning("Missing tk2dBaseSprite component: ");
            else
                this._sprite.MakePixelPerfect();
        }
    }
}
