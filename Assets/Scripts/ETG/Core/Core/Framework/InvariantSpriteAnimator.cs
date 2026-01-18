using UnityEngine;

#nullable disable

[RequireComponent(typeof (tk2dSpriteAnimator))]
public class InvariantSpriteAnimator : BraveBehaviour
    {
        public void Awake() => this.spriteAnimator.ignoreTimeScale = true;
    }

