// Decompiled with JetBrains decompiler
// Type: PastCameraUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public static class PastCameraUtility
    {
      public static void UnlockConversation()
      {
        GameManager.Instance.PrimaryPlayer.ClearInputOverride("past");
        if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
          GameManager.Instance.SecondaryPlayer.ClearInputOverride("past");
        Pixelator.Instance.LerpToLetterbox(0.5f, 0.25f);
        Pixelator.Instance.DoFinalNonFadedLayer = false;
        GameUIRoot.Instance.ToggleLowerPanels(true, source: string.Empty);
        GameUIRoot.Instance.ShowCoreUI(string.Empty);
        GameManager.Instance.MainCameraController.SetManualControl(false);
      }

      public static void LockConversation(Vector2 lockPos)
      {
        GameManager.Instance.PrimaryPlayer.SetInputOverride("past");
        if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
          GameManager.Instance.SecondaryPlayer.SetInputOverride("past");
        Pixelator.Instance.LerpToLetterbox(0.35f, 0.25f);
        Pixelator.Instance.DoFinalNonFadedLayer = true;
        GameUIRoot.Instance.ToggleLowerPanels(false, source: string.Empty);
        GameUIRoot.Instance.HideCoreUI(string.Empty);
        Vector2 vector = lockPos;
        CameraController cameraController = GameManager.Instance.MainCameraController;
        cameraController.SetManualControl(true);
        cameraController.OverridePosition = vector.ToVector3ZUp();
      }
    }

}
