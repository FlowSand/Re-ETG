using System.Collections;
using System.Diagnostics;

using UnityEngine;

using Dungeonator;

#nullable disable

public class AlarmMushroom : DungeonPlaceableBehaviour, IPlaceConfigurable
    {
        public GameObject TriggerVFX;
        public GameObject DestroyVFX;
        private bool m_triggered;
        private RoomHandler m_room;

        private void Start()
        {
            this.specRigidbody.OnTriggerCollision += new SpeculativeRigidbody.OnTriggerDelegate(this.HandleTriggerCollision);
        }

        private void HandleTriggerCollision(
            SpeculativeRigidbody specRigidbody,
            SpeculativeRigidbody sourceSpecRigidbody,
            CollisionData collisionData)
        {
            if (this.m_triggered || !(bool) (Object) specRigidbody.GetComponent<PlayerController>())
                return;
            this.StartCoroutine(this.Trigger());
        }

        [DebuggerHidden]
        private IEnumerator Trigger()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new AlarmMushroom__Triggerc__Iterator0()
            {
                _this = this
            };
        }

        private void DestroyMushroom()
        {
            if ((bool) (Object) this.DestroyVFX)
                SpawnManager.SpawnVFX(this.DestroyVFX, (Vector3) this.specRigidbody.UnitCenter, Quaternion.identity);
            this.spriteAnimator.PlayAndDestroyObject("alarm_mushroom_break");
        }

        protected override void OnDestroy() => base.OnDestroy();

        public void ConfigureOnPlacement(RoomHandler room) => this.m_room = room;
    }

