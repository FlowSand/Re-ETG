// Decompiled with JetBrains decompiler
// Type: GunOverheatChallengeModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Systems.Data
{
    public class GunOverheatChallengeModifier : ChallengeModifier
    {
      public GoopDefinition Goop;
      public float Radius = 3f;

      private void Start()
      {
        for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
          GameManager.Instance.AllPlayers[index].OnReloadedGun += new Action<PlayerController, Gun>(this.HandleGunReloaded);
      }

      private void HandleGunReloaded(PlayerController player, Gun playerGun)
      {
        if (playerGun.ClipShotsRemaining != 0)
          return;
        DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.Goop).TimedAddGoopCircle(player.CenterPosition, this.Radius);
      }

      public void ForceGoop(PlayerController player)
      {
        DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.Goop).TimedAddGoopCircle(player.CenterPosition, this.Radius);
      }

      private void OnDestroy()
      {
        for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
          GameManager.Instance.AllPlayers[index].OnReloadedGun -= new Action<PlayerController, Gun>(this.HandleGunReloaded);
      }
    }

}
