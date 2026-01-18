using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(".NPCs")]
    [HutongGames.PlayMaker.Tooltip("Sets the NPC's visibility (renderers and Speculative Rigidbody).")]
    public class SetNpcVisibility : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("Set visibility to this.")]
        public FsmBool visible;

        public override void Reset() => this.visible = (FsmBool) true;

        public override void OnEnter()
        {
            TalkDoerLite component = this.Owner.GetComponent<TalkDoerLite>();
            if ((bool) (Object) component)
                SetNpcVisibility.SetVisible(component, this.visible.Value);
            this.Finish();
        }

        public static void SetVisible(TalkDoerLite talkDoer, bool visible)
        {
            talkDoer.renderer.enabled = visible;
            talkDoer.ShowOutlines = visible;
            if ((bool) (Object) talkDoer.shadow)
                talkDoer.shadow.GetComponent<Renderer>().enabled = visible;
            if ((bool) (Object) talkDoer.specRigidbody)
                talkDoer.specRigidbody.enabled = visible;
            if (!(bool) (Object) talkDoer.ultraFortunesFavor)
                return;
            talkDoer.ultraFortunesFavor.enabled = visible;
        }
    }
}
