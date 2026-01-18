// Decompiled with JetBrains decompiler
// Type: PrototypePlacedObjectData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable

[Serializable]
public class PrototypePlacedObjectData
  {
    public DungeonPlaceable placeableContents;
    [FormerlySerializedAs("behaviourContents")]
    public DungeonPlaceableBehaviour nonenemyBehaviour;
    public string enemyBehaviourGuid;
    public GameObject unspecifiedContents;
    public Vector2 contentsBasePosition;
    public int layer;
    public float spawnChance = 1f;
    public int xMPxOffset;
    public int yMPxOffset;
    public List<PrototypePlacedObjectFieldData> fieldData;
    public DungeonPrerequisite[] instancePrerequisites;
    public List<int> linkedTriggerAreaIDs;
    public int assignedPathIDx = -1;
    public int assignedPathStartNode;

    public bool IsEnemyForReinforcementLayerCheck()
    {
      return (!((UnityEngine.Object) this.placeableContents != (UnityEngine.Object) null) || this.placeableContents.ContainsEnemy || this.placeableContents.ContainsEnemylikeObjectForReinforcement) && !((UnityEngine.Object) this.nonenemyBehaviour != (UnityEngine.Object) null);
    }

    public bool GetBoolFieldValueByName(string name)
    {
      if (this.fieldData == null)
        return false;
      for (int index = 0; index < this.fieldData.Count; ++index)
      {
        if (this.fieldData[index].fieldName == name)
          return this.fieldData[index].boolValue;
      }
      return false;
    }

    public float GetFieldValueByName(string name)
    {
      for (int index = 0; index < this.fieldData.Count; ++index)
      {
        if (this.fieldData[index].fieldName == name)
          return this.fieldData[index].floatValue;
      }
      return 1f;
    }

    public int GetWidth(bool accountForFieldData = false)
    {
      if (accountForFieldData && this.fieldData != null)
      {
        for (int index = 0; index < this.fieldData.Count; ++index)
        {
          if (this.fieldData[index].fieldName == "DwarfConfigurableWidth")
            return Mathf.RoundToInt(this.GetFieldValueByName("DwarfConfigurableWidth"));
        }
      }
      if ((UnityEngine.Object) this.placeableContents != (UnityEngine.Object) null)
        return this.placeableContents.width;
      if ((UnityEngine.Object) this.nonenemyBehaviour != (UnityEngine.Object) null)
        return this.nonenemyBehaviour.placeableWidth;
      return !string.IsNullOrEmpty(this.enemyBehaviourGuid) ? EnemyDatabase.GetEntry(this.enemyBehaviourGuid).placeableWidth : 1;
    }

    public int GetHeight(bool accountForFieldData = false)
    {
      if (accountForFieldData && this.fieldData != null)
      {
        for (int index = 0; index < this.fieldData.Count; ++index)
        {
          if (this.fieldData[index].fieldName == "DwarfConfigurableHeight")
            return Mathf.RoundToInt(this.GetFieldValueByName("DwarfConfigurableHeight"));
        }
      }
      if ((UnityEngine.Object) this.placeableContents != (UnityEngine.Object) null)
        return this.placeableContents.height;
      if ((UnityEngine.Object) this.nonenemyBehaviour != (UnityEngine.Object) null)
        return this.nonenemyBehaviour.placeableHeight;
      return !string.IsNullOrEmpty(this.enemyBehaviourGuid) ? EnemyDatabase.GetEntry(this.enemyBehaviourGuid).placeableHeight : 1;
    }

    public PrototypePlacedObjectData CreateMirror(IntVector2 roomDimensions)
    {
      PrototypePlacedObjectData mirror = new PrototypePlacedObjectData()
      {
        placeableContents = this.placeableContents,
        nonenemyBehaviour = this.nonenemyBehaviour,
        enemyBehaviourGuid = this.enemyBehaviourGuid,
        unspecifiedContents = this.unspecifiedContents,
        contentsBasePosition = this.contentsBasePosition
      };
      mirror.contentsBasePosition.x = (float) roomDimensions.x - (mirror.contentsBasePosition.x + (float) this.GetWidth(true));
      mirror.layer = this.layer;
      mirror.spawnChance = this.spawnChance;
      mirror.xMPxOffset = this.xMPxOffset;
      mirror.yMPxOffset = this.yMPxOffset;
      mirror.fieldData = new List<PrototypePlacedObjectFieldData>();
      for (int index = 0; index < this.fieldData.Count; ++index)
        mirror.fieldData.Add(new PrototypePlacedObjectFieldData()
        {
          boolValue = this.fieldData[index].boolValue,
          fieldName = this.fieldData[index].fieldName,
          fieldType = this.fieldData[index].fieldType,
          floatValue = this.fieldData[index].floatValue
        });
      mirror.instancePrerequisites = new DungeonPrerequisite[this.instancePrerequisites.Length];
      for (int index = 0; index < this.instancePrerequisites.Length; ++index)
        mirror.instancePrerequisites[index] = this.instancePrerequisites[index];
      mirror.assignedPathIDx = this.assignedPathIDx;
      mirror.assignedPathStartNode = this.assignedPathStartNode;
      mirror.linkedTriggerAreaIDs = new List<int>();
      for (int index = 0; index < this.linkedTriggerAreaIDs.Count; ++index)
        mirror.linkedTriggerAreaIDs.Add(this.linkedTriggerAreaIDs[index]);
      return mirror;
    }
  }

