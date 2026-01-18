using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Plays a Random Audio Clip at a position defined by a Game Object or a Vector3. If a position is defined, it takes priority over the game object. You can set the relative weight of the clips to control how often they are selected.")]
    [ActionCategory(ActionCategory.Audio)]
    public class PlayRandomSound : FsmStateAction
    {
        public FsmOwnerDefault gameObject;
        public FsmVector3 position;
        [CompoundArray("Audio Clips", "Audio Clip", "Weight")]
        public AudioClip[] audioClips;
        [HasFloatSlider(0.0f, 1f)]
        public FsmFloat[] weights;
        [HasFloatSlider(0.0f, 1f)]
        public FsmFloat volume = (FsmFloat) 1f;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            FsmVector3 fsmVector3 = new FsmVector3();
            fsmVector3.UseVariable = true;
            this.position = fsmVector3;
            this.audioClips = new AudioClip[3];
            this.weights = new FsmFloat[3]
            {
                (FsmFloat) 1f,
                (FsmFloat) 1f,
                (FsmFloat) 1f
            };
            this.volume = (FsmFloat) 1f;
        }

        public override void OnEnter()
        {
            this.DoPlayRandomClip();
            this.Finish();
        }

        private void DoPlayRandomClip()
        {
            if (this.audioClips.Length == 0)
                return;
            int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
            if (randomWeightedIndex == -1)
                return;
            AudioClip audioClip = this.audioClips[randomWeightedIndex];
            if (!((Object) audioClip != (Object) null))
                return;
            if (!this.position.IsNone)
            {
                AudioSource.PlayClipAtPoint(audioClip, this.position.Value, this.volume.Value);
            }
            else
            {
                GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
                if ((Object) ownerDefaultTarget == (Object) null)
                    return;
                AudioSource.PlayClipAtPoint(audioClip, ownerDefaultTarget.transform.position, this.volume.Value);
            }
        }
    }
}
