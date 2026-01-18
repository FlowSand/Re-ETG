// Decompiled with JetBrains decompiler
// Type: GungeonMapItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable

public class GungeonMapItem : PassiveItem
  {
    public override void Pickup(PlayerController player)
    {
      if (this.m_pickedUp)
        return;
      if (RoomHandler.unassignedInteractableObjects.Contains((IPlayerInteractable) this))
        RoomHandler.unassignedInteractableObjects.Remove((IPlayerInteractable) this);
      this.m_pickedUp = true;
      if (!this.m_pickedUpThisRun)
        this.HandleEncounterable(player);
      tk2dSprite component = Object.Instantiate<GameObject>((GameObject) BraveResources.Load("Global VFX/VFX_Item_Pickup", typeof (GameObject))).GetComponent<tk2dSprite>();
      component.PlaceAtPositionByAnchor((Vector3) this.sprite.WorldCenter, tk2dBaseSprite.Anchor.MiddleCenter);
      component.UpdateZDepth();
      this.m_pickedUpThisRun = true;
      player.EverHadMap = true;
      player.stats.RecalculateStats(player);
      if ((Object) Minimap.Instance != (Object) null)
        Minimap.Instance.RevealAllRooms(true);
      Object.Destroy((Object) this.gameObject);
    }

    public override DebrisObject Drop(PlayerController player)
    {
      Debug.LogError((object) "IT SHOULD BE IMPOSSIBLE TO DROP MAPS.");
      DebrisObject debrisObject = base.Drop(player);
      debrisObject.GetComponent<GungeonMapItem>().m_pickedUpThisRun = true;
      return debrisObject;
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

