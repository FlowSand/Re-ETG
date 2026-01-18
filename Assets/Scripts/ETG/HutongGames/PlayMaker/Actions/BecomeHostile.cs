using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Makes this NPC become an enemy.")]
    [ActionCategory(".NPCs")]
    public class BecomeHostile : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The enemy prefab to spawn.")]
        public FsmString enemyGuid;
        [HutongGames.PlayMaker.Tooltip("Optionally, a different TalkDoerLite to become hostile. Used for controlling groups.")]
        public TalkDoerLite alternativeTarget;

        public override void Reset() => this.enemyGuid = (FsmString) null;

        public override void OnEnter()
        {
            TalkDoerLite talkDoer = this.Owner.GetComponent<TalkDoerLite>();
            if ((Object) this.alternativeTarget != (Object) null)
                talkDoer = this.alternativeTarget;
            SetNpcVisibility.SetVisible(talkDoer, false);
            AIActor aiActor = AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid(this.enemyGuid.Value), talkDoer.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor), talkDoer.ParentRoom);
            aiActor.specRigidbody.Initialize();
            aiActor.transform.position += (Vector3) (talkDoer.specRigidbody.UnitBottomLeft - aiActor.specRigidbody.UnitBottomLeft);
            aiActor.specRigidbody.Reinitialize();
            aiActor.HasBeenEngaged = true;
            if ((Object) this.alternativeTarget == (Object) null)
            {
                GenericIntroDoer component = aiActor.GetComponent<GenericIntroDoer>();
                if ((bool) (Object) component)
                    component.TriggerSequence(talkDoer.TalkingPlayer);
            }
            talkDoer.HostileObject = aiActor;
            this.Finish();
        }
    }
}
