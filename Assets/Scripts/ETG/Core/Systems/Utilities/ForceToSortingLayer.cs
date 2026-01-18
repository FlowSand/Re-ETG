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

