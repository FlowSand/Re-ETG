// Decompiled with JetBrains decompiler
// Type: FoyerInfoPanelController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class FoyerInfoPanelController : MonoBehaviour
    {
      public static bool IsTransitioning;
      private static FoyerInfoPanelController m_extantPanelController;
      public PlayableCharacters characterIdentity;
      public tk2dSprite[] scaledSprites;
      public tk2dSprite arrow;
      public dfPanel textPanel;
      public dfPanel itemsPanel;
      public Transform followTransform;
      public Vector3 offset;
      public Vector3 AdditionalDaveOffset;
      private dfPanel m_panel;

      private void SetBadgeVisibility()
      {
      }

      private void ProcessSprite(dfSprite targetSprite, bool playerHas, bool anyHas)
      {
        if (playerHas)
        {
          targetSprite.IsVisible = true;
          targetSprite.Color = (Color32) Color.white;
        }
        else if (anyHas)
        {
          targetSprite.IsVisible = true;
          targetSprite.Color = (Color32) new Color(0.35f, 0.0f, 0.0f);
        }
        else
          targetSprite.IsVisible = false;
      }

      private bool AnyPlayerElement(int elementIndex) => false;

      [DebuggerHidden]
      private IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new FoyerInfoPanelController.<Start>c__Iterator0()
        {
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleTransition()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new FoyerInfoPanelController.<HandleTransition>c__Iterator1()
        {
          _this = this
        };
      }

      private void Update()
      {
        if (GameManager.Instance.IsPaused && (bool) (Object) this.arrow && this.arrow.transform.childCount > 0)
        {
          MeshRenderer component = this.arrow.transform.GetChild(0).GetComponent<MeshRenderer>();
          if ((bool) (Object) component)
            component.enabled = false;
        }
        for (int index = 0; index < this.scaledSprites.Length; ++index)
          this.scaledSprites[index].transform.localScale = GameUIUtility.GetCurrentTK2D_DFScale(this.m_panel.GetManager()) * Vector3.one;
      }

      private void OnDestroy()
      {
        if (!((Object) FoyerInfoPanelController.m_extantPanelController == (Object) this))
          return;
        FoyerInfoPanelController.m_extantPanelController = (FoyerInfoPanelController) null;
      }

      private void LateUpdate()
      {
        this.transform.position = dfFollowObject.ConvertWorldSpaces(this.followTransform.position + this.offset + this.AdditionalDaveOffset, GameManager.Instance.MainCameraController.Camera, this.m_panel.GUIManager.RenderCamera).WithZ(0.0f);
        this.transform.position = this.transform.position.QuantizeFloor(this.m_panel.PixelsToUnits() / (Pixelator.Instance.ScaleTileScale / Pixelator.Instance.CurrentTileScale));
      }

      [DebuggerHidden]
      private IEnumerator HandleOpen()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new FoyerInfoPanelController.<HandleOpen>c__Iterator2()
        {
          _this = this
        };
      }

      private void ResolutionChangedPanel(
        dfControl newTextPanel,
        Vector3 previousRelativePosition,
        Vector3 newRelativePosition)
      {
        dfLabel component1 = newTextPanel.transform.Find("NameLabel").GetComponent<dfLabel>();
        dfLabel component2 = newTextPanel.transform.Find("DescLabel").GetComponent<dfLabel>();
        dfLabel component3 = newTextPanel.transform.Find("GunLabel").GetComponent<dfLabel>();
        dfLabel component4 = newTextPanel.transform.Find("PastKilledLabel").GetComponent<dfLabel>();
        if (this.characterIdentity == PlayableCharacters.Eevee || this.characterIdentity == PlayableCharacters.Gunslinger || GameStatsManager.Instance.TestPastBeaten(this.characterIdentity))
          component4.IsVisible = true;
        else
          component4.IsVisible = false;
        float currentTileScale = Pixelator.Instance.CurrentTileScale;
        int num1 = Mathf.FloorToInt(currentTileScale);
        tk2dBaseSprite sprite = newTextPanel.Parent.GetComponentsInChildren<CharacterSelectFacecardIdleDoer>(true)[0].sprite;
        newTextPanel.transform.position = sprite.transform.position + new Vector3(18f * currentTileScale * component1.PixelsToUnits(), 41f * currentTileScale * component1.PixelsToUnits(), 0.0f);
        if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.ENGLISH)
        {
          component1.Padding = new RectOffset(2 * num1, 2 * num1, -2 * num1, 0 * num1);
          component2.Padding = new RectOffset(2 * num1, 2 * num1, -2 * num1, 0 * num1);
          component3.Padding = new RectOffset(2 * num1, 2 * num1, -2 * num1, 0 * num1);
          component4.Padding = new RectOffset(2 * num1, 2 * num1, -2 * num1, 0 * num1);
        }
        else
        {
          component1.Padding = new RectOffset(2 * num1, 2 * num1, 0, 0);
          component2.Padding = new RectOffset(2 * num1, 2 * num1, 0, 0);
          component3.Padding = new RectOffset(2 * num1, 2 * num1, 0, 0);
          component4.Padding = new RectOffset(2 * num1, 2 * num1, 0, 0);
        }
        component1.RelativePosition = new Vector3(currentTileScale * 2f, currentTileScale, 0.0f);
        component2.RelativePosition = new Vector3(0.0f, currentTileScale + component1.Size.y, 0.0f) + component1.RelativePosition;
        component3.RelativePosition = new Vector3(0.0f, currentTileScale + component2.Size.y, 0.0f) + component2.RelativePosition;
        component4.RelativePosition = new Vector3(0.0f, currentTileScale + component3.Size.y, 0.0f) + component3.RelativePosition;
        if (!((Object) this.itemsPanel != (Object) null))
          return;
        this.itemsPanel.RelativePosition = component2.RelativePosition;
        List<dfSprite> dfSpriteList = new List<dfSprite>();
        for (int index1 = 0; index1 < this.itemsPanel.Controls.Count; ++index1)
        {
          this.itemsPanel.Controls[index1].RelativePosition = this.itemsPanel.Controls[index1].RelativePosition.WithY(((float) (((double) this.itemsPanel.Height - (double) this.itemsPanel.Controls[index1].Height) / 2.0)).Quantize((float) num1));
          if (dfSpriteList.Count == 0)
          {
            dfSpriteList.Add(this.itemsPanel.Controls[index1] as dfSprite);
          }
          else
          {
            bool flag = false;
            for (int index2 = 0; index2 < dfSpriteList.Count; ++index2)
            {
              if ((double) this.itemsPanel.Controls[index1].RelativePosition.x < (double) dfSpriteList[index2].RelativePosition.x)
              {
                dfSpriteList.Insert(index2, this.itemsPanel.Controls[index1] as dfSprite);
                flag = true;
                break;
              }
            }
            if (!flag)
              dfSpriteList.Add(this.itemsPanel.Controls[index1] as dfSprite);
          }
        }
        this.itemsPanel.CenterChildControls();
        float num2 = 0.0f;
        for (int index = 0; index < dfSpriteList.Count; ++index)
        {
          if (index == 0)
          {
            dfSpriteList[index].RelativePosition = dfSpriteList[index].RelativePosition.WithX((float) (num1 * 4));
          }
          else
          {
            dfSprite dfSprite = dfSpriteList[index];
            dfSprite.RelativePosition = dfSprite.RelativePosition.WithX(dfSpriteList[index - 1].RelativePosition.x + dfSpriteList[index - 1].Size.x + (float) (num1 * 4));
          }
          dfSpriteList[index].RelativePosition = dfSpriteList[index].RelativePosition.Quantize((float) num1);
          num2 = dfSpriteList[index].RelativePosition.x + dfSpriteList[index].Size.x + (float) (num1 * 4);
        }
        this.itemsPanel.Width = num2;
        component4.RelativePosition = component1.RelativePosition + new Vector3(component1.Width + (float) num1, 0.0f, 0.0f);
        if (!component4.Text.StartsWith("("))
          component4.Text = $"({component4.Text})";
        component4.Color = (Color32) new Color(0.6f, 0.6f, 0.6f);
      }
    }

}
