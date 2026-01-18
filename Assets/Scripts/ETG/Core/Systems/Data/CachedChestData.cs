// Decompiled with JetBrains decompiler
// Type: CachedChestData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

[Serializable]
public class CachedChestData
  {
    private bool m_glitch;
    private bool m_rainbow;
    private bool m_synergy;
    private PickupObject.ItemQuality chestQuality = PickupObject.ItemQuality.D;
    private bool m_isLocked;

    public CachedChestData(string data) => this.Deserialize(data);

    public CachedChestData(Chest c)
    {
      this.m_isLocked = c.IsLocked;
      this.m_glitch = c.IsGlitched;
      this.m_rainbow = c.IsRainbowChest;
      this.m_synergy = c.lootTable != null && c.lootTable.CompletesSynergy;
      RewardManager rewardManager = GameManager.Instance.RewardManager;
      this.chestQuality = PickupObject.ItemQuality.D;
      if (c.name.Contains(rewardManager.S_Chest.name))
        this.chestQuality = PickupObject.ItemQuality.S;
      else if (c.name.Contains(rewardManager.A_Chest.name))
        this.chestQuality = PickupObject.ItemQuality.A;
      else if (c.name.Contains(rewardManager.B_Chest.name))
      {
        this.chestQuality = PickupObject.ItemQuality.B;
      }
      else
      {
        if (!c.name.Contains(rewardManager.C_Chest.name))
          return;
        this.chestQuality = PickupObject.ItemQuality.C;
      }
    }

    public void Upgrade()
    {
      switch (this.chestQuality)
      {
        case PickupObject.ItemQuality.COMMON:
          this.chestQuality = PickupObject.ItemQuality.D;
          break;
        case PickupObject.ItemQuality.D:
          this.chestQuality = PickupObject.ItemQuality.C;
          break;
        case PickupObject.ItemQuality.C:
          this.chestQuality = PickupObject.ItemQuality.B;
          break;
        case PickupObject.ItemQuality.B:
          this.chestQuality = PickupObject.ItemQuality.A;
          break;
        case PickupObject.ItemQuality.A:
          this.chestQuality = PickupObject.ItemQuality.S;
          break;
      }
    }

    public string Serialize()
    {
      return $"{this.chestQuality.ToString()}|{this.m_glitch.ToString()}|{this.m_rainbow.ToString()}|{this.m_isLocked.ToString()}|{this.m_synergy.ToString()}";
    }

    public void Deserialize(string data)
    {
      string[] strArray = data.Split('|');
      this.chestQuality = (PickupObject.ItemQuality) Enum.Parse(typeof (PickupObject.ItemQuality), strArray[0]);
      this.m_glitch = bool.Parse(strArray[1]);
      this.m_rainbow = bool.Parse(strArray[2]);
      this.m_isLocked = bool.Parse(strArray[3]);
      if (strArray.Length <= 4)
        return;
      this.m_synergy = bool.Parse(strArray[4]);
    }

    public void SpawnChest(IntVector2 position)
    {
      Chest chestPrefab;
      if (this.m_synergy)
      {
        chestPrefab = GameManager.Instance.RewardManager.Synergy_Chest;
      }
      else
      {
        switch (this.chestQuality)
        {
          case PickupObject.ItemQuality.D:
            chestPrefab = GameManager.Instance.RewardManager.D_Chest;
            break;
          case PickupObject.ItemQuality.C:
            chestPrefab = GameManager.Instance.RewardManager.C_Chest;
            break;
          case PickupObject.ItemQuality.B:
            chestPrefab = GameManager.Instance.RewardManager.B_Chest;
            break;
          case PickupObject.ItemQuality.A:
            chestPrefab = GameManager.Instance.RewardManager.A_Chest;
            break;
          case PickupObject.ItemQuality.S:
            chestPrefab = GameManager.Instance.RewardManager.S_Chest;
            break;
          default:
            chestPrefab = GameManager.Instance.RewardManager.D_Chest;
            break;
        }
      }
      if (!(bool) (UnityEngine.Object) chestPrefab)
        return;
      Chest chest = Chest.Spawn(chestPrefab, position);
      chest.RegisterChestOnMinimap(position.ToVector2().GetAbsoluteRoom());
      chest.IsLocked = this.m_isLocked;
      if (this.m_glitch)
        chest.BecomeGlitchChest();
      if (!this.m_rainbow)
        return;
      chest.BecomeRainbowChest();
    }
  }

