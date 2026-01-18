using System;

using UnityEngine;

#nullable disable

public class BossDoorMimicDeathController : BraveBehaviour
    {
        public void Start() => this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnBossDeath);

        protected override void OnDestroy() => base.OnDestroy();

        private void OnBossDeath(Vector2 dir)
        {
            BossDoorMimicIntroDoer component = this.GetComponent<BossDoorMimicIntroDoer>();
            if (!(bool) (UnityEngine.Object) component)
                return;
            component.PhantomDoorBlocker.Unseal();
        }
    }

