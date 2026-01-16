// Decompiled with JetBrains decompiler
// Type: PlayerConsumables
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[Serializable]
public class PlayerConsumables
{
  [NonSerialized]
  private bool m_infiniteKeys;
  [SerializeField]
  private int StartCurrency;
  [SerializeField]
  private int StartKeyBullets = 1;
  private int m_currency;
  private int m_keyBullets;
  private int m_ratKeys;

  public int Currency
  {
    get => this.m_currency;
    set
    {
      int num = Mathf.Max(0, value);
      if (num > this.m_currency && GameStatsManager.HasInstance)
        GameStatsManager.Instance.RegisterStatChange(TrackedStats.TOTAL_MONEY_COLLECTED, (float) (num - this.m_currency));
      if (num >= 300 && GameManager.HasInstance && GameManager.Instance.platformInterface != null)
      {
        float realtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
        if ((double) realtimeSinceStartup > (double) PlatformInterface.LastManyCoinsUnlockTime + 5.0 || (double) realtimeSinceStartup < (double) PlatformInterface.LastManyCoinsUnlockTime)
        {
          GameManager.Instance.platformInterface.AchievementUnlock(Achievement.HAVE_MANY_COINS);
          PlatformInterface.LastManyCoinsUnlockTime = realtimeSinceStartup;
        }
      }
      this.m_currency = num;
      if (!GameUIRoot.HasInstance)
        return;
      GameUIRoot.Instance.UpdatePlayerConsumables(this);
    }
  }

  public int KeyBullets
  {
    get => this.m_keyBullets;
    set
    {
      this.m_keyBullets = value;
      GameStatsManager.Instance.UpdateMaximum(TrackedMaximums.MOST_KEYS_HELD, (float) this.m_keyBullets);
      GameUIRoot.Instance.UpdatePlayerConsumables(this);
    }
  }

  public int ResourcefulRatKeys
  {
    get => this.m_ratKeys;
    set
    {
      this.m_ratKeys = value;
      GameUIRoot.Instance.UpdatePlayerConsumables(this);
    }
  }

  public void Initialize()
  {
    this.Currency = this.StartCurrency;
    this.KeyBullets = this.StartKeyBullets;
  }

  public void ForceUpdateUI()
  {
    if (!((UnityEngine.Object) GameUIRoot.Instance != (UnityEngine.Object) null))
      return;
    GameUIRoot.Instance.UpdatePlayerConsumables(this);
  }

  public bool InfiniteKeys
  {
    get
    {
      if (GameManager.Instance.AllPlayers != null)
      {
        for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        {
          PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
          if ((bool) (UnityEngine.Object) allPlayer && (bool) (UnityEngine.Object) allPlayer.CurrentGun && allPlayer.CurrentGun.gunName == "AKey-47")
            return true;
        }
      }
      return this.m_infiniteKeys;
    }
    set => this.m_infiniteKeys = value;
  }
}
