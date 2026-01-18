// Decompiled with JetBrains decompiler
// Type: ForceToSortingLayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class ForceToSortingLayer : MonoBehaviour
  {
    public DepthLookupManager.GungeonSortingLayer sortingLayer;
    public int targetSortingOrder = -1;

    private void OnEnable()
    {
      DepthLookupManager.AssignRendererToSortingLayer(this.GetComponent<Renderer>(), this.sortingLayer);
      if (this.targetSortingOrder == -1)
        return;
      DepthLookupManager.UpdateRendererWithWorldYPosition(this.GetComponent<Renderer>(), this.transform.position.y);
    }
  }

