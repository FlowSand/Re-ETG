using Dungeonator;
using System;
using UnityEngine;

#nullable disable

public class NPCCellKeyItem : PickupObject
  {
    private static GameObject m_defaultIcon;
    private bool m_pickedUp;
    private GameObject minimapIcon;
    private GameObject m_instanceMinimapIcon;
    private RoomHandler m_minimapIconRoom;
    [NonSerialized]
    public bool IsBeingDestroyed;
    private bool m_forceExtant;

    private void Start()
    {
      this.specRigidbody.OnTriggerCollision += new SpeculativeRigidbody.OnTriggerDelegate(this.OnPreCollision);
      if (!this.m_pickedUp)
        this.RegisterMinimapIcon();
      SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black);
    }

    private void Update()
    {
      if (this.m_pickedUp || !(bool) (UnityEngine.Object) this || GameManager.Instance.IsAnyPlayerInRoom(this.transform.position.GetAbsoluteRoom()))
        return;
      PlayerController bestActivePlayer = GameManager.Instance.BestActivePlayer;
      if (!(bool) (UnityEngine.Object) bestActivePlayer || bestActivePlayer.IsGhost || !bestActivePlayer.AcceptingAnyInput)
        return;
      this.Pickup(bestActivePlayer);
    }

    public void RegisterMinimapIcon()
    {
      if ((double) this.transform.position.y < -300.0)
        return;
      if ((UnityEngine.Object) this.minimapIcon == (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) NPCCellKeyItem.m_defaultIcon == (UnityEngine.Object) null)
          NPCCellKeyItem.m_defaultIcon = (GameObject) BraveResources.Load("Global Prefabs/Minimap_CellKey_Icon");
        this.minimapIcon = NPCCellKeyItem.m_defaultIcon;
      }
      if (!((UnityEngine.Object) this.minimapIcon != (UnityEngine.Object) null) || this.m_pickedUp)
        return;
      this.m_minimapIconRoom = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.transform.position.IntXY(VectorConversions.Floor));
      this.m_instanceMinimapIcon = Minimap.Instance.RegisterRoomIcon(this.m_minimapIconRoom, this.minimapIcon);
    }

    public void GetRidOfMinimapIcon()
    {
      if (!((UnityEngine.Object) this.m_instanceMinimapIcon != (UnityEngine.Object) null))
        return;
      Minimap.Instance.DeregisterRoomIcon(this.m_minimapIconRoom, this.m_instanceMinimapIcon);
      this.m_instanceMinimapIcon = (GameObject) null;
    }

    private void OnPreCollision(
      SpeculativeRigidbody otherRigidbody,
      SpeculativeRigidbody source,
      CollisionData collisionData)
    {
      if (this.m_pickedUp)
        return;
      PlayerController component = otherRigidbody.GetComponent<PlayerController>();
      if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
        return;
      this.Pickup(component);
      int num = (int) AkSoundEngine.PostEvent("Play_OBJ_goldkey_pickup_01", this.gameObject);
    }

    public void DropLogic()
    {
      this.m_forceExtant = true;
      this.m_pickedUp = false;
    }

    public override void Pickup(PlayerController player)
    {
      if (this.IsBeingDestroyed || this.m_pickedUp)
        return;
      this.m_pickedUp = true;
      SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
      this.GetRidOfMinimapIcon();
      if ((bool) (UnityEngine.Object) this.specRigidbody)
        this.specRigidbody.enabled = false;
      if ((bool) (UnityEngine.Object) this.renderer)
        this.renderer.enabled = false;
      this.HandleEncounterable(player);
      DebrisObject component = this.GetComponent<DebrisObject>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null || this.m_forceExtant)
      {
        if ((bool) (UnityEngine.Object) component)
          UnityEngine.Object.Destroy((UnityEngine.Object) component);
        if ((bool) (UnityEngine.Object) this.specRigidbody)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.specRigidbody);
        player.BloopItemAboveHead(this.sprite, string.Empty);
        player.AcquirePuzzleItem((PickupObject) this);
      }
      else
      {
        UnityEngine.Object.Instantiate<GameObject>(this.gameObject);
        player.BloopItemAboveHead(this.sprite, string.Empty);
        player.AcquirePuzzleItem((PickupObject) this);
      }
      GameUIRoot.Instance.UpdatePlayerConsumables(player.carriedConsumables);
    }

    protected override void OnDestroy()
    {
      base.OnDestroy();
      this.GetRidOfMinimapIcon();
    }
  }

