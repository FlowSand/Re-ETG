using System;
using System.Collections.Generic;

using UnityEngine;

#nullable disable

public class DevolverModifier : MonoBehaviour
    {
        public float chanceToDevolve = 0.1f;
        public List<DevolverTier> DevolverHierarchy = new List<DevolverTier>();
        public List<string> EnemyGuidsToIgnore = new List<string>();

        private void Start()
        {
            Projectile component = this.GetComponent<Projectile>();
            if (!(bool) (UnityEngine.Object) component)
                return;
            component.OnHitEnemy += new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHitEnemy);
        }

        private void HandleHitEnemy(
            Projectile sourceProjectile,
            SpeculativeRigidbody enemyRigidbody,
            bool killingBlow)
        {
            if (killingBlow || !(bool) (UnityEngine.Object) enemyRigidbody || !(bool) (UnityEngine.Object) enemyRigidbody.aiActor || (double) UnityEngine.Random.value > (double) this.chanceToDevolve)
                return;
            AIActor aiActor = enemyRigidbody.aiActor;
            if (!aiActor.IsNormalEnemy || aiActor.IsHarmlessEnemy || aiActor.healthHaver.IsBoss)
                return;
            string enemyGuid = aiActor.EnemyGuid;
            for (int index = 0; index < this.EnemyGuidsToIgnore.Count; ++index)
            {
                if (this.EnemyGuidsToIgnore[index] == enemyGuid)
                    return;
            }
            int index1 = this.DevolverHierarchy.Count - 1;
            for (int index2 = 0; index2 < this.DevolverHierarchy.Count; ++index2)
            {
                List<string> tierGuids = this.DevolverHierarchy[index2].tierGuids;
                for (int index3 = 0; index3 < tierGuids.Count; ++index3)
                {
                    if (tierGuids[index3] == enemyGuid)
                    {
                        index1 = index2 - 1;
                        break;
                    }
                }
            }
            if (index1 < 0 || index1 >= this.DevolverHierarchy.Count)
                return;
            List<string> tierGuids1 = this.DevolverHierarchy[index1].tierGuids;
            string guid = tierGuids1[UnityEngine.Random.Range(0, tierGuids1.Count)];
            aiActor.Transmogrify(EnemyDatabase.GetOrLoadByGuid(guid), (GameObject) ResourceCache.Acquire("Global VFX/VFX_Item_Spawn_Poof"));
            int num = (int) AkSoundEngine.PostEvent("Play_WPN_devolver_morph_01", this.gameObject);
        }
    }

