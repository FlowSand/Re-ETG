// Decompiled with JetBrains decompiler
// Type: InvisibleBridgeMaker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class InvisibleBridgeMaker : DungeonPlaceableBehaviour
  {
    public MovingPlatform InvisiblePlatform2x2;
    private List<MovingPlatform> m_extantPlatforms = new List<MovingPlatform>();

    private void Start()
    {
      this.RegenerateBridge();
      GameManager.Instance.PrimaryPlayer.OnReceivedDamage += new Action<PlayerController>(this.HandlePlayerDamaged);
      if (GameManager.Instance.CurrentGameType != GameManager.GameType.COOP_2_PLAYER)
        return;
      GameManager.Instance.SecondaryPlayer.OnReceivedDamage += new Action<PlayerController>(this.HandlePlayerDamaged);
    }

    protected override void OnDestroy()
    {
      if (!GameManager.HasInstance)
        return;
      if ((bool) (UnityEngine.Object) GameManager.Instance.PrimaryPlayer)
        GameManager.Instance.PrimaryPlayer.OnReceivedDamage -= new Action<PlayerController>(this.HandlePlayerDamaged);
      if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && (bool) (UnityEngine.Object) GameManager.Instance.SecondaryPlayer)
        GameManager.Instance.SecondaryPlayer.OnReceivedDamage -= new Action<PlayerController>(this.HandlePlayerDamaged);
      base.OnDestroy();
    }

    private void HandlePlayerDamaged(PlayerController obj)
    {
      if (obj.CurrentRoom != this.GetAbsoluteParentRoom())
        return;
      this.RegenerateBridge();
    }

    private void AddPlatformPosition(
      IntVector2 position,
      List<IntVector2> points,
      HashSet<IntVector2> positions)
    {
      positions.Add(position);
      positions.Add(position + IntVector2.Up);
      positions.Add(position + IntVector2.Right);
      positions.Add(position + IntVector2.One);
      points.Add(position);
    }

    private IntVector2 RotateDir(IntVector2 curDir)
    {
      if (curDir.x > 0)
        return (double) UnityEngine.Random.value < 0.5 ? IntVector2.Up : IntVector2.Down;
      if (curDir.y < 0)
        return (double) UnityEngine.Random.value < 0.5 ? IntVector2.Right : IntVector2.Right;
      if (curDir.y > 0)
        return (double) UnityEngine.Random.value < 0.5 ? IntVector2.Right : IntVector2.Right;
      if (curDir.x >= 0)
        return curDir;
      return (double) UnityEngine.Random.value < 0.5 ? IntVector2.Up : IntVector2.Down;
    }

    private bool IsPositionValid(
      IntVector2 testPosition,
      IntVector2 minPosition,
      IntVector2 maxPosition,
      HashSet<IntVector2> usedPositions)
    {
      return !usedPositions.Contains(testPosition) && testPosition.y >= minPosition.y && testPosition.y < maxPosition.y;
    }

    private MovingPlatform CreateNewPlatform(IntVector2 position)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.InvisiblePlatform2x2.gameObject, position.ToVector3(), Quaternion.identity);
      gameObject.GetComponent<SpeculativeRigidbody>().Reinitialize();
      return gameObject.GetComponent<MovingPlatform>();
    }

    private void RegenerateBridge()
    {
      for (int index = 0; index < StaticReferenceManager.AllDebris.Count; ++index)
      {
        if (StaticReferenceManager.AllDebris[index].transform.position.GetAbsoluteRoom() == this.GetAbsoluteParentRoom())
          StaticReferenceManager.AllDebris[index].ForceUpdatePitfall();
      }
      IntVector2 minPosition = this.transform.position.IntXY(VectorConversions.Floor);
      IntVector2 maxPosition = minPosition + new IntVector2(this.GetWidth(), this.GetHeight());
      int y = UnityEngine.Random.Range(minPosition.y, maxPosition.y - 1);
      List<IntVector2> points = new List<IntVector2>();
      HashSet<IntVector2> intVector2Set = new HashSet<IntVector2>();
      IntVector2 position = new IntVector2(minPosition.x, y);
      IntVector2 curDir = IntVector2.Right;
      this.AddPlatformPosition(position, points, intVector2Set);
      int num = 0;
      IntVector2 intVector2_1;
      for (; maxPosition.x - position.x > 3; position = intVector2_1)
      {
        intVector2_1 = position + curDir + curDir;
        if (!this.IsPositionValid(intVector2_1, minPosition, maxPosition, intVector2Set))
        {
          curDir = IntVector2.Right;
          intVector2_1 = position + curDir + curDir;
          num = 0;
        }
        this.AddPlatformPosition(intVector2_1, points, intVector2Set);
        ++num;
        if (num > 2 && (double) UnityEngine.Random.value < 0.5)
        {
          IntVector2 intVector2_2 = this.RotateDir(curDir);
          if (!this.IsPositionValid(intVector2_1 + intVector2_2 + intVector2_2, minPosition, maxPosition, intVector2Set))
            intVector2_2 = -1 * intVector2_2;
          if (!this.IsPositionValid(intVector2_1 + intVector2_2 + intVector2_2, minPosition, maxPosition, intVector2Set))
            intVector2_2 = curDir;
          if (intVector2_2 != curDir)
            num = 0;
          curDir = intVector2_2;
        }
      }
      for (int index = 0; index < this.m_extantPlatforms.Count; ++index)
      {
        this.m_extantPlatforms[index].ClearCells();
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_extantPlatforms[index].gameObject);
      }
      this.m_extantPlatforms.Clear();
      for (int index = 0; index < points.Count; ++index)
      {
        if (index >= this.m_extantPlatforms.Count)
        {
          this.m_extantPlatforms.Add(this.CreateNewPlatform(points[index]));
        }
        else
        {
          this.m_extantPlatforms[index].ClearCells();
          this.m_extantPlatforms[index].transform.position = points[index].ToVector3();
          this.m_extantPlatforms[index].specRigidbody.Reinitialize();
          this.m_extantPlatforms[index].MarkCells();
        }
      }
      if (this.m_extantPlatforms.Count <= points.Count)
        return;
      for (int index = points.Count; index < this.m_extantPlatforms.Count; index = index - 1 + 1)
      {
        this.m_extantPlatforms[index].ClearCells();
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_extantPlatforms[index].gameObject);
        this.m_extantPlatforms.RemoveAt(index);
      }
    }
  }

