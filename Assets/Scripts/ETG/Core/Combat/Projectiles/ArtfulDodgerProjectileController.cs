using System;
using System.Collections.Generic;

using UnityEngine;

#nullable disable

public class ArtfulDodgerProjectileController : MonoBehaviour
    {
        [NonSerialized]
        public bool hitTarget;
        private Projectile m_projectile;
        private BounceProjModifier m_bouncer;

        private void Start()
        {
            this.m_projectile = this.GetComponent<Projectile>();
            this.m_projectile.OnDestruction += new Action<Projectile>(this.HandleDestruction);
            this.m_bouncer = this.GetComponent<BounceProjModifier>();
        }

        private void HandleDestruction(Projectile source)
        {
            if (!this.hitTarget)
            {
                List<ArtfulDodgerTargetController> componentsAbsoluteInRoom = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.transform.position.IntXY(VectorConversions.Floor)).GetComponentsAbsoluteInRoom<ArtfulDodgerTargetController>();
                for (int index = 0; index < componentsAbsoluteInRoom.Count; ++index)
                {
                    if (!componentsAbsoluteInRoom[index].IsBroken)
                        componentsAbsoluteInRoom[index].GetComponentInChildren<tk2dSpriteAnimator>().PlayForDuration("target_miss", 3f, "target_idle");
                }
            }
            else
                GameStatsManager.Instance.RegisterStatChange(TrackedStats.WINCHESTER_SHOTS_HIT, 1f);
        }

        private void Update()
        {
            this.m_projectile.ChangeTintColorShader(0.0f, BraveUtility.GetRainbowColor(this.m_bouncer.numberOfBounces));
        }
    }

