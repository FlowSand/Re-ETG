using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

#nullable disable

public class GatlingGullOutroDoer : BraveBehaviour
    {
        public GameObject CrowVFX;
        protected List<GatlingGullCrowController> m_extantCrows;

        private void Start() => BossKillCam.hackGatlingGullOutroDoer = this;

        public void TriggerSequence()
        {
            int num1 = (int) AkSoundEngine.PostEvent("Play_ANM_Gull_Outro_01", this.gameObject);
            this.sprite.usesOverrideMaterial = true;
            this.sprite.IsPerpendicular = false;
            this.m_extantCrows = new List<GatlingGullCrowController>();
            int num2 = 100;
            for (int index = 0; index < num2; ++index)
            {
                float num3 = (float) Random.Range(30, 40);
                if ((double) Random.value < 0.039999999105930328)
                    num3 = (float) Random.Range(20, 30);
                if (index == 0)
                    num3 = 10f;
                Vector2 vector2_1 = Random.insideUnitCircle.normalized * num3;
                Vector2 vector = this.transform.position.XY() + vector2_1;
                Vector2 vector2_2 = this.transform.position.XY() + Vector2.Scale(Random.insideUnitCircle, new Vector2(2.25f, 1.75f)) + new Vector2(3.1f, 2.1f);
                GatlingGullCrowController component = SpawnManager.SpawnVFX(this.CrowVFX, vector.ToVector3ZUp(vector.y), Quaternion.identity, true).GetComponent<GatlingGullCrowController>();
                component.CurrentTargetPosition = vector2_2;
                component.sprite.SortingOrder = 3;
                component.sprite.HeightOffGround = 20f;
                component.CurrentTargetHeight = 2f;
                component.useFacePoint = true;
                component.facePoint = this.transform.position.XY() + new Vector2(3.1f, 2.1f);
                this.m_extantCrows.Add(component);
            }
            this.StartCoroutine(this.HandleSequence());
        }

        [DebuggerHidden]
        private IEnumerator HandleSequence()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new GatlingGullOutroDoer__HandleSequencec__Iterator0()
            {
                _this = this
            };
        }

        protected override void OnDestroy() => base.OnDestroy();
    }

