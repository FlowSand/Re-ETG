// Decompiled with JetBrains decompiler
// Type: WeightedGameObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
[Serializable]
public class WeightedGameObject
{
  [FormerlySerializedAs("gameObject")]
  public GameObject rawGameObject;
  [PickupIdentifier]
  public int pickupId = -1;
  public float weight;
  public bool forceDuplicatesPossible;
  public DungeonPrerequisite[] additionalPrerequisites;
  [NonSerialized]
  private bool m_hasCachedGameObject;
  [NonSerialized]
  private GameObject m_cachedGameObject;

  public GameObject gameObject
  {
    get
    {
      if (!this.m_hasCachedGameObject)
      {
        if (this.pickupId >= 0)
        {
          PickupObject byId = PickupObjectDatabase.GetById(this.pickupId);
          if ((bool) (UnityEngine.Object) byId)
            this.m_cachedGameObject = byId.gameObject;
        }
        if (!(bool) (UnityEngine.Object) this.m_cachedGameObject)
          this.m_cachedGameObject = this.rawGameObject;
        this.m_hasCachedGameObject = true;
      }
      return this.m_cachedGameObject;
    }
  }

  public void SetGameObject(GameObject gameObject)
  {
    this.m_cachedGameObject = gameObject;
    this.m_hasCachedGameObject = true;
  }

  public void SetGameObjectEditor(GameObject gameObject)
  {
    if ((bool) (UnityEngine.Object) gameObject)
    {
      PickupObject component = gameObject.GetComponent<PickupObject>();
      if ((bool) (UnityEngine.Object) component)
      {
        this.pickupId = component.PickupObjectId;
        this.rawGameObject = (GameObject) null;
        return;
      }
    }
    this.rawGameObject = gameObject;
  }
}
