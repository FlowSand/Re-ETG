using System.Collections.Generic;

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(".Brave")]
    [HutongGames.PlayMaker.Tooltip("Responds to trigger events with Speculative Rigidbodies.")]
    public class SpecTriggerEvent : FsmStateAction
    {
        [CompoundArray("Events", "Trigger Index", "Send Event")]
        [HutongGames.PlayMaker.Tooltip("Event to play when the corresponding trigger detects a collision.")]
        public FsmInt[] triggerIndices;
        public FsmEvent[] events;
        private SpeculativeRigidbody m_specRigidbody;
        private List<PixelCollider> m_triggerColliders = new List<PixelCollider>();

        public override void Reset()
        {
            this.triggerIndices = new FsmInt[0];
            this.events = new FsmEvent[0];
        }

        public override string ErrorCheck()
        {
            string empty = string.Empty;
            SpeculativeRigidbody component = this.Owner.GetComponent<SpeculativeRigidbody>();
            if (!(bool) (Object) component)
            {
                empty += "Owner does not have a Speculative Rigidbody.\n";
            }
            else
            {
                int num = 0;
                for (int index = 0; index < component.PixelColliders.Count; ++index)
                {
                    if (component.PixelColliders[index].IsTrigger)
                        ++num;
                }
                for (int index = 0; index < this.triggerIndices.Length; ++index)
                {
                    if (this.triggerIndices[index].Value >= num)
                        empty += $"Trigger index {this.triggerIndices[index].Value} is too high for a Speculative Rigidbody with {num} triggers.\n";
                }
            }
            return empty;
        }

        public override void OnEnter()
        {
            this.m_specRigidbody = this.Owner.GetComponent<SpeculativeRigidbody>();
            if (!(bool) (Object) this.m_specRigidbody)
                return;
            for (int index = 0; index < this.m_specRigidbody.PixelColliders.Count; ++index)
            {
                PixelCollider pixelCollider = this.m_specRigidbody.PixelColliders[index];
                if (pixelCollider.IsTrigger)
                    this.m_triggerColliders.Add(pixelCollider);
            }
            this.m_specRigidbody.OnEnterTrigger += new SpeculativeRigidbody.OnTriggerDelegate(this.OnEnterTrigger);
        }

        public override void OnExit()
        {
            if (!(bool) (Object) this.m_specRigidbody)
                return;
            this.m_specRigidbody.OnEnterTrigger -= new SpeculativeRigidbody.OnTriggerDelegate(this.OnEnterTrigger);
        }

        private void OnEnterTrigger(
            SpeculativeRigidbody specRigidbody,
            SpeculativeRigidbody sourceSpecRigidbody,
            CollisionData collisionData)
        {
            for (int index = 0; index < this.triggerIndices.Length; ++index)
            {
                if (collisionData.MyPixelCollider == this.m_triggerColliders[this.triggerIndices[index].Value])
                    this.Fsm.Event(this.events[index]);
            }
        }
    }
}
