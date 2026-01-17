// Decompiled with JetBrains decompiler
// Type: GameUIReloadBarController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class GameUIReloadBarController : BraveBehaviour
    {
      public dfSprite activeReloadSprite;
      public dfSlider progressSlider;
      public dfSprite celebrationSprite;
      public int startValue;
      public int endValue;
      public int lieFactor = 3;
      public dfFollowObject follower;
      public float initialDelay = 0.1f;
      public float finalDelay = 0.25f;
      [Header("Status Bars")]
      public dfProgressBar statusBarDrain;
      public dfProgressBar statusBarPoison;
      public dfProgressBar statusBarFire;
      public dfProgressBar statusBarCurse;
      private int m_activeReloadStartValue;
      private int m_activeReloadEndValue;
      private bool m_reloadIsActive;
      private Vector3 m_worldOffset;
      private Vector3 m_screenOffset;
      private PlayerController m_attachPlayer;
      private Camera worldCamera;
      private Camera uiCamera;
      private dfPanel StatusBarPanel;
      private OverridableBool m_isInvisible = new OverridableBool(false);
      private bool m_isReloading;

      public bool ReloadIsActive => this.m_reloadIsActive;

      private void Awake()
      {
        if (!((UnityEngine.Object) this.statusBarDrain != (UnityEngine.Object) null))
          return;
        this.StatusBarPanel = this.statusBarDrain.Parent as dfPanel;
      }

      private void Start()
      {
        GameManager.Instance.MainCameraController.OnFinishedFrame += new System.Action(this.OnMainCameraFinishedFrame);
      }

      public void SetInvisibility(bool visible, string reason)
      {
        this.m_isInvisible.SetOverride(reason, visible);
      }

      public void TriggerReload(
        PlayerController attachParent,
        Vector3 offset,
        float duration,
        float activeReloadStartPercent,
        int pixelWidth)
      {
        this.progressSlider.transform.localScale = Vector3.one / GameUIRoot.GameUIScalar;
        this.progressSlider.IsVisible = true;
        this.m_attachPlayer = attachParent;
        this.worldCamera = GameManager.Instance.MainCameraController.GetComponent<Camera>();
        this.uiCamera = this.progressSlider.GetManager().RenderCamera;
        this.m_worldOffset = offset;
        this.m_screenOffset = new Vector3((float) (-(double) this.progressSlider.Width / (2.0 * (double) GameUIRoot.GameUIScalar)) * this.progressSlider.PixelsToUnits(), 0.0f, 0.0f);
        this.m_reloadIsActive = true;
        this.activeReloadSprite.enabled = true;
        this.progressSlider.Thumb.enabled = true;
        this.celebrationSprite.enabled = true;
        dfSpriteAnimation component = this.celebrationSprite.GetComponent<dfSpriteAnimation>();
        component.Stop();
        component.SetFrameExternal(0);
        this.celebrationSprite.enabled = false;
        this.progressSlider.Color = (Color32) Color.white;
        float width = this.progressSlider.Width;
        float maxValue = this.progressSlider.MaxValue;
        float num1 = (float) this.startValue / maxValue * width;
        float num2 = (float) this.endValue / maxValue * width;
        float x = num1 + (num2 - num1) * activeReloadStartPercent;
        float num3 = (float) pixelWidth * Pixelator.Instance.CurrentTileScale;
        this.activeReloadSprite.RelativePosition = (Vector3) GameUIUtility.QuantizeUIPosition((Vector2) this.activeReloadSprite.RelativePosition.WithX(x));
        this.celebrationSprite.RelativePosition = this.activeReloadSprite.RelativePosition + new Vector3(Pixelator.Instance.CurrentTileScale * -1f, Pixelator.Instance.CurrentTileScale * -2f, 0.0f);
        this.activeReloadSprite.Width = num3;
        this.m_activeReloadStartValue = Mathf.RoundToInt((float) (this.endValue - this.startValue) * activeReloadStartPercent) + this.startValue - this.lieFactor / 2;
        this.m_activeReloadEndValue = this.m_activeReloadStartValue + this.lieFactor;
        bool flag = (bool) (UnityEngine.Object) attachParent && (bool) (UnityEngine.Object) attachParent.CurrentGun && attachParent.CurrentGun.LocalActiveReload;
        if (attachParent.IsPrimaryPlayer)
          this.activeReloadSprite.IsVisible = Gun.ActiveReloadActivated || flag;
        else
          this.activeReloadSprite.IsVisible = Gun.ActiveReloadActivatedPlayerTwo || flag;
        this.StartCoroutine(this.HandlePlayerReloadBar(duration));
      }

      public bool IsActiveReloadGracePeriod()
      {
        return (double) this.progressSlider.Value <= 0.30000001192092896 * (double) this.progressSlider.MaxValue;
      }

      public bool AttemptActiveReload()
      {
        if (!this.m_reloadIsActive)
          return false;
        if ((double) this.progressSlider.Value >= (double) this.m_activeReloadStartValue && (double) this.progressSlider.Value <= (double) this.m_activeReloadEndValue)
        {
          this.progressSlider.Color = (Color32) Color.green;
          int num = (int) AkSoundEngine.PostEvent("Play_WPN_active_reload_01", this.gameObject);
          this.celebrationSprite.enabled = true;
          this.activeReloadSprite.enabled = false;
          this.progressSlider.Thumb.enabled = false;
          this.m_reloadIsActive = false;
          this.celebrationSprite.GetComponent<dfSpriteAnimation>().Play();
          return true;
        }
        this.progressSlider.Color = (Color32) Color.red;
        return false;
      }

      public void CancelReload()
      {
        this.m_reloadIsActive = false;
        this.m_isReloading = false;
        this.progressSlider.IsVisible = false;
      }

      private Vector3 ConvertWorldSpaces(Vector3 inPoint, Camera inCamera, Camera outCamera)
      {
        Vector3 viewportPoint = inCamera.WorldToViewportPoint(inPoint);
        return outCamera.ViewportToWorldPoint(viewportPoint);
      }

      [DebuggerHidden]
      private IEnumerator HandlePlayerReloadBar(float duration)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new GameUIReloadBarController.\u003CHandlePlayerReloadBar\u003Ec__Iterator0()
        {
          duration = duration,
          \u0024this = this
        };
      }

      public bool AnyStatusBarVisible()
      {
        return this.statusBarDrain.IsVisible || this.statusBarCurse.IsVisible || this.statusBarFire.IsVisible || this.statusBarPoison.IsVisible;
      }

      public void UpdateStatusBars(PlayerController player)
      {
        if ((UnityEngine.Object) this.statusBarPoison == (UnityEngine.Object) null || (UnityEngine.Object) this.statusBarDrain == (UnityEngine.Object) null || (UnityEngine.Object) this.statusBarPoison == (UnityEngine.Object) null)
          return;
        this.StatusBarPanel.transform.localScale = Vector3.one / GameUIRoot.GameUIScalar;
        if (!(bool) (UnityEngine.Object) player || player.healthHaver.IsDead || GameManager.Instance.IsPaused)
        {
          this.statusBarPoison.IsVisible = false;
          this.statusBarDrain.IsVisible = false;
          this.statusBarFire.IsVisible = false;
          this.statusBarCurse.IsVisible = false;
        }
        else
        {
          this.m_attachPlayer = player;
          this.worldCamera = GameManager.Instance.MainCameraController.GetComponent<Camera>();
          this.uiCamera = this.progressSlider.GetManager().RenderCamera;
          this.m_worldOffset = new Vector3(0.1f, (float) ((double) player.SpriteDimensions.y / 2.0 + 0.25), 0.0f);
          this.m_screenOffset = new Vector3((float) (-(double) this.progressSlider.Width / (2.0 * (double) GameUIRoot.GameUIScalar)) * this.progressSlider.PixelsToUnits(), 0.0f, 0.0f);
          if ((double) player.CurrentPoisonMeterValue > 0.0)
          {
            this.statusBarPoison.IsVisible = true;
            this.statusBarPoison.Value = player.CurrentPoisonMeterValue;
          }
          else
            this.statusBarPoison.IsVisible = false;
          if ((double) player.CurrentCurseMeterValue > 0.0)
          {
            this.statusBarCurse.IsVisible = true;
            this.statusBarCurse.Value = player.CurrentCurseMeterValue;
          }
          else
            this.statusBarCurse.IsVisible = false;
          if (player.IsOnFire)
          {
            this.statusBarFire.IsVisible = true;
            this.statusBarFire.Value = player.CurrentFireMeterValue;
          }
          else
            this.statusBarFire.IsVisible = false;
          if ((double) player.CurrentDrainMeterValue > 0.0)
          {
            this.statusBarDrain.IsVisible = true;
            this.statusBarDrain.Value = player.CurrentDrainMeterValue;
          }
          else
            this.statusBarDrain.IsVisible = false;
          int num1 = 0;
          for (int index = 0; index < 4; ++index)
          {
            dfProgressBar dfProgressBar = (dfProgressBar) null;
            switch (index)
            {
              case 0:
                dfProgressBar = this.statusBarPoison;
                break;
              case 1:
                dfProgressBar = this.statusBarDrain;
                break;
              case 2:
                dfProgressBar = this.statusBarFire;
                break;
              case 3:
                dfProgressBar = this.statusBarCurse;
                break;
            }
            if (dfProgressBar.IsVisible)
              ++num1;
          }
          float num2 = 0.0f;
          int b = (num1 - 1) * 18;
          for (int index = 0; index < 4; ++index)
          {
            dfProgressBar dfProgressBar = (dfProgressBar) null;
            switch (index)
            {
              case 0:
                dfProgressBar = this.statusBarPoison;
                break;
              case 1:
                dfProgressBar = this.statusBarDrain;
                break;
              case 2:
                dfProgressBar = this.statusBarFire;
                break;
              case 3:
                dfProgressBar = this.statusBarCurse;
                break;
            }
            if (dfProgressBar.IsVisible)
            {
              float x = (float) b;
              if (b != 0)
                x = Mathf.Lerp((float) -b, (float) b, num2 / ((float) num1 - 1f));
              dfProgressBar.RelativePosition = new Vector3(36f, -12f / GameUIRoot.GameUIScalar, 0.0f) + new Vector3(x, 0.0f, 0.0f);
              ++num2;
            }
          }
        }
      }

      private void OnMainCameraFinishedFrame()
      {
        if (!(bool) (UnityEngine.Object) this.m_attachPlayer || !this.progressSlider.IsVisible && !this.AnyStatusBarVisible())
          return;
        this.progressSlider.transform.position = (Vector3) (Vector2) (this.ConvertWorldSpaces((Vector3) (Vector2) (this.m_attachPlayer.LockedApproximateSpriteCenter + this.m_worldOffset), this.worldCamera, this.uiCamera).WithZ(0.0f) + this.m_screenOffset);
        this.progressSlider.transform.position = this.progressSlider.transform.position.QuantizeFloor(this.progressSlider.PixelsToUnits() / (Pixelator.Instance.ScaleTileScale / Pixelator.Instance.CurrentTileScale));
        if (!((UnityEngine.Object) this.StatusBarPanel != (UnityEngine.Object) null))
          return;
        this.StatusBarPanel.transform.position = this.progressSlider.transform.position - new Vector3(0.0f, -48f * this.progressSlider.PixelsToUnits(), 0.0f);
      }

      protected override void OnDestroy()
      {
        if (GameManager.HasInstance && (bool) (UnityEngine.Object) GameManager.Instance.MainCameraController)
          GameManager.Instance.MainCameraController.OnFinishedFrame -= new System.Action(this.OnMainCameraFinishedFrame);
        base.OnDestroy();
      }
    }

}
