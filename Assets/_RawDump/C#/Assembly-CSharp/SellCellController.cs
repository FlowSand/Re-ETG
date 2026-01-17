// Decompiled with JetBrains decompiler
// Type: SellCellController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class SellCellController : DungeonPlaceableBehaviour, IPlaceConfigurable
{
  public float SellValueModifier = 0.1f;
  public TalkDoerLite SellPitDweller;
  public GameObject SellExplosionVFX;
  public tk2dSprite CellTopSprite;
  public string ExplodedSellSpriteName;
  private bool m_isExploded;
  private int m_thingsSold;
  private int m_masteryRoundsSold;
  private bool m_currentlySellingAnItem;
  private float m_timeHovering;

  private void Start()
  {
    if (!(bool) (Object) this.SellPitDweller || !(bool) (Object) this.SellPitDweller.spriteAnimator)
      return;
    this.SellPitDweller.spriteAnimator.alwaysUpdateOffscreen = true;
  }

  public void AttemptSellItem(PickupObject targetItem)
  {
    if (this.m_isExploded || (Object) targetItem == (Object) null || !targetItem.CanBeSold || targetItem.IsBeingSold)
      return;
    switch (targetItem)
    {
      case CurrencyPickup _:
        break;
      case KeyBulletPickup _:
        break;
      case HealthPickup _:
        break;
      default:
        if (!this.specRigidbody.ContainsPoint(targetItem.sprite.WorldCenter, collideWithTriggers: true))
          break;
        this.StartCoroutine(this.HandleSoldItem(targetItem));
        break;
    }
  }

  private void HandleFlightCollider()
  {
    if (GameManager.Instance.IsLoadingLevel || !this.m_isExploded)
      return;
    for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
    {
      PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
      if ((bool) (Object) allPlayer && !allPlayer.IsGhost && allPlayer.IsFlying && new Rect(this.transform.position.XY(), new Vector2(3f, 3f)).Contains(allPlayer.CenterPosition))
      {
        this.m_timeHovering += BraveTime.DeltaTime;
        if ((double) this.m_timeHovering > 2.0)
        {
          allPlayer.ForceFall();
          this.m_timeHovering = 0.0f;
        }
      }
    }
  }

  [DebuggerHidden]
  private IEnumerator HandleSellPitOpening()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new SellCellController.\u003CHandleSellPitOpening\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  private void OnDisable()
  {
    if (!this.m_isExploded || !(this.CellTopSprite.CurrentSprite.name != this.ExplodedSellSpriteName))
      return;
    this.CellTopSprite.SetSprite(this.ExplodedSellSpriteName);
    for (int x = 1; x < this.GetWidth(); ++x)
    {
      for (int y = 0; y < this.GetHeight(); ++y)
      {
        IntVector2 intVector2 = this.transform.position.IntXY() + new IntVector2(x, y);
        if (GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(intVector2))
          GameManager.Instance.Dungeon.data[intVector2].fallingPrevented = false;
      }
    }
  }

  [DebuggerHidden]
  private IEnumerator HandleSoldItem(PickupObject targetItem)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new SellCellController.\u003CHandleSoldItem\u003Ec__Iterator1()
    {
      targetItem = targetItem,
      \u0024this = this
    };
  }

  private void Update() => this.HandleFlightCollider();

  protected override void OnDestroy() => base.OnDestroy();

  public void ConfigureOnPlacement(RoomHandler room)
  {
    for (int x = 1; x < this.GetWidth(); ++x)
    {
      for (int y = 0; y < this.GetHeight(); ++y)
      {
        IntVector2 intVector2 = this.transform.position.IntXY() + new IntVector2(x, y);
        if (GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(intVector2))
        {
          CellData cellData = GameManager.Instance.Dungeon.data[intVector2];
          cellData.type = CellType.PIT;
          cellData.fallingPrevented = true;
        }
      }
    }
  }
}
