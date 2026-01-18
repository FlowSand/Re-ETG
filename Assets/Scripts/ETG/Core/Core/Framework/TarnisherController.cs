using System;

using UnityEngine;

#nullable disable

public class TarnisherController : BraveBehaviour
    {
        public void Awake() => this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnPreDeath);

        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.healthHaver.OnPreDeath -= new Action<Vector2>(this.OnPreDeath);
        }

        private void OnPreDeath(Vector2 vector2)
        {
            this.aiAnimator.OtherAnimations.Find((Predicate<AIAnimator.NamedDirectionalAnimation>) (a => a.name == "pitfall")).anim.Prefix = "pitfall_dead";
        }
    }

