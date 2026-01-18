using System;
using System.Collections.Generic;

using UnityEngine;

#nullable disable

public class PressurePlate : BraveBehaviour
    {
        public bool PlayersCanTrigger = true;
        public bool EnemiesCanTrigger;
        public bool ArbitraryObjectsCanTrigger;
        public bool CanUnpress = true;
        public string depressAnimationName = string.Empty;
        public string unpressAnimationName = string.Empty;
        public Action<PressurePlate> OnPressurePlateDepressed;
        public Action<PressurePlate> OnPressurePlateUnpressed;
        private HashSet<GameObject> m_currentDepressors;
        private List<PlayerController> m_queuedDepressors = new List<PlayerController>();
        private bool m_pressed;

        private void Start()
        {
            this.m_currentDepressors = new HashSet<GameObject>();
            this.specRigidbody.OnEnterTrigger += new SpeculativeRigidbody.OnTriggerDelegate(this.HandleEnterTriggerCollision);
            this.specRigidbody.OnExitTrigger += new SpeculativeRigidbody.OnTriggerExitDelegate(this.HandleExitTriggerCollision);
        }

        private void HandleEnterTriggerCollision(
            SpeculativeRigidbody specRigidbody,
            SpeculativeRigidbody sourceSpecRigidbody,
            CollisionData collisionData)
        {
            int count1 = this.m_currentDepressors.Count;
            if (this.PlayersCanTrigger)
            {
                PlayerController component = specRigidbody.GetComponent<PlayerController>();
                if ((UnityEngine.Object) component != (UnityEngine.Object) null)
                {
                    if (component.IsDodgeRolling && !component.IsGrounded && !component.IsFlying && GameManager.Instance.IsFoyer)
                    {
                        this.m_queuedDepressors.Add(component);
                        return;
                    }
                    this.m_currentDepressors.Add(specRigidbody.gameObject);
                    if (!this.m_pressed)
                    {
                        int num = (int) AkSoundEngine.PostEvent("Play_OBJ_plate_press_01", this.gameObject);
                    }
                }
            }
            if (this.EnemiesCanTrigger && (UnityEngine.Object) specRigidbody.GetComponent<AIActor>() != (UnityEngine.Object) null)
                this.m_currentDepressors.Add(specRigidbody.gameObject);
            if (this.ArbitraryObjectsCanTrigger)
                this.m_currentDepressors.Add(specRigidbody.gameObject);
            int count2 = this.m_currentDepressors.Count;
            this.m_pressed = true;
            if (count1 != 0 || count2 <= 0)
                return;
            if (!string.IsNullOrEmpty(this.depressAnimationName))
                this.spriteAnimator.Play(this.depressAnimationName);
            if (this.OnPressurePlateDepressed == null)
                return;
            this.OnPressurePlateDepressed(this);
        }

        private void Update()
        {
            if (this.m_queuedDepressors.Count <= 0)
                return;
            for (int index = 0; index < this.m_queuedDepressors.Count; ++index)
            {
                if (!this.m_queuedDepressors[index].IsDodgeRolling || this.m_queuedDepressors[index].IsGrounded)
                {
                    this.HandleEnterTriggerCollision(this.m_queuedDepressors[index].specRigidbody, this.specRigidbody, (CollisionData) null);
                    this.m_queuedDepressors.RemoveAt(index);
                    --index;
                }
            }
        }

        private void HandleExitTriggerCollision(
            SpeculativeRigidbody specRigidbody,
            SpeculativeRigidbody sourceSpecRigidbody)
        {
            if (!this.CanUnpress)
                return;
            PlayerController component = specRigidbody.GetComponent<PlayerController>();
            if ((bool) (UnityEngine.Object) component && this.m_queuedDepressors.Contains(component))
            {
                this.m_queuedDepressors.Remove(component);
            }
            else
            {
                int count1 = this.m_currentDepressors.Count;
                if (this.m_currentDepressors.Contains(specRigidbody.gameObject))
                    this.m_currentDepressors.Remove(specRigidbody.gameObject);
                int count2 = this.m_currentDepressors.Count;
                if (count1 <= 0 || count2 != 0)
                    return;
                this.m_pressed = false;
                if (!string.IsNullOrEmpty(this.unpressAnimationName))
                    this.spriteAnimator.Play(this.unpressAnimationName);
                if (this.OnPressurePlateUnpressed == null)
                    return;
                this.OnPressurePlateUnpressed(this);
            }
        }

        protected override void OnDestroy() => base.OnDestroy();
    }

