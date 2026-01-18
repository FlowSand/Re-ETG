// Decompiled with JetBrains decompiler
// Type: GunQueueChallengeModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;

#nullable disable

public class GunQueueChallengeModifier : ChallengeModifier
  {
    public float AutoSwitchTime = 15f;
    private float m_elapsed;
    private float m_gunQueueTimer;

    private void Start()
    {
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      {
        GameManager.Instance.AllPlayers[index].inventory.GunLocked.SetOverride("challenge", true);
        GameManager.Instance.AllPlayers[index].OnReloadPressed += new Action<PlayerController, Gun>(this.HandleGunReloadPress);
        GameManager.Instance.AllPlayers[index].OnReloadedGun += new Action<PlayerController, Gun>(this.HandleGunReloaded);
      }
      this.m_gunQueueTimer = this.AutoSwitchTime;
    }

    private void Update()
    {
      this.m_elapsed += BraveTime.DeltaTime;
      this.m_gunQueueTimer -= BraveTime.DeltaTime;
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      {
        Gun currentGun = GameManager.Instance.AllPlayers[index].CurrentGun;
        if ((bool) (UnityEngine.Object) currentGun && (currentGun.ammo == 0 || currentGun.UsesRechargeLikeActiveItem && (double) currentGun.RemainingActiveCooldownAmount > 0.0))
          this.HandleGunReloaded(GameManager.Instance.AllPlayers[index], (Gun) null);
      }
      if ((double) this.m_gunQueueTimer > 0.0)
        return;
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        this.HandleGunReloaded(GameManager.Instance.AllPlayers[index], (Gun) null);
    }

    public override bool IsValid(RoomHandler room)
    {
      if (room.IsGunslingKingChallengeRoom)
        return false;
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      {
        if ((bool) (UnityEngine.Object) GameManager.Instance.AllPlayers[index] && GameManager.Instance.AllPlayers[index].inventory != null && GameManager.Instance.AllPlayers[index].inventory.AllGuns.Count > 1)
          return true;
      }
      return false;
    }

    private void HandleGunReloadPress(PlayerController player, Gun playerGun)
    {
      if ((double) this.m_elapsed <= 1.0)
        return;
      this.QueueLogic(player, playerGun);
    }

    private void HandleGunReloaded(PlayerController player, Gun playerGun)
    {
      this.QueueLogic(player, playerGun);
    }

    private void QueueLogic(PlayerController player, Gun playerGun)
    {
      if (!(bool) (UnityEngine.Object) this)
        return;
      player.inventory.GunLocked.RemoveOverride("challenge");
      Gun currentGun = player.CurrentGun;
      if ((bool) (UnityEngine.Object) currentGun && player.inventory.GunLocked.Value)
      {
        MimicGunController component = currentGun.GetComponent<MimicGunController>();
        if ((bool) (UnityEngine.Object) component)
          component.ForceClearMimic();
      }
      if ((bool) (UnityEngine.Object) ChallengeManager.Instance && (bool) (UnityEngine.Object) currentGun && currentGun.ClipShotsRemaining == 0)
      {
        for (int index = 0; index < ChallengeManager.Instance.ActiveChallenges.Count; ++index)
        {
          if (ChallengeManager.Instance.ActiveChallenges[index] is GunOverheatChallengeModifier)
            (ChallengeManager.Instance.ActiveChallenges[index] as GunOverheatChallengeModifier).ForceGoop(player);
        }
      }
      if ((bool) (UnityEngine.Object) currentGun)
        currentGun.ForceImmediateReload(true);
      player.inventory.GunChangeForgiveness = true;
      player.ChangeGun(1);
      player.inventory.GunChangeForgiveness = false;
      player.inventory.GunLocked.SetOverride("challenge", true);
      this.m_gunQueueTimer = this.AutoSwitchTime;
      this.m_elapsed = 0.0f;
    }

    private void OnDestroy()
    {
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      {
        GameManager.Instance.AllPlayers[index].OnReloadedGun -= new Action<PlayerController, Gun>(this.HandleGunReloaded);
        GameManager.Instance.AllPlayers[index].OnReloadPressed -= new Action<PlayerController, Gun>(this.HandleGunReloadPress);
        GameManager.Instance.AllPlayers[index].inventory.GunLocked.RemoveOverride("challenge");
      }
    }
  }

