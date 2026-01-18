using System.Collections.Generic;

using UnityEngine;

#nullable disable

public class TrashLauncher : BraveBehaviour
    {
        public TrashLauncher.TrashManipulateMode mode;
        private HashSet<DebrisObject> m_debris = new HashSet<DebrisObject>();
        private PlayerController m_player;
        public float liftIntensity = 2f;

        private void Start() => this.m_player = this.GetComponentInParent<PlayerController>();

        private void Update()
        {
            Vector2 worldCenter = this.sprite.WorldCenter;
            for (int index = 0; index < StaticReferenceManager.AllDebris.Count; ++index)
            {
                DebrisObject allDebri = StaticReferenceManager.AllDebris[index];
                if ((bool) (Object) allDebri && !allDebri.IsPickupObject && allDebri.Priority != EphemeralObject.EphemeralPriority.Critical)
                {
                    Vector2 vector2_1 = !(bool) (Object) allDebri.sprite ? allDebri.transform.position.XY() : allDebri.sprite.WorldCenter;
                    Vector2 vector2_2 = worldCenter - vector2_1;
                    float sqrMagnitude = vector2_2.sqrMagnitude;
                    switch (this.mode)
                    {
                        case TrashLauncher.TrashManipulateMode.GATHER_AND_TOSS:
                            if ((double) sqrMagnitude < 100.0)
                            {
                                if (!this.m_debris.Contains(allDebri))
                                    this.m_debris.Add(allDebri);
                                if (allDebri.HasBeenTriggered)
                                {
                                    allDebri.ApplyVelocity(vector2_2.normalized * 25f * allDebri.inertialMass * BraveTime.DeltaTime);
                                    allDebri.PreventFallingInPits = true;
                                    continue;
                                }
                                continue;
                            }
                            continue;
                        case TrashLauncher.TrashManipulateMode.DRAGONBALL_Z:
                            if ((double) sqrMagnitude < 100.0)
                            {
                                if (!this.m_debris.Contains(allDebri))
                                    this.m_debris.Add(allDebri);
                                if (allDebri.HasBeenTriggered && (double) allDebri.UnadjustedDebrisPosition.z < 0.75)
                                {
                                    allDebri.IncrementZHeight(this.liftIntensity * BraveTime.DeltaTime);
                                    continue;
                                }
                                continue;
                            }
                            continue;
                        default:
                            continue;
                    }
                }
            }
        }

        public void OnDespawned()
        {
            if (this.mode == TrashLauncher.TrashManipulateMode.GATHER_AND_TOSS)
            {
                Vector2 vector2_1 = Random.insideUnitCircle;
                if ((bool) (Object) this.m_player)
                    vector2_1 = this.m_player.unadjustedAimPoint.XY() - this.m_player.CenterPosition;
                vector2_1 = vector2_1.normalized;
                foreach (DebrisObject debri in this.m_debris)
                {
                    if ((bool) (Object) debri)
                    {
                        Vector2 vector2_2 = (Vector2) (Quaternion.Euler(0.0f, 0.0f, (float) Random.Range(-15, 15)) * (Vector3) vector2_1);
                        debri.ApplyVelocity(vector2_2 * (float) Random.Range(45, 55));
                    }
                }
            }
            this.OnDestroy();
        }

        public enum TrashManipulateMode
        {
            GATHER_AND_TOSS,
            DRAGONBALL_Z,
        }
    }

