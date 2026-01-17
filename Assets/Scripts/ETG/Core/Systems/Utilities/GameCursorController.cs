// Decompiled with JetBrains decompiler
// Type: GameCursorController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Runtime.InteropServices;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class GameCursorController : MonoBehaviour
    {
      public static OverridableBool CursorOverride = new OverridableBool(false);
      public Texture2D normalCursor;
      public Texture2D[] cursors;
      public float sizeMultiplier = 4f;
      private Vector3 lastPosition;
      private GameCursorController.RECT m_originalClippingRect;

      private static bool showMouseCursor
      {
        get
        {
          if (GameCursorController.CursorOverride.Value || GameManager.Instance.IsLoadingLevel || GameManager.IsReturningToBreach)
            return false;
          if (GameManager.Instance.IsSelectingCharacter && BraveInput.PlayerlessInstance.IsKeyboardAndMouse(true))
            return true;
          if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
          {
            if (!BraveInput.GetInstanceForPlayer(0).HasMouse() && !BraveInput.GetInstanceForPlayer(1).HasMouse())
              return false;
          }
          else if (!BraveInput.GetInstanceForPlayer(0).HasMouse())
            return false;
          return true;
        }
      }

      private static bool showPlayerOneControllerCursor
      {
        get
        {
          return !GameCursorController.CursorOverride.Value && !GameManager.Instance.IsLoadingLevel && !GameManager.IsReturningToBreach && !BraveInput.GetInstanceForPlayer(0).IsKeyboardAndMouse() && GameManager.Options.PlayerOneControllerCursor;
        }
      }

      private static bool showPlayerTwoControllerCursor
      {
        get
        {
          return GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && !GameCursorController.CursorOverride.Value && !GameManager.Instance.IsLoadingLevel && !GameManager.IsReturningToBreach && !BraveInput.GetInstanceForPlayer(1).IsKeyboardAndMouse() && GameManager.Options.PlayerTwoControllerCursor;
        }
      }

      private void Start()
      {
        GameCursorController.GetClipCursor(out this.m_originalClippingRect);
        Cursor.visible = false;
        Cursor.lockState = GameManager.Options.CurrentPreferredFullscreenMode != GameOptions.PreferredFullscreenMode.FULLSCREEN ? CursorLockMode.None : CursorLockMode.Confined;
      }

      [DllImport("user32.dll")]
      private static extern bool ClipCursor(ref GameCursorController.RECT lpRect);

      [DllImport("user32.dll")]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool GetClipCursor(out GameCursorController.RECT rcClip);

      public void ToggleClip(bool clipToWindow)
      {
        if (clipToWindow)
        {
          GameCursorController.RECT lpRect;
          lpRect.Left = 0;
          lpRect.Top = 0;
          lpRect.Right = Screen.width - 1;
          lpRect.Bottom = Screen.height - 1;
          GameCursorController.ClipCursor(ref lpRect);
        }
        else
          GameCursorController.ClipCursor(ref this.m_originalClippingRect);
      }

      private void OnDestroy() => GameCursorController.ClipCursor(ref this.m_originalClippingRect);

      public void DrawCursor()
      {
        if (!BraveUtility.isLoadingLevel && GameManager.HasInstance && (Object) GameManager.Instance.Dungeon != (Object) null)
          Cursor.visible = false;
        if (!GameManager.HasInstance)
          return;
        Texture2D texture2D = this.normalCursor;
        int currentCursorIndex = GameManager.Options.CurrentCursorIndex;
        if (currentCursorIndex >= 0 && currentCursorIndex < this.cursors.Length)
          texture2D = this.cursors[currentCursorIndex];
        if (GameCursorController.showMouseCursor)
        {
          Vector2 mousePosition = BraveInput.GetInstanceForPlayer(0).MousePosition;
          mousePosition.y = (float) Screen.height - mousePosition.y;
          Vector2 vector2 = new Vector2((float) texture2D.width, (float) texture2D.height) * (!((Object) Pixelator.Instance != (Object) null) ? 3f : (float) (int) Pixelator.Instance.ScaleTileScale);
          Rect screenRect = new Rect((float) ((double) mousePosition.x + 0.5 - (double) vector2.x / 2.0), (float) ((double) mousePosition.y + 0.5 - (double) vector2.y / 2.0), vector2.x, vector2.y);
          bool flag = false;
          if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && BraveInput.GetInstanceForPlayer(1).IsKeyboardAndMouse())
          {
            flag = true;
            Graphics.DrawTexture(screenRect, (Texture) texture2D, new Rect(0.0f, 0.0f, 1f, 1f), 0, 0, 0, 0, new Color(0.402f, 0.111f, 0.32f));
          }
          if (!flag)
            Graphics.DrawTexture(screenRect, (Texture) texture2D);
        }
        if (GameCursorController.showPlayerOneControllerCursor && !GameManager.Instance.IsPaused && !GameManager.IsBossIntro)
        {
          PlayerController primaryPlayer = GameManager.Instance.PrimaryPlayer;
          BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(0);
          if ((bool) (Object) primaryPlayer && instanceForPlayer.ActiveActions.Aim.Vector != Vector2.zero && (primaryPlayer.CurrentInputState == PlayerInputState.AllInput || primaryPlayer.IsInMinecart))
          {
            Vector2 screenViewport = BraveCameraUtility.ConvertGameViewportToScreenViewport((Vector2) Camera.main.WorldToViewportPoint((Vector3) (primaryPlayer.CenterPosition + instanceForPlayer.ActiveActions.Aim.Vector.normalized * 5f)));
            Vector2 vector2_1 = new Vector2(screenViewport.x * (float) Screen.width, (1f - screenViewport.y) * (float) Screen.height);
            Vector2 vector2_2 = new Vector2((float) texture2D.width, (float) texture2D.height) * (!((Object) Pixelator.Instance != (Object) null) ? 3f : (float) (int) Pixelator.Instance.ScaleTileScale);
            Graphics.DrawTexture(new Rect((float) ((double) vector2_1.x + 0.5 - (double) vector2_2.x / 2.0), (float) ((double) vector2_1.y + 0.5 - (double) vector2_2.y / 2.0), vector2_2.x, vector2_2.y), (Texture) texture2D);
          }
        }
        if (GameCursorController.showPlayerTwoControllerCursor && !GameManager.Instance.IsPaused && !GameManager.IsBossIntro)
        {
          PlayerController secondaryPlayer = GameManager.Instance.SecondaryPlayer;
          BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(1);
          if ((bool) (Object) secondaryPlayer && instanceForPlayer.ActiveActions.Aim.Vector != Vector2.zero && (secondaryPlayer.CurrentInputState == PlayerInputState.AllInput || secondaryPlayer.IsInMinecart))
          {
            Vector2 screenViewport = BraveCameraUtility.ConvertGameViewportToScreenViewport((Vector2) Camera.main.WorldToViewportPoint((Vector3) (secondaryPlayer.CenterPosition + instanceForPlayer.ActiveActions.Aim.Vector.normalized * 5f)));
            Vector2 vector2_3 = new Vector2(screenViewport.x * (float) Screen.width, (1f - screenViewport.y) * (float) Screen.height);
            Vector2 vector2_4 = new Vector2((float) texture2D.width, (float) texture2D.height) * (!((Object) Pixelator.Instance != (Object) null) ? 3f : (float) (int) Pixelator.Instance.ScaleTileScale);
            Graphics.DrawTexture(new Rect((float) ((double) vector2_3.x + 0.5 - (double) vector2_4.x / 2.0), (float) ((double) vector2_3.y + 0.5 - (double) vector2_4.y / 2.0), vector2_4.x, vector2_4.y), (Texture) texture2D, new Rect(0.0f, 0.0f, 1f, 1f), 0, 0, 0, 0, new Color(0.402f, 0.111f, 0.32f));
          }
        }
        if (GameManager.Options.CurrentPreferredFullscreenMode == GameOptions.PreferredFullscreenMode.FULLSCREEN)
          Cursor.lockState = !GameManager.Instance.IsPaused || !GameUIRoot.Instance.PauseMenuPanel.IsVisible && !GameUIRoot.Instance.HasOpenPauseSubmenu() ? CursorLockMode.Confined : CursorLockMode.None;
        else
          Cursor.lockState = CursorLockMode.None;
      }

      private void OnGUI() => this.DrawCursor();

      public struct RECT
      {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
      }
    }

}
