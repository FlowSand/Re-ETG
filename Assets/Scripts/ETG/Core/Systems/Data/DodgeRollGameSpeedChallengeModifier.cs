using System;

using UnityEngine;

#nullable disable

public class DodgeRollGameSpeedChallengeModifier : ChallengeModifier
    {
        public float SpeedGain = 2.5f;
        public float SpeedMax = 1.5f;
        [Header("Boss Parameters")]
        public float BossSpeedGain = 1f;
        public float BossSpeedMax = 1.3f;
        private float CurrentSpeedModifier = 1f;

        private void Start()
        {
            for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
                GameManager.Instance.AllPlayers[index].OnPreDodgeRoll += new Action<PlayerController>(this.OnDodgeRoll);
        }

        private void OnDodgeRoll(PlayerController obj)
        {
            float num = this.SpeedGain;
            float max = this.SpeedMax;
            if (GameManager.Instance.PrimaryPlayer.CurrentRoom.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.BOSS)
            {
                num = this.BossSpeedGain;
                max = this.BossSpeedMax;
            }
            this.CurrentSpeedModifier = Mathf.Clamp(this.CurrentSpeedModifier + num * 0.01f, 1f, max);
            BraveTime.ClearMultiplier(this.gameObject);
            BraveTime.RegisterTimeScaleMultiplier(this.CurrentSpeedModifier, this.gameObject);
        }

        private void OnDestroy()
        {
            for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
                GameManager.Instance.AllPlayers[index].OnPreDodgeRoll -= new Action<PlayerController>(this.OnDodgeRoll);
            BraveTime.ClearMultiplier(this.gameObject);
        }
    }

