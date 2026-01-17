// Decompiled with JetBrains decompiler
// Type: DodgeRollGameSpeedChallengeModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Data
{
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

}
