// Decompiled with JetBrains decompiler
// Type: BasicInteractItemGiver
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable

public class BasicInteractItemGiver : BraveBehaviour, IPlayerInteractable
  {
    [PickupIdentifier]
    public int pickupIdToGive = -1;
    public GungeonFlags flagToSetOnAcquisition;
    public bool destroyThisOnAcquisition;
    private bool m_pickedUp;
    private RoomHandler m_room;

    private void Start()
    {
      this.m_room = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.transform.position.IntXY());
      this.m_room.RegisterInteractable((IPlayerInteractable) this);
      SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black);
    }

    public float GetDistanceToPoint(Vector2 point)
    {
      Bounds bounds = this.sprite.GetBounds();
      bounds.SetMinMax(bounds.min + this.transform.position, bounds.max + this.transform.position);
      float num1 = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
      float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
      return Mathf.Sqrt((float) (((double) point.x - (double) num1) * ((double) point.x - (double) num1) + ((double) point.y - (double) num2) * ((double) point.y - (double) num2)));
    }

    public void OnEnteredRange(PlayerController interactor)
    {
      SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite, true);
      SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.white);
    }

    public void OnExitRange(PlayerController interactor)
    {
      SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite, true);
      SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black);
    }

    public void Interact(PlayerController interactor)
    {
      if (this.m_pickedUp)
        return;
      if (this.flagToSetOnAcquisition != GungeonFlags.NONE)
        GameStatsManager.Instance.SetFlag(this.flagToSetOnAcquisition, true);
      this.m_pickedUp = true;
      this.m_room.DeregisterInteractable((IPlayerInteractable) this);
      LootEngine.TryGivePrefabToPlayer(PickupObjectDatabase.GetById(this.pickupIdToGive).gameObject, interactor, true);
      if (!this.destroyThisOnAcquisition)
        return;
      SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
      Object.Destroy((Object) this.gameObject);
    }

    public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
    {
      shouldBeFlipped = false;
      return string.Empty;
    }

    public float GetOverrideMaxDistance() => -1f;

    protected override void OnDestroy() => base.OnDestroy();
  }

