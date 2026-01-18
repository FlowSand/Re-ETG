using System;

using UnityEngine;

#nullable disable

public class BulletBroDeathController : BraveBehaviour
    {
        private void Start() => this.healthHaver.OnDeath += new Action<Vector2>(this.OnDeath);

        protected override void OnDestroy() => base.OnDestroy();

        private void OnDeath(Vector2 finalDeathDir)
        {
            BroController otherBro = BroController.GetOtherBro(this.gameObject);
            if ((bool) (UnityEngine.Object) otherBro)
                otherBro.Enrage();
            else
                GameStatsManager.Instance.SetFlag(GungeonFlags.BOSSKILLED_BULLET_BROS, true);
        }
    }

