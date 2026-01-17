// Decompiled with JetBrains decompiler
// Type: GenericRoomTable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Data
{
    public class GenericRoomTable : ScriptableObject
    {
      public WeightedRoomCollection includedRooms;
      public List<GenericRoomTable> includedRoomTables;
      [NonSerialized]
      protected List<WeightedRoom> m_compiledList;
      [NonSerialized]
      protected WeightedRoomCollection m_compiledCollection;

      public WeightedRoom SelectByWeight() => this.GetCompiledCollection().SelectByWeight();

      public WeightedRoom SelectByWeightWithoutDuplicates(List<PrototypeDungeonRoom> extant)
      {
        return this.GetCompiledCollection().SelectByWeightWithoutDuplicates(extant);
      }

      public List<WeightedRoom> GetCompiledList()
      {
        if (this.m_compiledList != null)
          return this.m_compiledList;
        List<WeightedRoom> compiledList = new List<WeightedRoom>();
        for (int index = 0; index < this.includedRooms.elements.Count; ++index)
          compiledList.Add(this.includedRooms.elements[index]);
        for (int index1 = 0; index1 < this.includedRoomTables.Count; ++index1)
        {
          WeightedRoomCollection compiledCollection = this.includedRoomTables[index1].GetCompiledCollection();
          for (int index2 = 0; index2 < compiledCollection.elements.Count; ++index2)
            compiledList.Add(compiledCollection.elements[index2]);
        }
        if (Application.isPlaying)
          this.m_compiledList = compiledList;
        return compiledList;
      }

      protected WeightedRoomCollection GetCompiledCollection()
      {
        WeightedRoomCollection compiledCollection1 = new WeightedRoomCollection();
        for (int index = 0; index < this.includedRooms.elements.Count; ++index)
          compiledCollection1.Add(this.includedRooms.elements[index]);
        for (int index1 = 0; index1 < this.includedRoomTables.Count; ++index1)
        {
          WeightedRoomCollection compiledCollection2 = this.includedRoomTables[index1].GetCompiledCollection();
          for (int index2 = 0; index2 < compiledCollection2.elements.Count; ++index2)
            compiledCollection1.Add(compiledCollection2.elements[index2]);
        }
        this.m_compiledCollection = compiledCollection1;
        return compiledCollection1;
      }
    }

}
