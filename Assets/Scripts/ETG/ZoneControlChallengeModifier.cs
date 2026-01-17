// Decompiled with JetBrains decompiler
// Type: ZoneControlChallengeModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using Pathfinding;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class ZoneControlChallengeModifier : ChallengeModifier
{
  public DungeonPlaceable BoxPlaceable;
  public float AuraRadius = 5f;
  public float WinTimer = 10f;
  public float DecayScale = 3f / 16f;
  public int MinBoxes = 2;
  public int ExtraBoxAboveArea = 60;
  public int ExtraBoxEveryArea = 30;
  private FlippableCover[] m_instanceBox;
  private float m_timeElapsed;

  private void Start()
  {
    RoomHandler currentRoom = GameManager.Instance.PrimaryPlayer.CurrentRoom;
    int minBoxes = this.MinBoxes;
    for (int index = currentRoom.Cells.Count - this.ExtraBoxAboveArea; index > 0; index -= this.ExtraBoxEveryArea)
      ++minBoxes;
    int length = Mathf.Clamp(minBoxes, this.MinBoxes, 11);
    this.m_instanceBox = new FlippableCover[length];
    CellValidator cellValidator = (CellValidator) (c =>
    {
      CellData cellData = GameManager.Instance.Dungeon.data[c];
      if (cellData == null || cellData.containsTrap || cellData.isOccupied)
        return false;
      for (int index = 0; index < this.m_instanceBox.Length; ++index)
      {
        if ((Object) this.m_instanceBox[index] != (Object) null && (double) Vector2.Distance(this.m_instanceBox[index].specRigidbody.UnitCenter, c.ToCenterVector2()) < 5.0)
          return false;
      }
      return true;
    });
    for (int index = 0; index < length; ++index)
    {
      IntVector2? randomAvailableCell = currentRoom.GetRandomAvailableCell(new IntVector2?(new IntVector2(4, 4)), new CellTypes?(CellTypes.FLOOR), cellValidator: cellValidator);
      if (randomAvailableCell.HasValue)
      {
        GameObject gameObject = this.BoxPlaceable.InstantiateObject(currentRoom, randomAvailableCell.Value + IntVector2.One - currentRoom.area.basePosition);
        this.m_instanceBox[index] = gameObject.GetComponent<FlippableCover>();
        this.m_instanceBox[index].GetComponentInChildren<tk2dSpriteAnimator>().Play("moving_box_in");
        PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(this.m_instanceBox[index].specRigidbody);
      }
    }
  }

  private void UpdateAnimation(tk2dSpriteAnimator anim, bool playerRadius)
  {
    if (anim.IsPlaying("moving_box_in") || anim.IsPlaying("moving_box_out"))
      return;
    if (playerRadius && !anim.IsPlaying("moving_box_open"))
      anim.Play("moving_box_open");
    if (playerRadius || anim.IsPlaying("moving_box_close"))
      return;
    anim.Play("moving_box_close");
  }

  private void Update()
  {
    bool flag = false;
    for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      GameManager.Instance.AllPlayers[index].IsGunLocked = true;
    for (int index1 = 0; index1 < this.m_instanceBox.Length; ++index1)
    {
      if ((bool) (Object) this.m_instanceBox[index1])
      {
        flag = true;
        bool playerRadius = false;
        for (int index2 = 0; index2 < GameManager.Instance.AllPlayers.Length; ++index2)
        {
          PlayerController allPlayer = GameManager.Instance.AllPlayers[index2];
          if ((double) Vector2.Distance(this.m_instanceBox[index1].specRigidbody.UnitCenter, allPlayer.CenterPosition) < (double) this.AuraRadius)
          {
            allPlayer.IsGunLocked = false;
            this.m_timeElapsed = Mathf.Clamp(this.m_timeElapsed + BraveTime.DeltaTime, 0.0f, this.WinTimer + 1f);
            playerRadius = true;
          }
          else
            this.m_timeElapsed = Mathf.Clamp(this.m_timeElapsed - BraveTime.DeltaTime * this.DecayScale, 0.0f, this.WinTimer + 1f);
        }
        this.UpdateAnimation(this.m_instanceBox[index1].spriteAnimator, playerRadius);
      }
    }
    if (!flag)
    {
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        GameManager.Instance.AllPlayers[index].IsGunLocked = false;
    }
    float num = Mathf.Lerp(0.01f, 1f, this.m_timeElapsed / this.WinTimer);
    for (int index = 0; index < this.m_instanceBox.Length; ++index)
    {
      if ((bool) (Object) this.m_instanceBox[index])
        this.m_instanceBox[index].outlineEast.GetComponent<tk2dSprite>().scale = new Vector3(num, num, num);
    }
    if ((double) this.m_timeElapsed < (double) this.WinTimer)
      return;
    this.PopBox();
  }

  private void PopBox()
  {
    if (!GameManager.HasInstance || !(bool) (Object) GameManager.Instance.Dungeon)
      return;
    for (int index = 0; index < this.m_instanceBox.Length; ++index)
    {
      if ((bool) (Object) this.m_instanceBox[index])
      {
        GameManager.Instance.Dungeon.StartCoroutine(this.HandleBoxPop(this.m_instanceBox[index]));
        this.m_instanceBox[index] = (FlippableCover) null;
      }
    }
  }

  [DebuggerHidden]
  private IEnumerator HandleBoxPop(FlippableCover box)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ZoneControlChallengeModifier.\u003CHandleBoxPop\u003Ec__Iterator0()
    {
      box = box
    };
  }

  private void OnDestroy()
  {
    for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      GameManager.Instance.AllPlayers[index].IsGunLocked = false;
    this.PopBox();
  }
}
