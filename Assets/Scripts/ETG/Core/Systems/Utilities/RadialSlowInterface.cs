using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

using Dungeonator;

#nullable disable

[Serializable]
public class RadialSlowInterface
    {
        public float EffectRadius = 100f;
        public float RadialSlowInTime;
        public float RadialSlowHoldTime = 1f;
        public float RadialSlowOutTime = 0.5f;
        public float RadialSlowTimeModifier = 0.25f;
        public string audioEvent;
        public bool DoesSepia;
        public bool DoesCirclePass;
        public bool UpdatesForNewEnemies = true;

        public void DoRadialSlow(Vector2 centerPoint, RoomHandler targetRoom)
        {
            List<AIActor> activeEnemies = targetRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
            if (!string.IsNullOrEmpty(this.audioEvent))
            {
                int num = (int) AkSoundEngine.PostEvent(this.audioEvent, GameManager.Instance.BestActivePlayer.gameObject);
            }
            if (activeEnemies != null && activeEnemies.Count > 0)
            {
                for (int index = 0; index < activeEnemies.Count; ++index)
                {
                    AIActor target = activeEnemies[index];
                    if ((bool) (UnityEngine.Object) target && target.IsNormalEnemy && (bool) (UnityEngine.Object) target.healthHaver && !target.IsGone)
                        target.StartCoroutine(this.ProcessSlow(centerPoint, target, 0.0f));
                }
            }
            if (this.DoesSepia)
                Pixelator.Instance.StartCoroutine(this.ProcessEnsepia());
            if (this.DoesCirclePass)
                Pixelator.Instance.StartCoroutine(this.ProcessCirclePass(centerPoint));
            if (!this.UpdatesForNewEnemies)
                return;
            GameManager.Instance.Dungeon.StartCoroutine(this.HandleNewEnemies(centerPoint, targetRoom));
        }

        [DebuggerHidden]
        private IEnumerator HandleNewEnemies(Vector2 centerPoint, RoomHandler targetRoom)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new RadialSlowInterface__HandleNewEnemiesc__Iterator0()
            {
                centerPoint = centerPoint,
                targetRoom = targetRoom,
                _this = this
            };
        }

        [DebuggerHidden]
        private IEnumerator ProcessCirclePass(Vector2 centerPoint)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new RadialSlowInterface__ProcessCirclePassc__Iterator1()
            {
                centerPoint = centerPoint,
                _this = this
            };
        }

        [DebuggerHidden]
        private IEnumerator ProcessEnsepia()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new RadialSlowInterface__ProcessEnsepiac__Iterator2()
            {
                _this = this
            };
        }

        [DebuggerHidden]
        private IEnumerator ProcessSlow(Vector2 centerPoint, AIActor target, float startTime)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new RadialSlowInterface__ProcessSlowc__Iterator3()
            {
                startTime = startTime,
                target = target,
                centerPoint = centerPoint,
                _this = this
            };
        }
    }

