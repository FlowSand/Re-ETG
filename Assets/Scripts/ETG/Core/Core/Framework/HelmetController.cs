using System;

using UnityEngine;

#nullable disable

public class HelmetController : BraveBehaviour
    {
        public GameObject helmetEffect;
        public float helmetForce = 5f;

        public void Start() => this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnPreDeath);

        protected override void OnDestroy()
        {
            this.healthHaver.OnPreDeath -= new Action<Vector2>(this.OnPreDeath);
            base.OnDestroy();
        }

        public void OnPreDeath(Vector2 finalDamageDirection)
        {
            if (this.aiActor.IsFalling || !((UnityEngine.Object) this.helmetEffect != (UnityEngine.Object) null))
                return;
            DebrisObject component = SpawnManager.SpawnDebris(this.helmetEffect, (Vector3) this.specRigidbody.UnitTopLeft, Quaternion.identity).GetComponent<DebrisObject>();
            if (!(bool) (UnityEngine.Object) component)
                return;
            component.Trigger((Vector3) (finalDamageDirection.normalized * this.helmetForce), 1f);
        }
    }

