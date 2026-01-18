using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Audio)]
    [HutongGames.PlayMaker.Tooltip("Sets the global sound volume.")]
    public class SetGameVolume : FsmStateAction
    {
        [RequiredField]
        [HasFloatSlider(0.0f, 1f)]
        public FsmFloat volume;
        public bool everyFrame;

        public override void Reset()
        {
            this.volume = (FsmFloat) 1f;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            AudioListener.volume = this.volume.Value;
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => AudioListener.volume = this.volume.Value;
    }
}
