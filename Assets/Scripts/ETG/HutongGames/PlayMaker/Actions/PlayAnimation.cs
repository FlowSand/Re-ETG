using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Plays an Animation on a Game Object. You can add named animation clips to the object in the Unity editor, or with the Add Animation Clip action.")]
    [ActionCategory(ActionCategory.Animation)]
    public class PlayAnimation : BaseAnimationAction
    {
        [HutongGames.PlayMaker.Tooltip("Game Object to play the animation on.")]
        [CheckForComponent(typeof (Animation))]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The name of the animation to play.")]
        [UIHint(UIHint.Animation)]
        public FsmString animName;
        [HutongGames.PlayMaker.Tooltip("How to treat previously playing animations.")]
        public UnityEngine.PlayMode playMode;
        [HasFloatSlider(0.0f, 5f)]
        [HutongGames.PlayMaker.Tooltip("Time taken to blend to this animation.")]
        public FsmFloat blendTime;
        [HutongGames.PlayMaker.Tooltip("Event to send when the animation is finished playing. NOTE: Not sent with Loop or PingPong wrap modes!")]
        public FsmEvent finishEvent;
        [HutongGames.PlayMaker.Tooltip("Event to send when the animation loops. If you want to send this event to another FSM use Set Event Target. NOTE: This event is only sent with Loop and PingPong wrap modes.")]
        public FsmEvent loopEvent;
        [HutongGames.PlayMaker.Tooltip("Stop playing the animation when this state is exited.")]
        public bool stopOnExit;
        private AnimationState anim;
        private float prevAnimtTime;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.animName = (FsmString) null;
            this.playMode = UnityEngine.PlayMode.StopAll;
            this.blendTime = (FsmFloat) 0.3f;
            this.finishEvent = (FsmEvent) null;
            this.loopEvent = (FsmEvent) null;
            this.stopOnExit = false;
        }

        public override void OnEnter() => this.DoPlayAnimation();

        private void DoPlayAnimation()
        {
            if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
                this.Finish();
            else if (string.IsNullOrEmpty(this.animName.Value))
            {
                this.LogWarning("Missing animName!");
                this.Finish();
            }
            else
            {
                this.anim = this.animation[this.animName.Value];
                if ((TrackedReference) this.anim == (TrackedReference) null)
                {
                    this.LogWarning("Missing animation: " + this.animName.Value);
                    this.Finish();
                }
                else
                {
                    float fadeLength = this.blendTime.Value;
                    if ((double) fadeLength < 1.0 / 1000.0)
                        this.animation.Play(this.animName.Value, this.playMode);
                    else
                        this.animation.CrossFade(this.animName.Value, fadeLength, this.playMode);
                    this.prevAnimtTime = this.anim.time;
                }
            }
        }

        public override void OnUpdate()
        {
            if ((Object) this.Fsm.GetOwnerDefaultTarget(this.gameObject) == (Object) null || (TrackedReference) this.anim == (TrackedReference) null)
                return;
            if (!this.anim.enabled || this.anim.wrapMode == WrapMode.ClampForever && (double) this.anim.time > (double) this.anim.length)
            {
                this.Fsm.Event(this.finishEvent);
                this.Finish();
            }
            if (this.anim.wrapMode == WrapMode.ClampForever || (double) this.anim.time <= (double) this.anim.length || (double) this.prevAnimtTime >= (double) this.anim.length)
                return;
            this.Fsm.Event(this.loopEvent);
        }

        public override void OnExit()
        {
            if (!this.stopOnExit)
                return;
            this.StopAnimation();
        }

        private void StopAnimation()
        {
            if (!((Object) this.animation != (Object) null))
                return;
            this.animation.Stop(this.animName.Value);
        }
    }
}
