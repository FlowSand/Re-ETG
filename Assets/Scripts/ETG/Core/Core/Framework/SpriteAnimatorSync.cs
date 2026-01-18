using System;

#nullable disable

public class SpriteAnimatorSync : BraveBehaviour
    {
        public tk2dBaseSprite otherSprite;

        public void Start()
        {
            if ((bool) (UnityEngine.Object) this.otherSprite.spriteAnimator)
                this.otherSprite.spriteAnimator.alwaysUpdateOffscreen = true;
            this.otherSprite.SpriteChanged += new Action<tk2dBaseSprite>(this.OtherSpriteChanged);
            this.sprite.SetSprite(this.otherSprite.Collection, this.otherSprite.spriteId);
        }

        protected override void OnDestroy() => base.OnDestroy();

        private void OtherSpriteChanged(tk2dBaseSprite tk2DBaseSprite)
        {
            this.sprite.SetSprite(this.otherSprite.spriteId);
        }
    }

