using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Makes the NPC a ghost.")]
    [ActionCategory(".NPCs")]
    public class BecomeGhost : FsmStateAction
    {
        public override void Reset()
        {
        }

        public override string ErrorCheck() => string.Empty;

        public override void OnEnter()
        {
            if ((bool) (Object) this.Owner && (bool) (Object) this.Owner.GetComponent<TalkDoerLite>())
                this.Owner.GetComponent<TalkDoerLite>().ConvertToGhost();
            this.Finish();
        }
    }
}
