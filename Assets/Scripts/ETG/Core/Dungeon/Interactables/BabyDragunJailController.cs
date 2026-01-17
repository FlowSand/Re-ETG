// Decompiled with JetBrains decompiler
// Type: BabyDragunJailController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    public class BabyDragunJailController : DungeonPlaceableBehaviour, IPlayerInteractable
    {
      public tk2dSprite CagedBabyDragun;
      public Transform CagedBabyDragunTalkPoint;
      public SpeculativeRigidbody SellRegionRigidbody;
      public int RequiredItems = 2;
      [PickupIdentifier]
      public int ItemID;
      private bool m_isOpen;
      private RoomHandler m_room;
      private int m_itemsEaten;
      private bool m_currentlySellingAnItem;

      private void Start()
      {
        this.m_isOpen = true;
        this.m_room = this.transform.position.GetAbsoluteRoom();
        this.m_room.RegisterInteractable((IPlayerInteractable) this);
      }

      private void Update()
      {
        if (Dungeon.IsGenerating)
          return;
        bool flag = false;
        for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        {
          if (GameManager.Instance.AllPlayers[index].CurrentRoom == this.m_room)
          {
            flag = true;
            break;
          }
        }
        if (!flag || this.m_itemsEaten >= this.RequiredItems)
          return;
        for (int index = 0; index < StaticReferenceManager.AllDebris.Count; ++index)
        {
          DebrisObject allDebri = StaticReferenceManager.AllDebris[index];
          if ((bool) (Object) allDebri && allDebri.IsPickupObject && allDebri.Static)
          {
            PickupObject componentInChildren = allDebri.GetComponentInChildren<PickupObject>();
            if ((bool) (Object) componentInChildren && !(componentInChildren is GungeonEggItem))
              this.AttemptSellItem(componentInChildren);
          }
        }
        if (this.m_currentlySellingAnItem)
          return;
        for (int index = 0; index < StaticReferenceManager.AllNpcs.Count; ++index)
        {
          TalkDoerLite allNpc = StaticReferenceManager.AllNpcs[index];
          if ((bool) (Object) allNpc && allNpc.name.Contains("ResourcefulRat_Beaten") && (double) (allNpc.specRigidbody.UnitCenter - this.CagedBabyDragun.WorldCenter).magnitude < 3.0)
          {
            RoomHandler.unassignedInteractableObjects.Remove((IPlayerInteractable) allNpc);
            this.StartCoroutine(this.EatCorpse(allNpc));
          }
        }
      }

      [DebuggerHidden]
      private IEnumerator EatCorpse(TalkDoerLite targetCorpse)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BabyDragunJailController.<EatCorpse>c__Iterator0()
        {
          targetCorpse = targetCorpse,
          _this = this
        };
      }

      public void AttemptSellItem(PickupObject targetItem)
      {
        if ((Object) targetItem == (Object) null || !targetItem.CanBeSold || targetItem.IsBeingSold)
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
            if (this.m_itemsEaten >= this.RequiredItems || this.m_currentlySellingAnItem || !this.SellRegionRigidbody.ContainsPoint(targetItem.sprite.WorldCenter, collideWithTriggers: true))
              break;
            this.StartCoroutine(this.HandleSoldItem(targetItem));
            break;
        }
      }

      [DebuggerHidden]
      private IEnumerator HandleSoldItem(PickupObject targetItem)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BabyDragunJailController.<HandleSoldItem>c__Iterator1()
        {
          targetItem = targetItem,
          _this = this
        };
      }

      private void Talk(PlayerController interactor)
      {
        string key = this.m_itemsEaten != 0 ? "#BABYDRAGUN_FED_ONCE" : "#BABYDRAGUN_UNFED";
        TextBoxManager.ShowThoughtBubble((Vector3) (interactor.sprite.WorldTopCenter + new Vector2(0.0f, 0.5f)), interactor.transform, 3f, StringTableManager.GetString(key), overrideAudioTag: string.Empty);
      }

      public float GetDistanceToPoint(Vector2 point)
      {
        if (!this.m_isOpen)
          return 100f;
        Vector3 b = (Vector3) BraveMathCollege.ClosestPointOnRectangle(point, this.CagedBabyDragun.WorldBottomLeft, this.CagedBabyDragun.WorldTopRight - this.CagedBabyDragun.WorldBottomLeft);
        return Vector2.Distance(point, (Vector2) b) / 1.5f;
      }

      public void OnEnteredRange(PlayerController interactor)
      {
        SpriteOutlineManager.AddOutlineToSprite((tk2dBaseSprite) this.CagedBabyDragun, Color.white);
      }

      public void OnExitRange(PlayerController interactor)
      {
        if (!SpriteOutlineManager.HasOutline((tk2dBaseSprite) this.CagedBabyDragun))
          return;
        TextBoxManager.ClearTextBox(interactor.transform);
        SpriteOutlineManager.RemoveOutlineFromSprite((tk2dBaseSprite) this.CagedBabyDragun);
      }

      public void Interact(PlayerController interactor) => this.Talk(interactor);

      public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
      {
        shouldBeFlipped = false;
        return string.Empty;
      }

      public float GetOverrideMaxDistance() => -1f;

      protected override void OnDestroy()
      {
        this.m_room.DeregisterInteractable((IPlayerInteractable) this);
        base.OnDestroy();
      }
    }

}
