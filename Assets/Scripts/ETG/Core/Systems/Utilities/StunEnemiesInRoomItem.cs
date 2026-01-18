using System;
using System.Collections.Generic;

using UnityEngine;

using Dungeonator;

#nullable disable

public class StunEnemiesInRoomItem : MonoBehaviour
    {
        public float StunDuration = 5f;
        public bool DoChaffParticles = true;
        public bool AllowStealing = true;

        protected void AffectEnemy(AIActor target)
        {
            if (!(bool) (UnityEngine.Object) target || !(bool) (UnityEngine.Object) target.behaviorSpeculator)
                return;
            target.behaviorSpeculator.Stun(this.StunDuration);
        }

        private void Start()
        {
            int num = (int) AkSoundEngine.PostEvent("Play_OBJ_item_throw_01", this.gameObject);
            DebrisObject component = this.GetComponent<DebrisObject>();
            component.killTranslationOnBounce = false;
            if (!(bool) (UnityEngine.Object) component)
                return;
            component.OnGrounded += new Action<DebrisObject>(this.OnHitGround);
        }

        private void OnHitGround(DebrisObject obj)
        {
            Pixelator.Instance.FadeToColor(0.1f, Color.white, true, 0.1f);
            RoomHandler absoluteRoom = this.transform.position.GetAbsoluteRoom();
            List<AIActor> activeEnemies = absoluteRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
            if (activeEnemies != null)
            {
                for (int index = 0; index < activeEnemies.Count; ++index)
                    this.AffectEnemy(activeEnemies[index]);
            }
            if (this.DoChaffParticles)
            {
                GlobalSparksDoer.DoRandomParticleBurst(100, absoluteRoom.area.basePosition.ToVector3(), absoluteRoom.area.basePosition.ToVector3() + absoluteRoom.area.dimensions.ToVector3(), Vector3.up / 3f, 180f, 0.0f, new float?(0.125f), new float?(this.StunDuration), new Color?(Color.yellow), GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                int num = (int) AkSoundEngine.PostEvent("Play_OBJ_chaff_blast_01", this.gameObject);
            }
            if (this.AllowStealing)
            {
                List<BaseShopController> allShops = StaticReferenceManager.AllShops;
                for (int index = 0; index < allShops.Count; ++index)
                {
                    if ((bool) (UnityEngine.Object) allShops[index] && allShops[index].GetAbsoluteParentRoom() == absoluteRoom)
                        allShops[index].SetCapableOfBeingStolenFrom(true, nameof (StunEnemiesInRoomItem), new float?(this.StunDuration));
                }
            }
            UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
        }
    }

