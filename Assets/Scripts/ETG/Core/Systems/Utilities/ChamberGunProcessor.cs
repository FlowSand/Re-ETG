// Decompiled with JetBrains decompiler
// Type: ChamberGunProcessor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class ChamberGunProcessor : MonoBehaviour, ILevelLoadedListener
  {
    [PickupIdentifier]
    public int CastleGunID;
    [PickupIdentifier]
    public int GungeonGunID;
    [PickupIdentifier]
    public int MinesGunID;
    [PickupIdentifier]
    public int HollowGunID;
    [PickupIdentifier]
    public int ForgeGunID;
    [PickupIdentifier]
    public int HellGunID;
    [PickupIdentifier]
    public int OublietteGunID;
    [PickupIdentifier]
    public int AbbeyGunID;
    [PickupIdentifier]
    public int RatgeonGunID;
    [PickupIdentifier]
    public int OfficeGunID;
    public bool RefillsOnFloorChange = true;
    private GlobalDungeonData.ValidTilesets m_currentTileset;
    private Gun m_gun;
    [NonSerialized]
    public bool JustActiveReloaded;

    private void Awake()
    {
      this.m_currentTileset = GlobalDungeonData.ValidTilesets.CASTLEGEON;
      this.m_gun = this.GetComponent<Gun>();
      this.m_gun.OnReloadPressed += new Action<PlayerController, Gun, bool>(this.HandleReloadPressed);
    }

    private GlobalDungeonData.ValidTilesets GetFloorTileset()
    {
      return GameManager.Instance.IsLoadingLevel || !(bool) (UnityEngine.Object) GameManager.Instance.Dungeon ? GlobalDungeonData.ValidTilesets.CASTLEGEON : GameManager.Instance.Dungeon.tileIndices.tilesetId;
    }

    private bool IsValidTileset(GlobalDungeonData.ValidTilesets t)
    {
      if (t == this.GetFloorTileset())
        return true;
      PlayerController currentOwner = this.m_gun.CurrentOwner as PlayerController;
      return (bool) (UnityEngine.Object) currentOwner && (t == GlobalDungeonData.ValidTilesets.CASTLEGEON && currentOwner.HasPassiveItem(GlobalItemIds.MasteryToken_Castle) || t == GlobalDungeonData.ValidTilesets.GUNGEON && currentOwner.HasPassiveItem(GlobalItemIds.MasteryToken_Gungeon) || t == GlobalDungeonData.ValidTilesets.MINEGEON && currentOwner.HasPassiveItem(GlobalItemIds.MasteryToken_Mines) || t == GlobalDungeonData.ValidTilesets.CATACOMBGEON && currentOwner.HasPassiveItem(GlobalItemIds.MasteryToken_Catacombs) || t == GlobalDungeonData.ValidTilesets.FORGEGEON && currentOwner.HasPassiveItem(GlobalItemIds.MasteryToken_Forge));
    }

    private void ChangeToTileset(GlobalDungeonData.ValidTilesets t)
    {
      switch (t)
      {
        case GlobalDungeonData.ValidTilesets.GUNGEON:
          this.ChangeForme(this.GungeonGunID);
          this.m_currentTileset = GlobalDungeonData.ValidTilesets.GUNGEON;
          break;
        case GlobalDungeonData.ValidTilesets.CASTLEGEON:
          this.ChangeForme(this.CastleGunID);
          this.m_currentTileset = GlobalDungeonData.ValidTilesets.CASTLEGEON;
          break;
        case GlobalDungeonData.ValidTilesets.SEWERGEON:
          this.ChangeForme(this.OublietteGunID);
          this.m_currentTileset = GlobalDungeonData.ValidTilesets.SEWERGEON;
          break;
        case GlobalDungeonData.ValidTilesets.CATHEDRALGEON:
          this.ChangeForme(this.AbbeyGunID);
          this.m_currentTileset = GlobalDungeonData.ValidTilesets.CATHEDRALGEON;
          break;
        default:
          if (t != GlobalDungeonData.ValidTilesets.MINEGEON)
          {
            if (t != GlobalDungeonData.ValidTilesets.CATACOMBGEON)
            {
              if (t != GlobalDungeonData.ValidTilesets.FORGEGEON)
              {
                if (t != GlobalDungeonData.ValidTilesets.HELLGEON)
                {
                  if (t != GlobalDungeonData.ValidTilesets.OFFICEGEON)
                  {
                    if (t == GlobalDungeonData.ValidTilesets.RATGEON)
                    {
                      this.ChangeForme(this.RatgeonGunID);
                      this.m_currentTileset = GlobalDungeonData.ValidTilesets.RATGEON;
                      break;
                    }
                    this.ChangeForme(this.CastleGunID);
                    this.m_currentTileset = this.GetFloorTileset();
                    break;
                  }
                  this.ChangeForme(this.OfficeGunID);
                  this.m_currentTileset = GlobalDungeonData.ValidTilesets.OFFICEGEON;
                  break;
                }
                this.ChangeForme(this.HellGunID);
                this.m_currentTileset = GlobalDungeonData.ValidTilesets.HELLGEON;
                break;
              }
              this.ChangeForme(this.ForgeGunID);
              this.m_currentTileset = GlobalDungeonData.ValidTilesets.FORGEGEON;
              break;
            }
            this.ChangeForme(this.HollowGunID);
            this.m_currentTileset = GlobalDungeonData.ValidTilesets.CATACOMBGEON;
            break;
          }
          this.ChangeForme(this.MinesGunID);
          this.m_currentTileset = GlobalDungeonData.ValidTilesets.MINEGEON;
          break;
      }
    }

    private void ChangeForme(int targetID)
    {
      this.m_gun.TransformToTargetGun(PickupObjectDatabase.GetById(targetID) as Gun);
    }

    private void Update()
    {
      if (Dungeon.IsGenerating || GameManager.Instance.IsLoadingLevel)
        return;
      if ((bool) (UnityEngine.Object) this.m_gun && (!(bool) (UnityEngine.Object) this.m_gun.CurrentOwner || !this.IsValidTileset(this.m_currentTileset)))
      {
        GlobalDungeonData.ValidTilesets t = this.GetFloorTileset();
        if (!(bool) (UnityEngine.Object) this.m_gun.CurrentOwner)
          t = GlobalDungeonData.ValidTilesets.CASTLEGEON;
        if (this.m_currentTileset != t)
          this.ChangeToTileset(t);
      }
      this.JustActiveReloaded = false;
    }

    private GlobalDungeonData.ValidTilesets GetNextTileset(GlobalDungeonData.ValidTilesets inTileset)
    {
      switch (inTileset)
      {
        case GlobalDungeonData.ValidTilesets.GUNGEON:
          return GlobalDungeonData.ValidTilesets.CATHEDRALGEON;
        case GlobalDungeonData.ValidTilesets.CASTLEGEON:
          return GlobalDungeonData.ValidTilesets.SEWERGEON;
        case GlobalDungeonData.ValidTilesets.SEWERGEON:
          return GlobalDungeonData.ValidTilesets.GUNGEON;
        case GlobalDungeonData.ValidTilesets.CATHEDRALGEON:
          return GlobalDungeonData.ValidTilesets.MINEGEON;
        default:
          if (inTileset == GlobalDungeonData.ValidTilesets.MINEGEON)
            return GlobalDungeonData.ValidTilesets.RATGEON;
          if (inTileset == GlobalDungeonData.ValidTilesets.CATACOMBGEON)
            return GlobalDungeonData.ValidTilesets.OFFICEGEON;
          if (inTileset == GlobalDungeonData.ValidTilesets.FORGEGEON)
            return GlobalDungeonData.ValidTilesets.HELLGEON;
          if (inTileset == GlobalDungeonData.ValidTilesets.HELLGEON)
            return GlobalDungeonData.ValidTilesets.CASTLEGEON;
          if (inTileset == GlobalDungeonData.ValidTilesets.OFFICEGEON)
            return GlobalDungeonData.ValidTilesets.FORGEGEON;
          return inTileset == GlobalDungeonData.ValidTilesets.RATGEON ? GlobalDungeonData.ValidTilesets.CATACOMBGEON : GlobalDungeonData.ValidTilesets.CASTLEGEON;
      }
    }

    private GlobalDungeonData.ValidTilesets GetNextValidTileset()
    {
      GlobalDungeonData.ValidTilesets nextTileset = this.GetNextTileset(this.m_currentTileset);
      while (!this.IsValidTileset(nextTileset))
        nextTileset = this.GetNextTileset(nextTileset);
      return nextTileset;
    }

    private void HandleReloadPressed(PlayerController ownerPlayer, Gun sourceGun, bool manual)
    {
      if (this.JustActiveReloaded || !manual || sourceGun.IsReloading)
        return;
      GlobalDungeonData.ValidTilesets nextValidTileset = this.GetNextValidTileset();
      if (this.m_currentTileset == nextValidTileset)
        return;
      this.ChangeToTileset(nextValidTileset);
    }

    public void BraveOnLevelWasLoaded()
    {
      if (!this.RefillsOnFloorChange || !(bool) (UnityEngine.Object) this.m_gun || !(bool) (UnityEngine.Object) this.m_gun.CurrentOwner)
        return;
      this.m_gun.StartCoroutine(this.DelayedRegainAmmo());
    }

    [DebuggerHidden]
    private IEnumerator DelayedRegainAmmo()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new ChamberGunProcessor__DelayedRegainAmmoc__Iterator0()
      {
        _this = this
      };
    }
  }

