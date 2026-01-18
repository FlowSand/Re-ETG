using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class GenericLootTable : ScriptableObject
  {
    public WeightedGameObjectCollection defaultItemDrops;
    public List<GenericLootTable> includedLootTables;
    public DungeonPrerequisite[] tablePrerequisites;
    private WeightedGameObjectCollection m_compiledCollection;

    public bool RawContains(GameObject g)
    {
      for (int index = 0; index < this.defaultItemDrops.elements.Count; ++index)
      {
        if ((UnityEngine.Object) this.defaultItemDrops.elements[index].gameObject == (UnityEngine.Object) g)
          return true;
      }
      return false;
    }

    public GameObject SelectByWeight(bool useSeedRandom = false)
    {
      return this.GetCompiledCollection().SelectByWeight();
    }

    public GameObject SelectByWeightWithoutDuplicates(List<GameObject> extant, bool useSeedRandom = false)
    {
      return this.GetCompiledCollection().SelectByWeightWithoutDuplicates(extant, useSeedRandom);
    }

    public GameObject SelectByWeightWithoutDuplicatesFullPrereqs(
      List<GameObject> extant,
      bool allowSpice = true,
      bool useSeedRandom = false)
    {
      return this.GetCompiledCollection(allowSpice).SelectByWeightWithoutDuplicatesFullPrereqs(extant, (Func<GameObject, float, float>) null, useSeedRandom);
    }

    public GameObject SubshopSelectByWeightWithoutDuplicatesFullPrereqs(
      List<GameObject> extant,
      Func<GameObject, float, float> weightModifier,
      int minElements,
      bool useSeedRandom = false)
    {
      return this.GetCompiledCollection().SubshopStyleSelectByWeightWithoutDuplicatesFullPrereqs(extant, weightModifier, minElements, useSeedRandom);
    }

    public GameObject SelectByWeightWithoutDuplicatesFullPrereqs(
      List<GameObject> extant,
      Func<GameObject, float, float> weightModifier,
      bool useSeedRandom = false)
    {
      return this.GetCompiledCollection().SelectByWeightWithoutDuplicatesFullPrereqs(extant, weightModifier, useSeedRandom);
    }

    public List<WeightedGameObject> GetCompiledRawItems() => this.GetCompiledCollection().elements;

    protected WeightedGameObjectCollection GetCompiledCollection(bool allowSpice = true)
    {
      int spiceCount = 0;
      if (allowSpice && Application.isPlaying && (UnityEngine.Object) GameManager.Instance.PrimaryPlayer != (UnityEngine.Object) null)
      {
        spiceCount = GameManager.Instance.PrimaryPlayer.spiceCount;
        if ((UnityEngine.Object) GameManager.Instance.SecondaryPlayer != (UnityEngine.Object) null)
          spiceCount += GameManager.Instance.SecondaryPlayer.spiceCount;
      }
      if (this.includedLootTables.Count == 0 && spiceCount == 0)
        return this.defaultItemDrops;
      WeightedGameObjectCollection compiledCollection1 = new WeightedGameObjectCollection();
      for (int index = 0; index < this.defaultItemDrops.elements.Count; ++index)
        compiledCollection1.Add(this.defaultItemDrops.elements[index]);
      for (int index1 = 0; index1 < this.includedLootTables.Count; ++index1)
      {
        if (this.includedLootTables[index1].tablePrerequisites.Length > 0)
        {
          bool flag = false;
          for (int index2 = 0; index2 < this.includedLootTables[index1].tablePrerequisites.Length; ++index2)
          {
            if (!this.includedLootTables[index1].tablePrerequisites[index2].CheckConditionsFulfilled())
            {
              flag = true;
              break;
            }
          }
          if (flag)
            continue;
        }
        WeightedGameObjectCollection compiledCollection2 = this.includedLootTables[index1].GetCompiledCollection();
        for (int index3 = 0; index3 < compiledCollection2.elements.Count; ++index3)
          compiledCollection1.Add(compiledCollection2.elements[index3]);
      }
      if (allowSpice && spiceCount > 0)
      {
        float totalWeight = compiledCollection1.GetTotalWeight();
        float num = SpiceItem.GetSpiceWeight(spiceCount) * totalWeight;
        GameObject gameObject = PickupObjectDatabase.GetById(GlobalItemIds.Spice).gameObject;
        WeightedGameObject w = new WeightedGameObject();
        w.SetGameObject(gameObject);
        w.weight = num;
        w.additionalPrerequisites = new DungeonPrerequisite[0];
        compiledCollection1.Add(w);
      }
      return compiledCollection1;
    }
  }

