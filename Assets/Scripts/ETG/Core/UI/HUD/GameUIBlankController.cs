// Decompiled with JetBrains decompiler
// Type: GameUIBlankController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.UI.HUD
{
    public class GameUIBlankController : MonoBehaviour, ILevelLoadedListener
    {
      public dfSprite blankSpritePrefab;
      public List<dfSprite> extantBlanks;
      public bool IsRightAligned;
      private dfPanel m_panel;

      public dfPanel Panel => this.m_panel;

      private void Awake()
      {
        this.m_panel = this.GetComponent<dfPanel>();
        this.extantBlanks = new List<dfSprite>();
      }

      private void Start()
      {
        this.m_panel.IsInteractive = false;
        foreach (Object component in this.GetComponents<Collider>())
          Object.Destroy(component);
      }

      public void BraveOnLevelWasLoaded()
      {
        if (this.extantBlanks == null)
          return;
        this.extantBlanks.Clear();
      }

      public void UpdateScale()
      {
        for (int index = 0; index < this.extantBlanks.Count; ++index)
        {
          dfSprite extantBlank = this.extantBlanks[index];
          if ((bool) (Object) extantBlank)
          {
            Vector2 sizeInPixels = extantBlank.SpriteInfo.sizeInPixels;
            extantBlank.Size = sizeInPixels * Pixelator.Instance.CurrentTileScale;
          }
        }
      }

      public dfSprite AddBlank()
      {
        GameObject gameObject = Object.Instantiate<GameObject>(this.blankSpritePrefab.gameObject, this.transform.position, Quaternion.identity);
        gameObject.transform.parent = this.transform.parent;
        gameObject.layer = this.gameObject.layer;
        dfSprite component = gameObject.GetComponent<dfSprite>();
        if (this.IsRightAligned)
          component.Anchor = dfAnchorStyle.Top | dfAnchorStyle.Right;
        Vector2 sizeInPixels = component.SpriteInfo.sizeInPixels;
        component.Size = sizeInPixels * Pixelator.Instance.CurrentTileScale;
        if (!this.IsRightAligned)
        {
          float x = (component.Width + Pixelator.Instance.CurrentTileScale) * (float) this.extantBlanks.Count;
          component.RelativePosition = this.m_panel.RelativePosition + new Vector3(x, 0.0f, 0.0f);
        }
        else
        {
          component.RelativePosition = this.m_panel.RelativePosition - new Vector3(component.Width, 0.0f, 0.0f);
          for (int index = 0; index < this.extantBlanks.Count; ++index)
          {
            dfSprite extantBlank = this.extantBlanks[index];
            if ((bool) (Object) extantBlank)
            {
              extantBlank.RelativePosition = extantBlank.RelativePosition + new Vector3((float) (-1.0 * ((double) component.Width + (double) Pixelator.Instance.CurrentTileScale)), 0.0f, 0.0f);
              GameUIRoot.Instance.UpdateControlMotionGroup((dfControl) extantBlank);
            }
          }
        }
        component.IsInteractive = false;
        this.extantBlanks.Add(component);
        GameUIRoot.Instance.AddControlToMotionGroups((dfControl) component, !this.IsRightAligned ? DungeonData.Direction.WEST : DungeonData.Direction.EAST);
        return component;
      }

      public void RemoveBlank()
      {
        if (this.extantBlanks.Count <= 0)
          return;
        float width = this.extantBlanks[0].Width;
        dfSprite extantBlank1 = this.extantBlanks[this.extantBlanks.Count - 1];
        if ((bool) (Object) extantBlank1)
        {
          GameUIRoot.Instance.RemoveControlFromMotionGroups((dfControl) extantBlank1);
          Object.Destroy((Object) extantBlank1);
        }
        this.extantBlanks.RemoveAt(this.extantBlanks.Count - 1);
        if (!this.IsRightAligned)
          return;
        for (int index = 0; index < this.extantBlanks.Count; ++index)
        {
          dfSprite extantBlank2 = this.extantBlanks[index];
          if ((bool) (Object) extantBlank2)
          {
            extantBlank2.RelativePosition = extantBlank2.RelativePosition + new Vector3(width + Pixelator.Instance.CurrentTileScale, 0.0f, 0.0f);
            GameUIRoot.Instance.UpdateControlMotionGroup((dfControl) extantBlank2);
          }
        }
      }

      public void UpdateBlanks(int numBlanks)
      {
        if (GameManager.Instance.IsLoadingLevel || (double) UnityEngine.Time.timeSinceLevelLoad < 0.0099999997764825821)
          return;
        int num = numBlanks;
        if (this.extantBlanks.Count < num)
        {
          for (int count = this.extantBlanks.Count; count < num; ++count)
            this.AddBlank();
        }
        else
        {
          if (this.extantBlanks.Count <= num)
            return;
          while (this.extantBlanks.Count > num)
            this.RemoveBlank();
        }
      }
    }

}
