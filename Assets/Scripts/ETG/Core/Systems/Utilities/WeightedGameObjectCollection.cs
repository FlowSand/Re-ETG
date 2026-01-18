using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

[Serializable]
public class WeightedGameObjectCollection
  {
    public List<WeightedGameObject> elements;

    public WeightedGameObjectCollection() => this.elements = new List<WeightedGameObject>();

    public void Add(WeightedGameObject w) => this.elements.Add(w);

    public float GetTotalWeight()
    {
      float totalWeight = 0.0f;
      for (int index1 = 0; index1 < this.elements.Count; ++index1)
      {
        WeightedGameObject element = this.elements[index1];
        bool flag = true;
        for (int index2 = 0; index2 < element.additionalPrerequisites.Length; ++index2)
        {
          if (!element.additionalPrerequisites[index2].CheckConditionsFulfilled())
          {
            flag = false;
            break;
          }
        }
        if (flag)
          totalWeight += element.weight;
      }
      return totalWeight;
    }

    public GameObject SelectByWeight()
    {
      int outIndex = -1;
      return this.SelectByWeight(out outIndex);
    }

    public GameObject SelectByWeight(out int outIndex, bool useSeedRandom = false)
    {
      outIndex = -1;
      List<WeightedGameObject> weightedGameObjectList = new List<WeightedGameObject>();
      float num1 = 0.0f;
      for (int index1 = 0; index1 < this.elements.Count; ++index1)
      {
        WeightedGameObject element = this.elements[index1];
        bool flag = true;
        if (element.additionalPrerequisites != null)
        {
          for (int index2 = 0; index2 < element.additionalPrerequisites.Length; ++index2)
          {
            if (!element.additionalPrerequisites[index2].CheckConditionsFulfilled())
            {
              flag = false;
              break;
            }
          }
        }
        if ((UnityEngine.Object) element.gameObject != (UnityEngine.Object) null)
        {
          PickupObject component = element.gameObject.GetComponent<PickupObject>();
          if ((UnityEngine.Object) component != (UnityEngine.Object) null && !component.PrerequisitesMet())
            flag = false;
        }
        if (flag)
        {
          weightedGameObjectList.Add(element);
          num1 += element.weight;
        }
      }
      float num2 = (!useSeedRandom ? UnityEngine.Random.value : BraveRandom.GenerationRandomValue()) * num1;
      float num3 = 0.0f;
      for (int index = 0; index < weightedGameObjectList.Count; ++index)
      {
        num3 += weightedGameObjectList[index].weight;
        if ((double) num3 > (double) num2)
        {
          outIndex = this.elements.IndexOf(weightedGameObjectList[index]);
          return weightedGameObjectList[index].gameObject;
        }
      }
      outIndex = this.elements.IndexOf(weightedGameObjectList[weightedGameObjectList.Count - 1]);
      return weightedGameObjectList[weightedGameObjectList.Count - 1].gameObject;
    }

    public GameObject SubshopStyleSelectByWeightWithoutDuplicatesFullPrereqs(
      List<GameObject> extant,
      Func<GameObject, float, float> weightModifier,
      int minElements,
      bool useSeedRandom = false)
    {
      List<WeightedGameObject> weightedGameObjectList = new List<WeightedGameObject>();
      float num1 = 0.0f;
      bool flag1 = useSeedRandom;
      int num2 = 0;
      while (num2 < 2 && weightedGameObjectList.Count < minElements)
      {
        ++num2;
        for (int index1 = 0; index1 < this.elements.Count; ++index1)
        {
          WeightedGameObject element = this.elements[index1];
          if (!((UnityEngine.Object) element.gameObject == (UnityEngine.Object) null) && (extant == null || !extant.Contains(element.gameObject) || element.forceDuplicatesPossible))
          {
            PickupObject component1 = element.gameObject.GetComponent<PickupObject>();
            bool flag2 = true;
            if (component1.quality == PickupObject.ItemQuality.SPECIAL)
            {
              flag2 = false;
              switch (component1)
              {
                case AncientPrimerItem _:
                case ArcaneGunpowderItem _:
                case AstralSlugItem _:
                case ObsidianShellItem _:
                  flag2 = true;
                  break;
              }
            }
            if (!((UnityEngine.Object) component1 != (UnityEngine.Object) null) || component1.PrerequisitesMet() && flag2)
            {
              EncounterTrackable component2 = element.gameObject.GetComponent<EncounterTrackable>();
              if (!((UnityEngine.Object) component2 != (UnityEngine.Object) null) || flag1 || GameStatsManager.Instance.QueryEncounterableDifferentiator(component2) <= 0 || element.forceDuplicatesPossible)
              {
                bool flag3 = true;
                for (int index2 = 0; index2 < element.additionalPrerequisites.Length; ++index2)
                {
                  if (!element.additionalPrerequisites[index2].CheckConditionsFulfilled())
                  {
                    flag3 = false;
                    break;
                  }
                }
                if (flag3)
                {
                  float num3 = weightModifier == null ? element.weight : weightModifier(element.gameObject, element.weight);
                  weightedGameObjectList.Add(element);
                  num1 += num3;
                }
              }
            }
          }
        }
        flag1 = true;
      }
      float num4 = (!useSeedRandom ? UnityEngine.Random.value : BraveRandom.GenerationRandomValue()) * num1;
      float num5 = 0.0f;
      for (int index = 0; index < weightedGameObjectList.Count; ++index)
      {
        float num6 = weightModifier == null ? weightedGameObjectList[index].weight : weightModifier(weightedGameObjectList[index].gameObject, weightedGameObjectList[index].weight);
        num5 += num6;
        if ((double) num5 > (double) num4)
          return weightedGameObjectList[index].gameObject;
      }
      return weightedGameObjectList.Count == 0 ? (GameObject) null : weightedGameObjectList[weightedGameObjectList.Count - 1].gameObject;
    }

    public GameObject SelectByWeightWithoutDuplicatesFullPrereqs(
      List<GameObject> extant,
      Func<GameObject, float, float> weightModifier,
      bool useSeedRandom = false)
    {
      List<WeightedGameObject> weightedGameObjectList = new List<WeightedGameObject>();
      float num1 = 0.0f;
      for (int index1 = 0; index1 < this.elements.Count; ++index1)
      {
        WeightedGameObject element = this.elements[index1];
        if ((UnityEngine.Object) element.gameObject == (UnityEngine.Object) null)
        {
          weightedGameObjectList.Add(element);
          num1 += element.weight;
        }
        else if (extant == null || !extant.Contains(element.gameObject) || element.forceDuplicatesPossible)
        {
          PickupObject component1 = element.gameObject.GetComponent<PickupObject>();
          bool flag1 = true;
          if (component1.quality == PickupObject.ItemQuality.SPECIAL)
          {
            flag1 = false;
            bool flag2 = component1 is SpecialKeyItem && (component1 as SpecialKeyItem).keyType == SpecialKeyItem.SpecialKeyType.RESOURCEFUL_RAT_LAIR;
            switch (component1)
            {
              case AncientPrimerItem _:
              case ArcaneGunpowderItem _:
              case AstralSlugItem _:
              case ObsidianShellItem _:
  label_7:
                flag1 = true;
                break;
              default:
                if (!flag2)
                  break;
                goto label_7;
            }
          }
          if (!((UnityEngine.Object) component1 != (UnityEngine.Object) null) || component1.PrerequisitesMet() && flag1)
          {
            EncounterTrackable component2 = element.gameObject.GetComponent<EncounterTrackable>();
            if (useSeedRandom || !((UnityEngine.Object) component2 != (UnityEngine.Object) null) || GameStatsManager.Instance.QueryEncounterableDifferentiator(component2) <= 0 || element.forceDuplicatesPossible)
            {
              bool flag3 = true;
              for (int index2 = 0; index2 < element.additionalPrerequisites.Length; ++index2)
              {
                if (!element.additionalPrerequisites[index2].CheckConditionsFulfilled())
                {
                  flag3 = false;
                  break;
                }
              }
              if (flag3)
              {
                float num2 = weightModifier == null ? element.weight : weightModifier(element.gameObject, element.weight);
                weightedGameObjectList.Add(element);
                num1 += num2;
              }
            }
          }
        }
      }
      float num3 = (!useSeedRandom ? UnityEngine.Random.value : BraveRandom.GenerationRandomValue()) * num1;
      float num4 = 0.0f;
      for (int index = 0; index < weightedGameObjectList.Count; ++index)
      {
        float num5 = weightModifier == null ? weightedGameObjectList[index].weight : weightModifier(weightedGameObjectList[index].gameObject, weightedGameObjectList[index].weight);
        num4 += num5;
        if ((double) num4 > (double) num3)
          return weightedGameObjectList[index].gameObject;
      }
      return weightedGameObjectList.Count == 0 ? (GameObject) null : weightedGameObjectList[weightedGameObjectList.Count - 1].gameObject;
    }

    public GameObject SelectByWeightWithoutDuplicates(List<GameObject> extant, bool useSeedRandom = false)
    {
      if (extant.Count == this.elements.Count)
        return (GameObject) null;
      List<WeightedGameObject> weightedGameObjectList = new List<WeightedGameObject>();
      float num1 = 0.0f;
      for (int index1 = 0; index1 < this.elements.Count; ++index1)
      {
        WeightedGameObject element = this.elements[index1];
        if (!extant.Contains(element.gameObject))
        {
          bool flag = true;
          for (int index2 = 0; index2 < element.additionalPrerequisites.Length; ++index2)
          {
            if (!element.additionalPrerequisites[index2].CheckConditionsFulfilled())
            {
              flag = false;
              break;
            }
          }
          if (flag)
          {
            weightedGameObjectList.Add(element);
            num1 += element.weight;
          }
        }
      }
      float num2 = (!useSeedRandom ? UnityEngine.Random.value : BraveRandom.GenerationRandomValue()) * num1;
      float num3 = 0.0f;
      for (int index = 0; index < weightedGameObjectList.Count; ++index)
      {
        num3 += weightedGameObjectList[index].weight;
        if ((double) num3 > (double) num2)
          return weightedGameObjectList[index].gameObject;
      }
      return weightedGameObjectList.Count == 0 ? (GameObject) null : weightedGameObjectList[weightedGameObjectList.Count - 1].gameObject;
    }
  }

