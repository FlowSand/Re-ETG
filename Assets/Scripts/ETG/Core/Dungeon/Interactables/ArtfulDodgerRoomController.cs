using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using HutongGames.PlayMaker;
using UnityEngine;

using Dungeonator;

#nullable disable

public class ArtfulDodgerRoomController : DungeonPlaceableBehaviour, IPlaceConfigurable
    {
        [DwarfConfigurable]
        public float NumberShots = 3f;
        [DwarfConfigurable]
        public float NumberBounces = 1f;
        private Fsm m_fsm;
        private bool m_hasActivated;
        private bool m_rewardHandled;
        private List<ArtfulDodgerTargetController> m_targets = new List<ArtfulDodgerTargetController>();
        private List<ArtfulDodgerCameraManipulator> m_cameraZones = new List<ArtfulDodgerCameraManipulator>();
        [NonSerialized]
        public PlayerController gamePlayingPlayer;

        public bool Completed => this.m_rewardHandled;

        public void RegisterTarget(ArtfulDodgerTargetController target) => this.m_targets.Add(target);

        public void RegisterCameraZone(ArtfulDodgerCameraManipulator zone)
        {
            this.m_cameraZones.Add(zone);
        }

        public void Activate(Fsm sourceFsm)
        {
            this.m_hasActivated = true;
            this.m_fsm = sourceFsm;
            for (int index = 0; index < this.m_cameraZones.Count; ++index)
                this.m_cameraZones[index].Active = true;
            for (int index = 0; index < this.m_targets.Count; ++index)
                this.m_targets[index].Activate();
            GameManager.Instance.DungeonMusicController.StartArcadeGame();
        }

        public void DoHandleReward()
        {
            GameManager.Instance.DungeonMusicController.SwitchToArcadeMusic();
            this.StartCoroutine(this.HandleReward());
        }

        private void DoConfetti(Vector2 targetCenter)
        {
            string[] strArray = new string[3]
            {
                "Global VFX/Confetti_Blue_001",
                "Global VFX/Confetti_Yellow_001",
                "Global VFX/Confetti_Green_001"
            };
            Vector2 vector = targetCenter;
            for (int index = 0; index < 8; ++index)
            {
                WaftingDebrisObject component = UnityEngine.Object.Instantiate<GameObject>((GameObject) BraveResources.Load(strArray[UnityEngine.Random.Range(0, 3)])).GetComponent<WaftingDebrisObject>();
                component.sprite.PlaceAtPositionByAnchor(vector.ToVector3ZUp() + new Vector3(0.5f, 0.5f, 0.0f), tk2dBaseSprite.Anchor.MiddleCenter);
                Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
                insideUnitCircle.y = -Mathf.Abs(insideUnitCircle.y);
                component.Trigger(insideUnitCircle.ToVector3ZUp(1.5f) * UnityEngine.Random.Range(0.5f, 2f), 0.5f, 0.0f);
            }
        }

        [DebuggerHidden]
        public IEnumerator HandleReward()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new ArtfulDodgerRoomController__HandleRewardc__Iterator0()
            {
                _this = this
            };
        }

        public void LateUpdate()
        {
            if (!this.m_hasActivated || this.m_rewardHandled)
                return;
            bool flag = true;
            for (int index = 0; index < this.m_targets.Count; ++index)
            {
                if (!this.m_targets[index].IsBroken)
                {
                    flag = false;
                    break;
                }
            }
            if (!flag)
                return;
            this.StartCoroutine(this.HandleReward());
        }

        protected override void OnDestroy() => base.OnDestroy();

        public void ConfigureOnPlacement(RoomHandler room)
        {
            if (room.RoomVisualSubtype >= 0 && room.RoomVisualSubtype < GameManager.Instance.BestGenerationDungeonPrefab.roomMaterialDefinitions.Length && !GameManager.Instance.BestGenerationDungeonPrefab.roomMaterialDefinitions[room.RoomVisualSubtype].supportsPits)
                room.RoomVisualSubtype = 0;
            room.IsWinchesterArcadeRoom = true;
            room.Entered += new RoomHandler.OnEnteredEventHandler(this.HandleArcadeMusicEvents);
            if (room.connectedRooms.Count != 1)
                return;
            room.ShouldAttemptProceduralLock = true;
            room.AttemptProceduralLockChance = Mathf.Max(room.AttemptProceduralLockChance, UnityEngine.Random.Range(0.3f, 0.5f));
        }

        private void HandleArcadeMusicEvents(PlayerController p)
        {
            GameManager.Instance.DungeonMusicController.SwitchToArcadeMusic();
        }
    }

