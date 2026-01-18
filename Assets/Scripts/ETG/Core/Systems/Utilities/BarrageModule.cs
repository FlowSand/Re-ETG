using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

#nullable disable

[Serializable]
public class BarrageModule
    {
        public int BarrageColumns = 1;
        public GameObject barrageVFX;
        public ExplosionData barrageExplosionData;
        public float barrageRadius = 1.5f;
        public float delayBetweenStrikes = 0.25f;
        public float BarrageWidth = 3f;
        public float BarrageLength = 5f;

        public void DoBarrage(Vector2 startPoint, Vector2 direction, MonoBehaviour coroutineTarget)
        {
            List<Vector2> targets = this.AcquireBarrageTargets(startPoint, direction);
            coroutineTarget.StartCoroutine(this.HandleBarrage(targets));
        }

        protected List<Vector2> AcquireBarrageTargets(Vector2 startPoint, Vector2 direction)
        {
            List<Vector2> vector2List = new List<Vector2>();
            float num1 = (float) (-(double) this.barrageRadius / 2.0);
            Quaternion quaternion = Quaternion.Euler(0.0f, 0.0f, BraveMathCollege.Atan2Degrees(direction));
            for (; (double) num1 < (double) this.BarrageLength; num1 += this.barrageRadius)
            {
                Mathf.Clamp01(num1 / this.BarrageLength);
                float barrageWidth = this.BarrageWidth;
                float x = Mathf.Clamp(num1, 0.0f, this.BarrageLength);
                for (int index = 0; index < this.BarrageColumns; ++index)
                {
                    float num2 = Mathf.Lerp(-barrageWidth, barrageWidth, (float) (((double) index + 1.0) / ((double) this.BarrageColumns + 1.0)));
                    float num3 = UnityEngine.Random.Range((float) (-(double) barrageWidth / (4.0 * (double) this.BarrageColumns)), barrageWidth / (4f * (float) this.BarrageColumns));
                    Vector2 vector2_1 = new Vector2(x, num2 + num3);
                    Vector2 vector2_2 = (quaternion * (Vector3) vector2_1).XY();
                    vector2List.Add(startPoint + vector2_2);
                }
            }
            return vector2List;
        }

        [DebuggerHidden]
        private IEnumerator HandleBarrage(List<Vector2> targets)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new BarrageModule__HandleBarragec__Iterator0()
            {
                targets = targets,
                _this = this
            };
        }
    }

