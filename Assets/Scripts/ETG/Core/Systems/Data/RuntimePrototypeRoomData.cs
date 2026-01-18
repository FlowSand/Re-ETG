// Decompiled with JetBrains decompiler
// Type: RuntimePrototypeRoomData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class RuntimePrototypeRoomData
  {
    public int RoomId;
    public string GUID;
    public IntVector2 rewardChestSpawnPosition;
    public GameObject associatedMinimapIcon;
    public List<PrototypePlacedObjectData> placedObjects;
    public List<Vector2> placedObjectPositions;
    public List<PrototypeRoomObjectLayer> additionalObjectLayers;
    public List<SerializedPath> paths;
    public List<RoomEventDefinition> roomEvents;
    public bool usesCustomAmbient;
    public Color customAmbient;
    public bool usesDifferentCustomAmbientLowQuality;
    public Color customAmbientLowQuality;
    public bool UsesCustomMusicState;
    public DungeonFloorMusicController.DungeonMusicState CustomMusicState;
    public bool UsesCustomMusic;
    public string CustomMusicEvent;
    public bool UsesCustomSwitch;
    public string CustomMusicSwitch;

    public RuntimePrototypeRoomData(PrototypeDungeonRoom source)
    {
      this.RoomId = source.RoomId;
      this.associatedMinimapIcon = source.associatedMinimapIcon;
      this.placedObjects = source.placedObjects;
      this.placedObjectPositions = source.placedObjectPositions;
      this.additionalObjectLayers = source.runtimeAdditionalObjectLayers ?? source.additionalObjectLayers;
      this.paths = source.paths;
      this.roomEvents = source.roomEvents;
      this.rewardChestSpawnPosition = source.rewardChestSpawnPosition;
      this.usesCustomAmbient = source.usesCustomAmbientLight;
      this.customAmbient = source.customAmbientLight;
      if (this.usesCustomAmbient)
      {
        this.usesDifferentCustomAmbientLowQuality = this.usesCustomAmbient;
        this.customAmbientLowQuality = new Color(this.customAmbient.r + 0.35f, this.customAmbient.g + 0.35f, this.customAmbient.b + 0.35f);
      }
      else
        this.usesDifferentCustomAmbientLowQuality = false;
      this.UsesCustomMusic = source.UseCustomMusic;
      this.CustomMusicEvent = source.CustomMusicEvent;
      this.UsesCustomMusicState = source.UseCustomMusicState;
      this.CustomMusicState = source.OverrideMusicState;
      this.UsesCustomSwitch = source.UseCustomMusicSwitch;
      this.CustomMusicSwitch = source.CustomMusicSwitch;
      this.GUID = source.GUID;
    }

    public bool DoesUnsealOnClear()
    {
      for (int index = 0; index < this.roomEvents.Count; ++index)
      {
        if (this.roomEvents[index].condition == RoomEventTriggerCondition.ON_ENEMIES_CLEARED && this.roomEvents[index].action == RoomEventTriggerAction.UNSEAL_ROOM)
          return true;
      }
      return false;
    }
  }

