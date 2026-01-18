using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

#nullable disable

public class DraGunBoulderController : BraveBehaviour
    {
        public float LifeTime = 1f;
        public tk2dSprite CircleSprite;
        private float m_lifeTime;
        private List<PlayerController> m_cursedPlayers = new List<PlayerController>();

        public void Start()
        {
            this.specRigidbody.OnEnterTrigger += new SpeculativeRigidbody.OnTriggerDelegate(this.HandleTriggerEntered);
            this.specRigidbody.OnExitTrigger += new SpeculativeRigidbody.OnTriggerExitDelegate(this.HandleTriggerExited);
            if ((bool) (Object) this.CircleSprite)
            {
                tk2dSpriteDefinition currentSpriteDef = this.CircleSprite.GetCurrentSpriteDef();
                Vector2 lhs1 = new Vector2(float.MaxValue, float.MaxValue);
                Vector2 lhs2 = new Vector2(float.MinValue, float.MinValue);
                for (int index = 0; index < currentSpriteDef.uvs.Length; ++index)
                {
                    lhs1 = Vector2.Min(lhs1, currentSpriteDef.uvs[index]);
                    lhs2 = Vector2.Max(lhs2, currentSpriteDef.uvs[index]);
                }
                Vector2 vector2 = (lhs1 + lhs2) / 2f;
                this.CircleSprite.renderer.material.SetVector("_WorldCenter", new Vector4(vector2.x, vector2.y, vector2.x - lhs1.x, vector2.y - lhs1.y));
            }
            this.m_lifeTime = 0.0f;
        }

        private void Update()
        {
            this.m_lifeTime += BraveTime.DeltaTime;
            if ((double) this.m_lifeTime >= (double) this.LifeTime)
            {
                this.m_lifeTime = 0.0f;
                GameManager.Instance.Dungeon.StartCoroutine(this.HandleBreakCR());
            }
            for (int index = 0; index < this.m_cursedPlayers.Count; ++index)
                this.DoCurse(this.m_cursedPlayers[index]);
        }

        [DebuggerHidden]
        private IEnumerator HandleBreakCR()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new DraGunBoulderController__HandleBreakCRc__Iterator0()
            {
                _this = this
            };
        }

        private void DoCurse(PlayerController targetPlayer)
        {
            if (targetPlayer.IsGhost)
                return;
            targetPlayer.CurrentStoneGunTimer = Mathf.Max(targetPlayer.CurrentStoneGunTimer, 0.3f);
        }

        private void HandleTriggerExited(
            SpeculativeRigidbody exitRigidbody,
            SpeculativeRigidbody sourceSpecRigidbody)
        {
            if (!(bool) (Object) exitRigidbody || !(bool) (Object) exitRigidbody.gameActor || !(exitRigidbody.gameActor is PlayerController) || !this.m_cursedPlayers.Contains(exitRigidbody.gameActor as PlayerController))
                return;
            this.m_cursedPlayers.Remove(exitRigidbody.gameActor as PlayerController);
        }

        private void HandleTriggerEntered(
            SpeculativeRigidbody enteredRigidbody,
            SpeculativeRigidbody sourceSpecRigidbody,
            CollisionData collisionData)
        {
            if (GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.specRigidbody.UnitCenter.ToIntVector2()) != GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(enteredRigidbody.UnitCenter.ToIntVector2()) || !((Object) enteredRigidbody.gameActor != (Object) null) || !(enteredRigidbody.gameActor is PlayerController))
                return;
            this.m_cursedPlayers.Add(enteredRigidbody.gameActor as PlayerController);
        }
    }

