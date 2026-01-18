using UnityEngine;

#nullable disable

  public static class DepthLookupManager
  {
    public static float DEPTH_RESOLUTION_PER_UNIT = 5f;

    public static void PinRendererToRenderer(Renderer attachment, Renderer target)
    {
      tk2dSprite component = attachment.GetComponent<tk2dSprite>();
      if ((Object) component != (Object) null)
        component.automaticallyManagesDepth = false;
      attachment.sortingLayerName = target.sortingLayerName;
      attachment.sortingOrder = target.sortingOrder;
    }

    public static void ProcessRenderer(Renderer r)
    {
      DepthLookupManager.AssignRendererToSortingLayer(r, DepthLookupManager.GungeonSortingLayer.PLAYFIELD);
      DepthLookupManager.UpdateRenderer(r);
    }

    public static void ProcessRenderer(Renderer r, DepthLookupManager.GungeonSortingLayer l)
    {
      DepthLookupManager.AssignRendererToSortingLayer(r, l);
      DepthLookupManager.UpdateRenderer(r);
    }

    public static void UpdateRenderer(Renderer r)
    {
    }

    public static void UpdateRendererWithWorldYPosition(Renderer r, float worldY)
    {
    }

    public static void AssignSortingOrder(Renderer r, int order)
    {
    }

    public static void AssignRendererToSortingLayer(
      Renderer r,
      DepthLookupManager.GungeonSortingLayer targetLayer)
    {
      string str = string.Empty;
      switch (targetLayer)
      {
        case DepthLookupManager.GungeonSortingLayer.BACKGROUND:
          str = "Background";
          break;
        case DepthLookupManager.GungeonSortingLayer.PLAYFIELD:
          str = "Player";
          break;
        case DepthLookupManager.GungeonSortingLayer.FOREGROUND:
          str = "Foreground";
          break;
        default:
          BraveUtility.Log("Switching on invalid sorting layer in AssignRendererToSortingLayer!", Color.red, BraveUtility.LogVerbosity.IMPORTANT);
          break;
      }
      r.sortingLayerName = str;
    }

    private static void AssignSortingOrderByDepth(Renderer r, float yPosition)
    {
    }

public enum GungeonSortingLayer
    {
      BACKGROUND,
      PLAYFIELD,
      FOREGROUND,
    }
  }

